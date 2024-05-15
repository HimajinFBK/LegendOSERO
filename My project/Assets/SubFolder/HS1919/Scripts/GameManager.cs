using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject blackPiecePrefab; // ���΂̃v���n�u
    public GameObject whitePiecePrefab; // ���΂̃v���n�u

    private GameObject[,] board; // �Ֆʂ̃}�X��

    void Start()
    {
        InitializeBoard();
        PlaceInitialPieces();
    }

    void InitializeBoard()
    {
        board = new GameObject[8, 8]; // 8x8�̔Ֆʂ��쐬
        // �Ֆʂ̔z�u�Ȃǂ�����������R�[�h��ǉ�
    }

    void PlaceInitialPieces()
    {
        // �����z�u�Ő΂�z�u����i�^�񒆂�4�̃}�X�j
        InstantiatePiece(blackPiecePrefab, 3, 3); // ����
        InstantiatePiece(whitePiecePrefab, 3, 4); // �E��
        InstantiatePiece(whitePiecePrefab, 4, 3); // ����
        InstantiatePiece(blackPiecePrefab, 4, 4); // �E��
    }

    void InstantiatePiece(GameObject piecePrefab, int row, int col)
    {
        Vector3 position = CalculatePiecePosition(row, col);
        GameObject piece = Instantiate(piecePrefab, position, Quaternion.identity);
        board[row, col] = piece;
    }

    Vector3 CalculatePiecePosition(int row, int col)
    {
        // �}�X�ڂ̍��W������ۂ̈ʒu���v�Z����
        float offsetX = 1.0f; // �}�X�ڂ̕�
        float offsetY = 1.0f; // �}�X�ڂ̍���
        Vector3 origin = new Vector3(-3.5f, 0.0f, -3.5f); // �Ֆʂ̍����̈ʒu
        return origin + new Vector3(col * offsetX, 0.0f, row * offsetY);
    }
}
