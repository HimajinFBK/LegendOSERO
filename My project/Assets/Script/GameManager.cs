using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;// �J����
    [SerializeField]
    private LayerMask boardLayer;//�{�[�h���C���[
    [SerializeField]
    private Disc discBlackUP;
    [SerializeField]           //���Ɣ��̃f�B�X�N�̃v���n�u
    private Disc discWhiteUp;

    [SerializeField]
    private GameObject highlightPrefab;//�n�C���C�g�p�v���n�u
    [SerializeField]
    private UIManager uiManager;//UI�}�l�[�W���[���w��

    // �e�v���C���[�ɑΉ�����f�B�X�N�̃v���n�u��ێ�
    private Dictionary<Player, Disc> discPrefabs = new Dictionary<Player, Disc>();

    // �Q�[���̏�Ԃ�ێ�����GameStete�N���X�̃C���X�^���X
    private GameStete gameState = new GameStete();

    // �{�[�h��̃f�B�X�N�̏�Ԃ�ێ�����2�����z��
    private Disc[,] discs = new Disc[8, 8];

    // �v���C���[�����łĂ��Ԃ��������t���O
    private bool canMove = true;

    // ���@��̃n�C���C�g��\������GameObject�̃��X�g
    private List<GameObject> highlights = new List<GameObject>();

    // Start���\�b�h�̓X�N���v�g���L���ɂȂ����Ƃ��ɍŏ��Ɏ��s�����
    void Start()
    {
        // ���Ɣ��̃v���C���[�ɑΉ�����f�B�X�N�̃v���n�u��o�^
        discPrefabs[Player.Black] = discBlackUP;
        discPrefabs[Player.White] = discWhiteUp;

        // �����f�B�X�N�̔z�u�ƍ��@��̃n�C���C�g�\���AUI�X�V
        AddStartDiscs();
        ShowLeglMoves();
        uiManager.SetPlayerText(gameState.CurrentPlayer);
    }

    // Update���\�b�h�͖��t���[�����s�����
    private void Update()
    {
        // Escape�L�[�ŃQ�[�����I��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // ���N���b�N�Ń{�[�h����N���b�N�����ꍇ
        if (Input.GetMouseButtonDown(0))
        {
            // �J��������N���b�N�ʒu�Ɍ�����Ray�𔭎˂��A�{�[�h��̃N���b�N�ʒu�𔻒�
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, boardLayer))
            {
                Vector3 impact = hitInfo.point;
                Position boardPos = SceneToBoardPos(impact);
                OnBoardClicked(boardPos);
            }
        }
    }

    // ���@����n�C���C�g�\�����郁�\�b�h
    private void ShowLeglMoves()
    {
        foreach (Position boardPos in gameState.LegalMoves.Keys)
        {
            Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.01f;
            GameObject highlight = Instantiate(highlightPrefab, scenePos, Quaternion.identity);
            highlights.Add(highlight);
        }
    }

    // ���@��̃n�C���C�g���������郁�\�b�h
    private void HideLegalMoves()
    {
        highlights.ForEach(Destroy);
        highlights.Clear();
    }

    // �{�[�h��̈ʒu���N���b�N�����ۂ̏���
    private void OnBoardClicked(Position boardPos)
    {
        if (!canMove)
        {
            return;
        }

        // �N���b�N�ʒu�Ƀf�B�X�N��u����ꍇ�A���i�s
        if (gameState.MakeMove(boardPos, out MoveInfo moveInfo))
        {
            StartCoroutine(OnMoveMade(moveInfo));
        }
    }

    // �w�肵�����i�s������R���[�`��
    private IEnumerator OnMoveMade(MoveInfo moveInfo)
    {
        canMove = false;
        HideLegalMoves();
        yield return ShowMonve(moveInfo);
        yield return ShowTurnOutcome(moveInfo);
        ShowLeglMoves();
        canMove = true;
    }

    // �V�[�����W���{�[�h��̈ʒu�ɕϊ����郁�\�b�h
    private Position SceneToBoardPos(Vector3 scenePos)
    {
        int col = (int)(scenePos.x - 0.1f);
        int row = 7 - (int)(scenePos.z - 0.1f);
        return new Position(row, col);
    }

    // �{�[�h��̈ʒu���V�[�����3D��Ԃ̍��W�ɕϊ����郁�\�b�h
    private Vector3 BoardToScenePos(Position boardPos)
    {
        return new Vector3(boardPos.Col + 0.6f, 0, 7 - boardPos.Row + 0.6f);
    }

    // �w��ʒu�Ƀf�B�X�N��z�u���郁�\�b�h
    private void SpawnDisc(Disc prefab, Position boardPos)
    {
        Vector3 scenPos = BoardToScenePos(boardPos) + Vector3.up * 0.01f;
        discs[boardPos.Row, boardPos.Col] = Instantiate(prefab, scenPos, Quaternion.identity);
    }

    // �����f�B�X�N��z�u���郁�\�b�h
    private void AddStartDiscs()
    {
        foreach (Position boardPos in gameState.OccupiedPositions())
        {
            Player player = gameState.Board[boardPos.Row, boardPos.Col];
            SpawnDisc(discPrefabs[player], boardPos);
        }
    }

    // �w��ʒu�̃f�B�X�N�𔽓]�����郁�\�b�h
    private void FlipDiscs(List<Position> positions)
    {
        foreach (Position boardPos in positions)
        {
            discs[boardPos.Row, boardPos.Col].Flip();
        }
    }

    // �w��̈ړ���\������R���[�`��
    private IEnumerator ShowMonve(MoveInfo moveInfo)
    {
        SpawnDisc(discPrefabs[moveInfo.Player], moveInfo.Position);
        yield return new WaitForSeconds(0.33f);
        FlipDiscs(moveInfo.Outflanked);
        yield return new WaitForSeconds(0.83f);
    }

    // �^�[�����X�L�b�v���ꂽ�ۂ̕\�����s���R���[�`��
    private IEnumerator ShowTurnSkipped(Player skippedPlayer)
    {
        uiManager.SetSkippedText(skippedPlayer);
        yield return uiManager.AnimateTopText();
    }

    // �Q�[���I�����̉�ʕ\�����s���R���[�`��
    private IEnumerator ShowGameOver(Player winner)
    {
        uiManager.SetTopText("Finish!!");
        yield return uiManager.AnimateTopText();

        yield return uiManager.ShowScoreText();
        yield return new WaitForSeconds(0.5f);

        yield return ShowCounting();

        uiManager.SetWinnerText(winner); // ���҂�\��
        yield return uiManager.ShowEndScreen();
    }

    // �^�[���̌��ʂ�\������R���[�`��
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

    // �f�B�X�N�̖����𐔂��ĕ\������R���[�`��
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

            // �f�B�X�N��Twitch�A�j���[�V�����ŋ����\��
            discs[pos.Row, pos.Col].Twitch();
            yield return new WaitForSeconds(0.05f);
        }
    }

    // �Q�[�����ĊJ�n���邽�߂̃R���[�`��
    private IEnumerator RestartGame()
    {
        yield return uiManager.HideEndScreen();
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name); // �V�[�������[�h
    }

    // �u������x�v���C�v�{�^���������ꂽ�ۂ̏���
    public void OnPlayAgainClicked()
    {
        StartCoroutine(RestartGame());
    }
}
