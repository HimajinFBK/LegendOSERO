using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask boardLayer;
    [SerializeField]
    private Disc discBlackUP;
    [SerializeField]
    private Disc discWhiteUp;

    [SerializeField]
    private GameObject highlightPrefab;
    [SerializeField]
    private UIManager uiManager;

    private Dictionary<Player,Disc> discPrefabs=new Dictionary<Player,Disc>();
    private GameStete gameState=new GameStete();
    private Disc[,] discs = new Disc[8,8];
    private bool canMove = true;
    private List<GameObject> highlights = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        discPrefabs[Player.Black] = discBlackUP;
        discPrefabs[Player.White] = discWhiteUp;

        AddStartDiscs();
        ShowLeglMoves();
        uiManager.SetPlayerText(gameState.CurrentPlayer);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if(Input.GetMouseButtonDown(0))
        {
            Ray ray=cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out RaycastHit hitInfo,100f, boardLayer))
            {
                Vector3 impact = hitInfo.point;
                Position boardPos = SceneToBoardPos(impact);
                OnBoardClicked(boardPos);
            }
        }
    }

    private void ShowLeglMoves()
    {
        foreach(Position boardPos in gameState.LegalMoves.Keys)
        {
            Vector3 scenePos=BoardToScenePos(boardPos)+Vector3.up*0.01f;
            GameObject highlight= Instantiate(highlightPrefab,scenePos,Quaternion.identity);
            highlights.Add(highlight);
        }
    }

    private void HideLegalMoves()
    {
        highlights.ForEach(Destroy);
        highlights.Clear();
    }


    private void OnBoardClicked(Position boardPos)
    {

        if (!canMove)
        {
            return;
        }
        if(gameState.MakeMove(boardPos,out MoveInfo moveInfo))
        {
            StartCoroutine(OnMoveMade(moveInfo));
        }
    }

    private IEnumerator OnMoveMade(MoveInfo moveInfo)
    {
        canMove = false;
        HideLegalMoves();
        yield return ShowMonve(moveInfo);
        yield return ShowTurnOutcome(moveInfo);
        ShowLeglMoves();
        canMove = true;
    }


    private Position SceneToBoardPos(Vector3 scenePos)
    {
        int col = (int)(scenePos.x - 0.1f);
        int row = 7 - (int)(scenePos.z - 0.1f);
        return new Position(row, col);
    }

    private Vector3 BoardToScenePos(Position boardPos)
    {
        return new Vector3(boardPos.Col + 0.6f, 0, 7 - boardPos.Row + 0.6f);
    }

    private void SpawnDisc(Disc prefab,Position boardPos)
    {
        Vector3 scenPos = BoardToScenePos(boardPos) + Vector3.up * 0.01f;
        discs[boardPos.Row,boardPos.Col]=Instantiate(prefab,scenPos,Quaternion.identity);
    }

    private void AddStartDiscs()
    {
        foreach(Position boardPos in gameState.OccupiedPositions())
        {
            Player player = gameState.Board[boardPos.Row,boardPos.Col];
            SpawnDisc(discPrefabs[player], boardPos);
        }
    }

    private void FlipDiscs(List<Position> positions)
    {
        foreach(Position boardPos in positions)
        {
            discs[boardPos.Row, boardPos.Col].Flip();
        }
    }

    private IEnumerator ShowMonve(MoveInfo moveInfo)
    {
        SpawnDisc(discPrefabs[moveInfo.Player], moveInfo.Position);
        yield return new WaitForSeconds(0.33f);
        FlipDiscs(moveInfo.Outflanked);
        yield return new WaitForSeconds(0.83f);
    }

    private IEnumerator ShowTurnSkipped(Player skippedPlayer)
    {
        uiManager.SetSkippedText(skippedPlayer);
        yield return uiManager.AnimateTopText();
        
    }

    private IEnumerator ShowTurnOutcome(MoveInfo moveInfo)
    {
        if (gameState.GameOver)
        {
            //
            yield break;
        }

        Player currentPlayer = gameState.CurrentPlayer;

        if (currentPlayer == moveInfo.Player)
        {
            yield return ShowTurnSkipped(currentPlayer.Opponent());
        }
        uiManager.SetPlayerText(currentPlayer);
    }
}
