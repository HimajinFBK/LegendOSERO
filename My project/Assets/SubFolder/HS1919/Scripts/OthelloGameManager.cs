using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OthelloGameManager : MonoBehaviour
{
    
    private const int BoardSize = 8; // オセロボードのサイズ
    private int[,] board = new int[BoardSize, BoardSize]; // ボードの状態（0: 空, 1: 黒, 2: 白）

    public GameObject blackPiecePrefab; // 黒い駒のプレハブ
    public GameObject whitePiecePrefab; // 白い駒のプレハブ
    public Transform boardTransform; // ボードのTransform
    public GameObject highlightPrefab; // ハイライトのプレハブ

    private int currentPlayer = 1; // 現在のプレイヤー（1: 黒, 2: 白）
    private Dictionary<Vector2Int, GameObject> pieces = new Dictionary<Vector2Int, GameObject>(); // 駒の位置とオブジェクトのマッピング
    private GameObject currentHighlight; // 現在のハイライトオブジェクト

    void Start()
    {
        InitializeBoard(); // ボードの初期化
    }

    private void InitializeBoard()
    {
        // ボードを空にする
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                board[x, y] = 0;
            }
        }

        // 初期の駒を配置する
        PlacePiece(3, 3, 2); // 白
        PlacePiece(3, 4, 1); // 黒
        PlacePiece(4, 3, 1); // 黒
        PlacePiece(4, 4, 2); // 白
    }

    public void UpdateHighlight(Vector2Int cursorPosition)
    {
        // 以前のハイライトを削除
        if (currentHighlight != null)
        {
            Destroy(currentHighlight);
        }

        // カーソル位置にハイライトを表示
        if (board[cursorPosition.x, cursorPosition.y] == 0)
        {
            Vector3 position = new Vector3(cursorPosition.x, 0.05f, cursorPosition.y);
            currentHighlight = Instantiate(highlightPrefab, position, Quaternion.identity, boardTransform);
        }
    }

    public bool PlacePiece(int x, int y, int player, bool isStrongPlace = false)
    {
        // ボードの範囲をチェック
        if (x < 0 || x >= BoardSize || y < 0 || y >= BoardSize)
        {
            Debug.Log($"Invalid board position: x = {x}, y = {y}");
            return false;
        }

        // 既に駒が置かれているかをチェック
        if (board[x, y] != 0)
        {
            Debug.Log($"Position already occupied: x = {x}, y = {y}");
            return false;
        }

        bool validMove = false;
        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

        List<Vector2Int> piecesToFlip = new List<Vector2Int>(); // ひっくり返す駒のリスト

        // 8方向のチェック
        for (int dir = 0; dir < 8; dir++)
        {
            int nx = x + dx[dir];
            int ny = y + dy[dir];
            bool hasOpponent = false;
            List<Vector2Int> tempFlip = new List<Vector2Int>();

            // 1方向に沿って駒をチェック
            while (nx >= 0 && nx < BoardSize && ny >= 0 && ny < BoardSize)
            {
                if (board[nx, ny] == 0) break; // 空のマスに到達したら終了
                if (board[nx, ny] == player)
                {
                    if (hasOpponent)
                    {
                        piecesToFlip.AddRange(tempFlip); // ひっくり返す駒を追加
                        validMove = true; // 有効な手と判定
                    }
                    break;
                }
                else
                {
                    hasOpponent = true; // 相手の駒を発見
                    tempFlip.Add(new Vector2Int(nx, ny)); // ひっくり返す候補に追加
                }
                nx += dx[dir];
                ny += dy[dir];
            }
        }

        // 駒を置く処理
        if (validMove || IsInitialPlacement(x, y))
        {
            board[x, y] = player;
            PlacePieceVisual(x, y, player); // 駒のビジュアルを更新

            // ひっくり返す駒のビジュアルを更新
            foreach (var pos in piecesToFlip)
            {
                board[pos.x, pos.y] = player;
                PlacePieceVisual(pos.x, pos.y, player);
            }

            // 強く置く場合の処理
            if (isStrongPlace)
            {
                FlipRandomAdjacentPieces(x, y, player); // ランダムに周囲の駒をひっくり返す
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

    // 強く置いた時に周囲の駒をランダムにひっくり返す処理
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
                    adjacentPositions.Add(new Vector2Int(nx, ny)); // 隣接する相手の駒を追加
                }
            }
        }

        if (adjacentPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, adjacentPositions.Count);
            Vector2Int pos = adjacentPositions[randomIndex];
            board[pos.x, pos.y] = player;
            PlacePieceVisual(pos.x, pos.y, player); // ランダムに1つひっくり返す
        }
    }

    // 駒のビジュアルを配置
    private void PlacePieceVisual(int x, int y, int player)
    {
        Vector2Int pos = new Vector2Int(x, y);

        // 既に駒がある場合は削除
        if (pieces.ContainsKey(pos))
        {
            Destroy(pieces[pos]);
            pieces.Remove(pos);
        }

        GameObject piecePrefab = (player == 1) ? blackPiecePrefab : whitePiecePrefab;
        Vector3 position = new Vector3(x, 0.1f, y);
        GameObject newPiece = Instantiate(piecePrefab, position, Quaternion.identity, boardTransform);
        pieces[pos] = newPiece; // 新しい駒を追加
    }

    // プレイヤーを交代
    public void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
    }

    // 現在のプレイヤーを取得
    public int GetCurrentPlayer()
    {
        return currentPlayer;
    }

    // プレイヤーが駒を置けるかどうかをチェック
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

    // 指定された位置に駒を置くのが有効かどうかをチェック
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

    // 初期配置かどうかをチェック
    private bool IsInitialPlacement(int x, int y)
    {
        return (x == 3 && y == 3) || (x == 3 && y == 4) || (x == 4 && y == 3) || (x == 4 && y == 4);
    }
}
