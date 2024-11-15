using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private int[,] board = new int[8, 8]; // 盤面を表す配列（例えば、0が空、1が黒、-1が白とする）

    // 石を置いた際に周囲の駒をひっくり返す関数
    public void PlaceStone(int x, int y, int playerColor)
    {
        if (board[x, y] != 0) // 既に石が置かれている場合は処理しない
            return;

        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(1, 0),   // 右
            new Vector2Int(-1, 0),  // 左
            new Vector2Int(0, 1),   // 上
            new Vector2Int(0, -1),  // 下
            new Vector2Int(1, 1),   // 右上
            new Vector2Int(-1, -1), // 左下
            new Vector2Int(1, -1),  // 右下
            new Vector2Int(-1, 1)   // 左上
        };

        List<Vector2Int> flippedStones = new List<Vector2Int>();

        foreach (Vector2Int dir in directions)
        {
            List<Vector2Int> stonesToFlip = new List<Vector2Int>();
            int currX = x + dir.x;
            int currY = y + dir.y;

            while (IsValidPosition(currX, currY) && board[currX, currY] == -playerColor)
            {
                stonesToFlip.Add(new Vector2Int(currX, currY));
                currX += dir.x;
                currY += dir.y;
            }

            if (IsValidPosition(currX, currY) && board[currX, currY] == playerColor)
            {
                flippedStones.AddRange(stonesToFlip);
            }
        }

        if (flippedStones.Count > 0)
        {
            board[x, y] = playerColor; // 石を置く

            foreach (Vector2Int stonePos in flippedStones)
            {
                board[stonePos.x, stonePos.y] = playerColor; // 駒をひっくり返す
            }
        }
    }

    // 座標が盤面内かどうかを判定する関数
    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < 8 && y >= 0 && y < 8;
    }
}
