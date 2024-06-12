using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private int[,] board = new int[8, 8]; // �Ֆʂ�\���z��i�Ⴆ�΁A0����A1�����A-1�����Ƃ���j

    // �΂�u�����ۂɎ��͂̋���Ђ�����Ԃ��֐�
    public void PlaceStone(int x, int y, int playerColor)
    {
        if (board[x, y] != 0) // ���ɐ΂��u����Ă���ꍇ�͏������Ȃ�
            return;

        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(1, 0),   // �E
            new Vector2Int(-1, 0),  // ��
            new Vector2Int(0, 1),   // ��
            new Vector2Int(0, -1),  // ��
            new Vector2Int(1, 1),   // �E��
            new Vector2Int(-1, -1), // ����
            new Vector2Int(1, -1),  // �E��
            new Vector2Int(-1, 1)   // ����
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
            board[x, y] = playerColor; // �΂�u��

            foreach (Vector2Int stonePos in flippedStones)
            {
                board[stonePos.x, stonePos.y] = playerColor; // ����Ђ�����Ԃ�
            }
        }
    }

    // ���W���Ֆʓ����ǂ����𔻒肷��֐�
    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < 8 && y >= 0 && y < 8;
    }
}
