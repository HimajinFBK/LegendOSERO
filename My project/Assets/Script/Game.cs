using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


//オセロゲームの管理
public class Game : SingletonMonoBehaviour<Game>
{
    public static readonly int XNum = 8;
    public static readonly int ZNum = 8;

    

    public enum State
    {
        None,
        Initializing,
        BlackTurn,
        WhiteTurn,
        Result,
    }

    [SerializeField]
    private Stone _stonePrefab; //コマ

    [SerializeField]
    private Transform _stoneBase;

    [SerializeField]
    private Player _seifPlayer; //自分

    [SerializeField]
    private Player _enemyPlayer; //相手

    [SerializeField]
    private TextMeshPro _blackScoreText; //黒スコア

    [SerializeField]
    private TextMeshPro _WhiteScoreText; //白スコア

    [SerializeField]
    private TextMeshPro _resultText; //勝敗メッセージ

    [SerializeField]
    private GameObject _cursor; //カーソル

    public GameObject Cursor { get { return _cursor; } }
    public Stone[][] Stones { get; private set; }
    public State CurrentState { get; private set; } = State.None;

    // 駒の位置とオブジェクトのマッピング
    private Dictionary<Vector3Int, GameObject> pieces = new Dictionary<Vector3Int, GameObject>(); 

    private int[,] board;
    private const int BoardSize = 8;



    // 現在のターン数を計算して取得する
    public int CurrentTurn
    {
        get
        {
            var turnCount = 0;
            // ゲーム盤全体を走査して、石が置かれているマスの数を数える
            for (var z = 0; z < ZNum; z++)
            {
                for (var x = 0; x < XNum; x++)
                {
                    if (Stones[z][x].CurrentState != Stone.State.None)
                    {
                        turnCount++;
                    }
                }
            }
            return turnCount;
        }

    }

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("Game started");// ゲーム開始のデバッグメッセージ
        // 石の2次元配列を初期化
        Stones = new Stone[ZNum][]; //石を並べる
        for (var z = 0; z < ZNum; z++)
        {
            Stones[z] = new Stone[XNum];
            for (var x = 0; x < XNum; x++)
            { // 石の配置
                var stone = Instantiate(_stonePrefab, _stoneBase);
                var t = stone.transform;
                t.localPosition = new Vector3(x * 1.0f, 0.84f, z * 1.0f); //間隔
                t.localRotation = Quaternion.identity; //回転

                // 石を非表示にして初期化
                stone.SetActive(false, Stone.Color.Black);
                Stones[z][x] = stone;



            }
        }

        // ゲームの状態を初期化中に設定
        CurrentState = State.Initializing;

