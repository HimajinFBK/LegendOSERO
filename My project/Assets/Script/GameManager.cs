using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;// カメラ
    [SerializeField]
    private LayerMask boardLayer;//ボードレイヤー
    [SerializeField]
    private Disc discBlackUP;
    [SerializeField]           //黒と白のディスクのプレハブ
    private Disc discWhiteUp;

    [SerializeField]
    private GameObject highlightPrefab;//ハイライト用プレハブ
    [SerializeField]
    private UIManager uiManager;//UIマネージャーを指定

    // 各プレイヤーに対応するディスクのプレハブを保持
    private Dictionary<Player, Disc> discPrefabs = new Dictionary<Player, Disc>();

    // ゲームの状態を保持するGameSteteクラスのインスタンス
    private GameStete gameState = new GameStete();

    // ボード上のディスクの状態を保持する2次元配列
    private Disc[,] discs = new Disc[8, 8];

    // プレイヤーが手を打てる状態かを示すフラグ
    private bool canMove = true;

    // 合法手のハイライトを表示するGameObjectのリスト
    private List<GameObject> highlights = new List<GameObject>();

    // Startメソッドはスクリプトが有効になったときに最初に実行される
    void Start()
    {
        // 黒と白のプレイヤーに対応するディスクのプレハブを登録
        discPrefabs[Player.Black] = discBlackUP;
        discPrefabs[Player.White] = discWhiteUp;

        // 初期ディスクの配置と合法手のハイライト表示、UI更新
        AddStartDiscs();
        ShowLeglMoves();
        uiManager.SetPlayerText(gameState.CurrentPlayer);
    }

    // Updateメソッドは毎フレーム実行される
    private void Update()
    {
        // Escapeキーでゲームを終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // 左クリックでボード上をクリックした場合
        if (Input.GetMouseButtonDown(0))
        {
            // カメラからクリック位置に向けてRayを発射し、ボード上のクリック位置を判定
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, boardLayer))
            {
                Vector3 impact = hitInfo.point;
                Position boardPos = SceneToBoardPos(impact);
                OnBoardClicked(boardPos);
            }
        }
    }

    // 合法手をハイライト表示するメソッド
    private void ShowLeglMoves()
    {
        foreach (Position boardPos in gameState.LegalMoves.Keys)
        {
            Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.01f;
            GameObject highlight = Instantiate(highlightPrefab, scenePos, Quaternion.identity);
            highlights.Add(highlight);
        }
    }

    // 合法手のハイライトを消去するメソッド
    private void HideLegalMoves()
    {
        highlights.ForEach(Destroy);
        highlights.Clear();
    }

    // ボード上の位置をクリックした際の処理
    private void OnBoardClicked(Position boardPos)
    {
        if (!canMove)
        {
            return;
        }

        // クリック位置にディスクを置ける場合、手を進行
        if (gameState.MakeMove(boardPos, out MoveInfo moveInfo))
        {
            StartCoroutine(OnMoveMade(moveInfo));
        }
    }

    // 指定した手を進行させるコルーチン
    private IEnumerator OnMoveMade(MoveInfo moveInfo)
    {
        canMove = false;
        HideLegalMoves();
        yield return ShowMonve(moveInfo);
        yield return ShowTurnOutcome(moveInfo);
        ShowLeglMoves();
        canMove = true;
    }

    // シーン座標をボード上の位置に変換するメソッド
    private Position SceneToBoardPos(Vector3 scenePos)
    {
        int col = (int)(scenePos.x - 0.1f);
        int row = 7 - (int)(scenePos.z - 0.1f);
        return new Position(row, col);
    }

    // ボード上の位置をシーン上の3D空間の座標に変換するメソッド
    private Vector3 BoardToScenePos(Position boardPos)
    {
        return new Vector3(boardPos.Col + 0.6f, 0, 7 - boardPos.Row + 0.6f);
    }

    // 指定位置にディスクを配置するメソッド
    private void SpawnDisc(Disc prefab, Position boardPos)
    {
        Vector3 scenPos = BoardToScenePos(boardPos) + Vector3.up * 0.01f;
        discs[boardPos.Row, boardPos.Col] = Instantiate(prefab, scenPos, Quaternion.identity);
    }

    // 初期ディスクを配置するメソッド
    private void AddStartDiscs()
    {
        foreach (Position boardPos in gameState.OccupiedPositions())
        {
            Player player = gameState.Board[boardPos.Row, boardPos.Col];
            SpawnDisc(discPrefabs[player], boardPos);
        }
    }

    // 指定位置のディスクを反転させるメソッド
    private void FlipDiscs(List<Position> positions)
    {
        foreach (Position boardPos in positions)
        {
            discs[boardPos.Row, boardPos.Col].Flip();
        }
    }

    // 指定の移動を表示するコルーチン
    private IEnumerator ShowMonve(MoveInfo moveInfo)
    {
        SpawnDisc(discPrefabs[moveInfo.Player], moveInfo.Position);
        yield return new WaitForSeconds(0.33f);
        FlipDiscs(moveInfo.Outflanked);
        yield return new WaitForSeconds(0.83f);
    }

    // ターンがスキップされた際の表示を行うコルーチン
    private IEnumerator ShowTurnSkipped(Player skippedPlayer)
    {
        uiManager.SetSkippedText(skippedPlayer);
        yield return uiManager.AnimateTopText();
    }

    // ゲーム終了時の画面表示を行うコルーチン
    private IEnumerator ShowGameOver(Player winner)
    {
        uiManager.SetTopText("Finish!!");
        yield return uiManager.AnimateTopText();

        yield return uiManager.ShowScoreText();
        yield return new WaitForSeconds(0.5f);

        yield return ShowCounting();

        uiManager.SetWinnerText(winner); // 勝者を表示
        yield return uiManager.ShowEndScreen();
    }

    // ターンの結果を表示するコルーチン
    private IEnumerator ShowTurnOutcome(MoveInfo moveInfo)
    {
        if (gameState.GameOver)
        {
            yield return ShowGameOver(gameState.Winner);
            yield break;
        }

        Player currentPlayer = gameState.CurrentPlayer;

        if (currentPlayer == moveInfo.Player)
        {
            yield return ShowTurnSkipped(currentPlayer.Opponent());
        }
        uiManager.SetPlayerText(currentPlayer);
    }

    // ディスクの枚数を数えて表示するコルーチン
    public IEnumerator ShowCounting()
    {
        int black = 0, white = 0;

        foreach (Position pos in gameState.OccupiedPositions())
        {
            Player player = gameState.Board[pos.Row, pos.Col];

            if (player == Player.Black)
            {
                black++;
                uiManager.SetBlackScoreTect(black);
            }
            else
            {
                white++;
                uiManager.SetWhiteScoreText(white);
            }

            // ディスクをTwitchアニメーションで強調表示
            discs[pos.Row, pos.Col].Twitch();
            yield return new WaitForSeconds(0.05f);
        }
    }

    // ゲームを再開始するためのコルーチン
    private IEnumerator RestartGame()
    {
        yield return uiManager.HideEndScreen();
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name); // シーンリロード
    }

    // 「もう一度プレイ」ボタンが押された際の処理
    public void OnPlayAgainClicked()
    {
        StartCoroutine(RestartGame());
    }
}
