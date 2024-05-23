using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OthelloGameManager : MonoBehaviour
{
    private const int BoardSize = 8;
    private int[,] board = new int[BoardSize, BoardSize]; // 0: empty, 1: black, 2: white

    public GameObject blackPiecePrefab;
    public GameObject whitePiecePrefab;
    public Transform boardTransform;

    private int currentPlayer = 1;

    void Start()
    {
        InitializeBoard();
    }

    void InitializeBoard()
    {
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                board[x, y] = 0;
            }
        }

        PlacePiece(3, 3, 2);
        PlacePiece(3, 4, 1);
        PlacePiece(4, 3, 1);
        PlacePiece(4, 4, 2);
    }

    public bool PlacePiece(int x, int y, int player)
    {
        // ボード範囲のチェック
        if (x < 0 || x >= BoardSize || y < 0 || y >= BoardSize || board[x, y] != 0)
        {
            return false;
        }

        bool validMove = false;
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
                        int fx = x + dx[dir];
                        int fy = y + dy[dir];
                        while (fx != nx || fy != ny)
                        {
                            board[fx, fy] = player;
                            PlacePieceVisual(fx, fy, player);
                            fx += dx[dir];
                            fy += dy[dir];
                        }
                        validMove = true;
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

        if (validMove)
        {
            board[x, y] = player;
            PlacePieceVisual(x, y, player);
        }

        return validMove;
    }

    private void PlacePieceVisual(int x, int y, int player)
    {
        GameObject piecePrefab = (player == 1) ? blackPiecePrefab : whitePiecePrefab;
        Vector3 position = new Vector3(x, 0.1f, y);
        Instantiate(piecePrefab, position, Quaternion.identity, boardTransform);
    }

    public void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
    }

    public int GetCurrentPlayer()
    {
        return currentPlayer;
    }
}