        // ボードの初期化
        board = new int[BoardSize, BoardSize];

       
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState) //ターンの動き
        {
            case State.Initializing:
                {
                    // ゲーム盤を初期化する
                    for (var z = 0; z < ZNum; z++)
                    {
                        for (var x = 0; x < XNum; x++)
                        {
                            Stones[z][x].SetActive(false, Stone.Color.Black);
                            board[x, z] = 0;
                        }
                    }

                    //初期配置の石４個
                    Stones[3][3].SetActive(true, Stone.Color.Black);
                    Stones[3][4].SetActive(true, Stone.Color.White);
                    Stones[4][3].SetActive(true, Stone.Color.White);
                    Stones[4][4].SetActive(true, Stone.Color.Black);

                    board[3, 3] = 1;
                    board[3, 4] = 2;
                    board[4, 3] = 2;
                    board[4, 4] = 1;

                    UpdateScore(); //初期スコア

                    // 結果テキストを非表示にする
                    _resultText.gameObject.SetActive(false);

                    CurrentState = State.BlackTurn; // 現在の状態を黒のターンに設定
                }
                break;

            case State.BlackTurn:
                { // プレイヤーが石を置けるかチェック
                    if (_seifPlayer.TryGetSelected(out var x, out var z))
                    {
                        // 黒の石を置き、ひっくり返し処理を行う
                        Stones[z][x].SetActive(true, Stone.Color.Black);
                        PlacePiece(x, z, 1);
                        Reverse(Stone.Color.Black, x, z);
                        UpdateScore();

                        // 敵が置けるかチェック
                        if (_enemyPlayer.CanPut())
                        {
                            CurrentState = State.WhiteTurn;
                        }
                        else if (!_seifPlayer.CanPut())
                        {
                            CurrentState = State.Result;
                        }
                    }
                }
                break;

            case State.WhiteTurn:
                {
                    // 敵が石を置けるかチェック
                    if (_enemyPlayer.TryGetSelected(out var x, out var z))
                    {
                        Stones[z][x].SetActive(true, Stone.Color.White);
                        PlacePiece(x, z, 2);
                        Reverse(Stone.Color.White, x, z);
                        UpdateScore();

                        // プレイヤーが置けるかチェック
                        if (_seifPlayer.CanPut())
                        {
                            CurrentState = State.BlackTurn;
                        }
                        else if (!_enemyPlayer.CanPut())
                        {
                            CurrentState = State.Result;
                        }
                    }
                }
                break;

            case State.Result:
                {
                    if (!_resultText.gameObject.activeSelf) //勝敗
                    {
                        int blackScore;
                        int whiteScore;
                        CalcScore(out blackScore, out whiteScore);
                        _resultText.text = blackScore < whiteScore ? "White Win"
                            : blackScore > whiteScore ? "Black Win" : "Draw";
                        _resultText.gameObject.SetActive(true);
                    }

                    var kb = Keyboard.current; //スペースキーでリセット
                    if (kb.enterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)
                    {
                        CurrentState = State.Initializing;
                    }
                }
                break;

            case State.None:
            default:
                break;
        }
    }

    private void UpdateScore()
    { //スコアの更新
        int blackScore;
        int whiteScore;
        CalcScore(out blackScore, out whiteScore);
        _blackScoreText.text = blackScore.ToString();
        _WhiteScoreText.text = whiteScore.ToString();
    }

    private void CalcScore(out int blackScore, out int whiteScore)
    { //黒と白コマのカウント
        blackScore = 0;
        whiteScore = 0;

        //各色の石の数を数える
        for (var z = 0; z < ZNum; z++)
        {
            for (var x = 0; x < XNum; x++)
            {
                if (Stones[z][x].CurrentState != Stone.State.None)
                {
                    switch (Stones[z][x].CurrentColor)
                    {
                        case Stone.Color.Black:
                            blackScore++;
                            break;
                        case Stone.Color.White:
                            whiteScore++;
                            break;
                    }
                }
            }
        }
    }

    private void Reverse(Stone.Color color, int putX, int putZ)
    {
        for (var dirZ = -1; dirZ <= 1; dirZ++)
        {
            for (var dirX = -1; dirX <= 1; dirX++)
            {
                var reverseCount = CalcReverseCount(color, putX, putZ, dirX, dirZ);
                for (var i = 1; i <= reverseCount; i++)
                {
                    Stones[putZ + dirZ * i][putX + dirX * i].Reverse();
                    board[putX + dirX * i, putZ + dirZ * i] = color == Stone.Color.Black ? 1 : 2;
                }
            }
        }
    }

    private int CalcReverseCount(Stone.Color color, int putX, int putZ, int dirX, int dirZ)
    {
        var x = putX;
        var z = putZ;
        var reverseCount = 0;
        for (var i = 0; i < 8; i++)
        {
            x += dirX;
            z += dirZ;
            if (x < 0 || x >= XNum || z < 0 || z >= ZNum)
            {
                reverseCount = 0;
                break;
            }
            var stone = Stones[z][x];
            if (stone.CurrentState == Stone.State.None)
            {
                reverseCount = 0;
                break;
            }
            else
            {
                if (stone.CurrentColor != color)
                {
                    reverseCount++;
                }
                else
                {
                    break;
                }
            }
        }
        return reverseCount;
    }

    public int CalcTotalReverseCount(Stone.Color color, int putX, int putZ)
    {
        if (Stones[putX][putZ].CurrentState != Stone.State.None)
            return 0;

        var totalReverseCount = 0;
        for (var dirZ = -1; dirZ <= 1; dirZ++)
        {
            for (var dirX = -1; dirX <= 1; dirX++)
            {
                totalReverseCount += CalcReverseCount(color, putX, putZ, dirX, dirZ);
            }
        }
        return totalReverseCount;
    }

    private void PlacePiece(int x, int y, int player)
    {
        if (board[x, y] == 0)
        {
            board[x, y] = player;
            PlacePieceVisual(x, y, player);
        }
    }

    private void PlacePieceVisual(int x, int y, int player)
    {
        var color = player == 1 ? Stone.Color.Black : Stone.Color.White;
        Stones[y][x].SetActive(true, color);
    }

    private void FlipRandomAdjacentPieces(int x, int y, int player)
    {
        List<Vector3Int> adjacentPositions = new List<Vector3Int>();

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
                    adjacentPositions.Add(new Vector3Int(nx, ny)); // 隣接する相手の駒を追加
                }
            }
        }

        if (adjacentPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, adjacentPositions.Count);
            Vector3Int pos = adjacentPositions[randomIndex];
            Debug.Log($"Flipping piece at x = {pos.x}, y = {pos.y} for player = {player}"); // デバッグメッセージ
            board[pos.x, pos.y] = player;
            PlacePieceVisual(pos.x, pos.y, player); // ランダムに1つひっくり返す
        }
        else
        {
            Debug.Log("No adjacent positions to flip");
        }
    }
}
