using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OthelloGameManager : MonoBehaviour
{
    
    private const int BoardSize = 8; // �I�Z���{�[�h�̃T�C�Y
    private int[,] board = new int[BoardSize, BoardSize]; // �{�[�h�̏�ԁi0: ��, 1: ��, 2: ���j

    public GameObject blackPiecePrefab; // ������̃v���n�u
    public GameObject whitePiecePrefab; // ������̃v���n�u
    public Transform boardTransform; // �{�[�h��Transform
    public GameObject highlightPrefab; // �n�C���C�g�̃v���n�u

    private int currentPlayer = 1; // ���݂̃v���C���[�i1: ��, 2: ���j
    private Dictionary<Vector2Int, GameObject> pieces = new Dictionary<Vector2Int, GameObject>(); // ��̈ʒu�ƃI�u�W�F�N�g�̃}�b�s���O
    private GameObject currentHighlight; // ���݂̃n�C���C�g�I�u�W�F�N�g

    void Start()
    {
        InitializeBoard(); // �{�[�h�̏�����
    }

    private void InitializeBoard()
    {
        // �{�[�h����ɂ���
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                board[x, y] = 0;
            }
        }

        // �����̋��z�u����
        PlacePiece(3, 3, 2); // ��
        PlacePiece(3, 4, 1); // ��
        PlacePiece(4, 3, 1); // ��
        PlacePiece(4, 4, 2); // ��
    }

    public void UpdateHighlight(Vector2Int cursorPosition)
    {
        // �ȑO�̃n�C���C�g���폜
        if (currentHighlight != null)
        {
            Destroy(currentHighlight);
        }

        // �J�[�\���ʒu�Ƀn�C���C�g��\��
        if (board[cursorPosition.x, cursorPosition.y] == 0)
        {
            Vector3 position = new Vector3(cursorPosition.x, 0.05f, cursorPosition.y);
            currentHighlight = Instantiate(highlightPrefab, position, Quaternion.identity, boardTransform);
        }
    }

    public bool PlacePiece(int x, int y, int player, bool isStrongPlace = false)
    {
        // �{�[�h�͈̔͂��`�F�b�N
        if (x < 0 || x >= BoardSize || y < 0 || y >= BoardSize)
        {
            Debug.Log($"Invalid board position: x = {x}, y = {y}");
            return false;
        }

        // ���ɋ�u����Ă��邩���`�F�b�N
        if (board[x, y] != 0)
        {
            Debug.Log($"Position already occupied: x = {x}, y = {y}");
            return false;
        }

        bool validMove = false;
        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

        List<Vector2Int> piecesToFlip = new List<Vector2Int>(); // �Ђ�����Ԃ���̃��X�g

        // 8�����̃`�F�b�N
        for (int dir = 0; dir < 8; dir++)
        {
            int nx = x + dx[dir];
            int ny = y + dy[dir];
            bool hasOpponent = false;
            List<Vector2Int> tempFlip = new List<Vector2Int>();

            // 1�����ɉ����ċ���`�F�b�N
            while (nx >= 0 && nx < BoardSize && ny >= 0 && ny < BoardSize)
            {
                if (board[nx, ny] == 0) break; // ��̃}�X�ɓ��B������I��
                if (board[nx, ny] == player)
                {
                    if (hasOpponent)
                    {
                        piecesToFlip.AddRange(tempFlip); // �Ђ�����Ԃ����ǉ�
                        validMove = true; // �L���Ȏ�Ɣ���
                    }
                    break;
                }
                else
                {
                    hasOpponent = true; // ����̋�𔭌�
                    tempFlip.Add(new Vector2Int(nx, ny)); // �Ђ�����Ԃ����ɒǉ�
                }
                nx += dx[dir];
                ny += dy[dir];
            }
        }

        // ���u������
        if (validMove || IsInitialPlacement(x, y))
        {
            board[x, y] = player;
            PlacePieceVisual(x, y, player); // ��̃r�W���A�����X�V

            // �Ђ�����Ԃ���̃r�W���A�����X�V
            foreach (var pos in piecesToFlip)
            {
                board[pos.x, pos.y] = player;
                PlacePieceVisual(pos.x, pos.y, player);
            }

            // �����u���ꍇ�̏���
            if (isStrongPlace)
            {
                FlipRandomAdjacentPieces(x, y, player); // �����_���Ɏ��͂̋���Ђ�����Ԃ�
            }

            Debug.Log($"Piece placed at x = {x}, y = {y} for player = {player}");
            return true;
        }
        else
        {
            Debug.Log("Move is not valid according to the rules");
            return false;
        }
    }

    // �����u�������Ɏ��͂̋�������_���ɂЂ�����Ԃ�����
    private void FlipRandomAdjacentPieces(int x, int y, int player)
    {
        List<Vector2Int> adjacentPositions = new List<Vector2Int>();

        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

        for (int dir = 0; dir < 8; dir++)
        {
            int nx = x + dx[dir];
            int ny = y + dy[dir];
            if (nx >= 0 && nx < BoardSize && ny >= 0 && ny < BoardSize)
            {
                if (board[nx, ny] != 0 && board[nx, ny] != player)
                {
                    adjacentPositions.Add(new Vector2Int(nx, ny)); // �אڂ��鑊��̋��ǉ�
                }
            }
        }

        if (adjacentPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, adjacentPositions.Count);
            Vector2Int pos = adjacentPositions[randomIndex];
            board[pos.x, pos.y] = player;
            PlacePieceVisual(pos.x, pos.y, player); // �����_����1�Ђ�����Ԃ�
        }
    }

    // ��̃r�W���A����z�u
    private void PlacePieceVisual(int x, int y, int player)
    {
        Vector2Int pos = new Vector2Int(x, y);

        // ���ɋ����ꍇ�͍폜
        if (pieces.ContainsKey(pos))
        {
            Destroy(pieces[pos]);
            pieces.Remove(pos);
        }

        GameObject piecePrefab = (player == 1) ? blackPiecePrefab : whitePiecePrefab;
        Vector3 position = new Vector3(x, 0.1f, y);
        GameObject newPiece = Instantiate(piecePrefab, position, Quaternion.identity, boardTransform);
        pieces[pos] = newPiece; // �V�������ǉ�
    }

    // �v���C���[�����
    public void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
    }

    // ���݂̃v���C���[���擾
    public int GetCurrentPlayer()
    {
        return currentPlayer;
    }

    // �v���C���[�����u���邩�ǂ������`�F�b�N
    public bool CanPlacePiece(int player)
    {
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                if (board[x, y] == 0 && IsValidMove(x, y, player))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // �w�肳�ꂽ�ʒu�ɋ��u���̂��L�����ǂ������`�F�b�N
    private bool IsValidMove(int x, int y, int player)
    {
        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

        for (int dir = 0; dir < 8; dir++)
        {
            int nx = x + dx[dir];
            int ny = y + dy[dir];
            bool hasOpponent = false;

            while (nx >= 0 && nx < BoardSize && ny >= 0 && ny < BoardSize)
            {
                if (board[nx, ny] == 0) break;
                if (board[nx, ny] == player)
                {
                    if (hasOpponent)
                    {
                        return true;
                    }
                    break;
                }
                else
                {
                    hasOpponent = true;
                }
                nx += dx[dir];
                ny += dy[dir];
            }
        }
        return false;
    }

    // �����z�u���ǂ������`�F�b�N
    private bool IsInitialPlacement(int x, int y)
    {
        return (x == 3 && y == 3) || (x == 3 && y == 4) || (x == 4 && y == 3) || (x == 4 && y == 4);
    }
}
