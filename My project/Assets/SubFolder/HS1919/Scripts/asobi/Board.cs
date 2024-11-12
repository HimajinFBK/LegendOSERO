using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Board : MonoBehaviour
{
    public GameObject cellPrefab; // �Z���̃v���n�u
    public GameObject blackPiecePrefab; // ����̃v���n�u
    public GameObject whitePiecePrefab; // ����̃v���n�u
    private Cell[,] cells = new Cell[8, 8];

    void Start()  // �v���n�u�����蓖�Ă��Ă��邩�m�F
    {
        if (cellPrefab == null || blackPiecePrefab == null || whitePiecePrefab == null)
        {
            Debug.LogError("Prefabs are not assigned!");
            return;
        }

        CreateBoard(); // �{�[�h���쐬
        InitializePieces(); // �����̋��z�u
    }

    void CreateBoard()
    { // 8x8�̃{�[�h�𐶐�
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            { // cellPrefab���C���X�^���X�����Ĕz�u
                GameObject cellObj = Instantiate(cellPrefab, new Vector3(x, 0, y), Quaternion.identity);
                cells[x, y] = cellObj.GetComponent<Cell>();
                if (cells[x, y] == null)
                {// Cell�R���|�[�l���g��������Ȃ��ꍇ�G���[���b�Z�[�W��\��
                    Debug.LogError("Cell component is missing on the instantiated prefab at (" + x + ", " + y + ")");
                    return;
                }
                cells[x, y].SetPosition(x, y); // �Z���̈ʒu��ݒ�
            }
        }
    }

    void InitializePieces()
    {
        // �����z�u: ������4�̋��ݒ�
        PlaceInitialPiece(3, 3, PieceColor.White);
        PlaceInitialPiece(4, 4, PieceColor.White);
        PlaceInitialPiece(3, 4, PieceColor.Black);
        PlaceInitialPiece(4, 3, PieceColor.Black);
    }

    void PlaceInitialPiece(int x, int y, PieceColor color)
    { // ��̃v���n�u��I��
        GameObject piecePrefab = (color == PieceColor.Black) ? blackPiecePrefab : whitePiecePrefab;
        // ����C���X�^���X�����Ĕz�u

        GameObject piece = Instantiate(piecePrefab, new Vector3(x, 0.5f, y), Quaternion.identity);
        piece.transform.SetParent(cells[x, y].transform);// ����Z���̎q�I�u�W�F�N�g�ɐݒ�
        cells[x, y].SetPiece(color);// �Z���ɋ�̐F��ݒ�
    }

    public Cell GetCell(int x, int y)
    { // �w�肳�ꂽ�ʒu���{�[�h�͈͓̔����m�F
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            return cells[x, y];// �Z����Ԃ�
        }
        return null; // �͈͊O�̏ꍇ��null��Ԃ�
    }

    public void PlacePiece(int x, int y, PieceColor color)
    {
        cells[x, y].SetPiece(color);// �Z���ɋ��z�u
        FlipPieces(x, y, color);// ��𗠕Ԃ����������s
    }

    void FlipPieces(int x, int y, PieceColor color)
    {// 8�������`
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1),
            new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1)
        };

        foreach (Vector2Int dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;
            bool hasOpponentPiece = false;// ����̋���邩�ǂ���
            // �{�[�h�͈͓̔����`�F�b�N���Ȃ���i��
            while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
            {
                Cell cell = cells[nx, ny];// ���݂̃Z�����擾
                if (cell.GetPieceColor() == PieceColor.None)
                {
                    break;// ��Ȃ��ꍇ���[�v�𔲂���
                }
                if (cell.GetPieceColor() == color)
                {
                    if (hasOpponentPiece)
                    {// ��𗠕Ԃ�����
                        int flipX = x + dir.x;
                        int flipY = y + dir.y;
                        while (flipX != nx || flipY != ny)
                        {
                            cells[flipX, flipY].SetPiece(color);
                            flipX += dir.x;
                            flipY += dir.y;
                        }
                    }
                    break;// �����F�̋�ɏo������烋�[�v�𔲂���
                }
                hasOpponentPiece = true;// ����̋��������
                nx += dir.x;
                ny += dir.y;
            }
        }
    }
}


