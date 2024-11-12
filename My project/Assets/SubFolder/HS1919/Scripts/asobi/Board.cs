using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Board : MonoBehaviour
{
    public GameObject cellPrefab; // セルのプレハブ
    public GameObject blackPiecePrefab; // 黒駒のプレハブ
    public GameObject whitePiecePrefab; // 白駒のプレハブ
    private Cell[,] cells = new Cell[8, 8];

    void Start()  // プレハブが割り当てられているか確認
    {
        if (cellPrefab == null || blackPiecePrefab == null || whitePiecePrefab == null)
        {
            Debug.LogError("Prefabs are not assigned!");
            return;
        }

        CreateBoard(); // ボードを作成
        InitializePieces(); // 初期の駒を配置
    }

    void CreateBoard()
    { // 8x8のボードを生成
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            { // cellPrefabをインスタンス化して配置
                GameObject cellObj = Instantiate(cellPrefab, new Vector3(x, 0, y), Quaternion.identity);
                cells[x, y] = cellObj.GetComponent<Cell>();
                if (cells[x, y] == null)
                {// Cellコンポーネントが見つからない場合エラーメッセージを表示
                    Debug.LogError("Cell component is missing on the instantiated prefab at (" + x + ", " + y + ")");
                    return;
                }
                cells[x, y].SetPosition(x, y); // セルの位置を設定
            }
        }
    }

    void InitializePieces()
    {
        // 初期配置: 中央の4つの駒を設定
        PlaceInitialPiece(3, 3, PieceColor.White);
        PlaceInitialPiece(4, 4, PieceColor.White);
        PlaceInitialPiece(3, 4, PieceColor.Black);
        PlaceInitialPiece(4, 3, PieceColor.Black);
    }

    void PlaceInitialPiece(int x, int y, PieceColor color)
    { // 駒のプレハブを選択
        GameObject piecePrefab = (color == PieceColor.Black) ? blackPiecePrefab : whitePiecePrefab;
        // 駒をインスタンス化して配置

        GameObject piece = Instantiate(piecePrefab, new Vector3(x, 0.5f, y), Quaternion.identity);
        piece.transform.SetParent(cells[x, y].transform);// 駒をセルの子オブジェクトに設定
        cells[x, y].SetPiece(color);// セルに駒の色を設定
    }

    public Cell GetCell(int x, int y)
    { // 指定された位置がボードの範囲内か確認
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            return cells[x, y];// セルを返す
        }
        return null; // 範囲外の場合はnullを返す
    }

    public void PlacePiece(int x, int y, PieceColor color)
    {
        cells[x, y].SetPiece(color);// セルに駒を配置
        FlipPieces(x, y, color);// 駒を裏返す処理を実行
    }

    void FlipPieces(int x, int y, PieceColor color)
    {// 8方向を定義
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1),
            new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1)
        };

        foreach (Vector2Int dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;
            bool hasOpponentPiece = false;// 相手の駒があるかどうか
            // ボードの範囲内をチェックしながら進む
            while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
            {
                Cell cell = cells[nx, ny];// 現在のセルを取得
                if (cell.GetPieceColor() == PieceColor.None)
                {
                    break;// 駒がない場合ループを抜ける
                }
                if (cell.GetPieceColor() == color)
                {
                    if (hasOpponentPiece)
                    {// 駒を裏返す処理
                        int flipX = x + dir.x;
                        int flipY = y + dir.y;
                        while (flipX != nx || flipY != ny)
                        {
                            cells[flipX, flipY].SetPiece(color);
                            flipX += dir.x;
                            flipY += dir.y;
                        }
                    }
                    break;// 同じ色の駒に出会ったらループを抜ける
                }
                hasOpponentPiece = true;// 相手の駒が見つかった
                nx += dir.x;
                ny += dir.y;
            }
        }
    }
}


