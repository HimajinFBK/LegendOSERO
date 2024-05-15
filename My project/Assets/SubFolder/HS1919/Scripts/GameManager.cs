using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject blackPiecePrefab; // 黒石のプレハブ
    public GameObject whitePiecePrefab; // 白石のプレハブ

    private GameObject[,] board; // 盤面のマス目

    void Start()
    {
        InitializeBoard();
        PlaceInitialPieces();
    }

    void InitializeBoard()
    {
        board = new GameObject[8, 8]; // 8x8の盤面を作成
        // 盤面の配置などを初期化するコードを追加
    }

    void PlaceInitialPieces()
    {
        // 初期配置で石を配置する（真ん中の4つのマス）
        InstantiatePiece(blackPiecePrefab, 3, 3); // 左上
        InstantiatePiece(whitePiecePrefab, 3, 4); // 右上
        InstantiatePiece(whitePiecePrefab, 4, 3); // 左下
        InstantiatePiece(blackPiecePrefab, 4, 4); // 右下
    }

    void InstantiatePiece(GameObject piecePrefab, int row, int col)
    {
        Vector3 position = CalculatePiecePosition(row, col);
        GameObject piece = Instantiate(piecePrefab, position, Quaternion.identity);
        board[row, col] = piece;
    }

    Vector3 CalculatePiecePosition(int row, int col)
    {
        // マス目の座標から実際の位置を計算する
        float offsetX = 1.0f; // マス目の幅
        float offsetY = 1.0f; // マス目の高さ
        Vector3 origin = new Vector3(-3.5f, 0.0f, -3.5f); // 盤面の左下の位置
        return origin + new Vector3(col * offsetX, 0.0f, row * offsetY);
    }
}
