
using Photon.Pun.Demo.Cockpit;
using System.Collections.Generic;

public class GameStete 
{
    public const int Rows = 8;// �Q�[���{�[�h�̍s��
    public const int Cols = 8;//��

    public Player[,] Board { get; }// �{�[�h��̊e�ʒu�̃v���C���[���
    public Dictionary<Player,int>DiscCount{ get; }// �e�v���C���[�̃f�B�X�N��
    public Player CurrentPlayer { get; private set; }// ���݂̃v���C���[�i�^�[���j
    public bool GameOver { get; private set; }// �Q�[�����I�����Ă��邩�ǂ���
    public Player Winner {  get; private set; }// ���҂�\��
    public Dictionary<Position,List<Position>> LegalMoves { get; private set; }// ���݂̃v���C���[�̍��@���ێ�����

    public GameStete()
    {// 8x8�̃{�[�h��������
        Board = new Player[Rows, Cols];
        Board[3, 3] = Player.White;
        Board[3, 4] = Player.Black;
        Board[4, 3] = Player.Black;
        Board[4, 4] = Player.White;

        DiscCount = new Dictionary<Player, int>()
        {
            { Player.Black, 2 },
            { Player.White, 2 },
        };

        CurrentPlayer=Player.Black; // ���v���C���[����
        LegalMoves = FindLegalMoves(CurrentPlayer); // ������Ԃł̍��v���C���[�̍��@����擾
    }


    public bool MakeMove(Position pos,out MoveInfo moveInfo)// �w��ʒu�Ƀf�B�X�N��u��
    {// �w��ʒu�����@��łȂ��ꍇ�͔z�u�ł��Ȃ�
        if (!LegalMoves.ContainsKey(pos))
        {
            moveInfo = null;
            return false;
        }
        Player movePlayer = CurrentPlayer;
        List<Position> outflanked = LegalMoves[pos];
        // �f�B�X�N��u���A���]����
        Board[pos.Row, pos.Col] = movePlayer;
        FlipDiscs(outflanked);
        // �f�B�X�N���̍X�V�ƃ^�[���̌��
        UpdeteDiscCounts(movePlayer, outflanked.Count);
        PassTurn();
        // �ړ����
        moveInfo = new MoveInfo { Player = movePlayer, Position = pos, Outflanked = outflanked };
        return true;

    }
    // �{�[�h��̐�L���ꂽ�ʒu
    public IEnumerable<Position> OccupiedPositions()
    {
        for(int r=0;r<Rows;r++)
        {
            for(int c = 0; c < Cols; c++)
            {
                if (Board[r, c] != Player.None)
                {
                    yield return new Position(r,c);
                }
            }
        }
    }

    private void FlipDiscs(List<Position> positions)
    {// �w�肵���ʒu�̃f�B�X�N�𔽓]������
        foreach (Position pos in positions) 
        {
            // �e�ʒu�̃f�B�X�N�𑊎�̐F�ɔ��]
            Board[pos.Row, pos.Col] = Board[pos.Row, pos.Col].Opponent();
        }
    }

    private void UpdeteDiscCounts(Player movePlayer,int outflankedCount)
    {// �f�B�X�N�����X�V
        DiscCount[movePlayer] += outflankedCount + 1;
        DiscCount[movePlayer.Opponent()]-= outflankedCount;
    }


    private void ChangePlayer()
    {// �^�[������サ�A���@����擾
        // ����̃v���C���[�Ƀ^�[�������
        CurrentPlayer = CurrentPlayer.Opponent();
        LegalMoves=FindLegalMoves(CurrentPlayer);
    }

    private Player FindWinner()
    {// �Q�[���̏��҂𔻒�
        if (DiscCount[Player.Black] > DiscCount[Player.White])
        {
            return Player.Black;
        }
        if (DiscCount[Player.White] > DiscCount[Player.Black])
        {
            return Player.White;
        }

        return Player.None;// ��������
    }

    private void PassTurn()
    {// �^�[����i�s
        ChangePlayer(); // ���݂̃v���C���[�����
        if (LegalMoves.Count > 0) // �V�����v���C���[�ɑłĂ�肪����΂��̂܂܌p��
        {
            return;
        }

        ChangePlayer();  // ����v���C���[�ɍēx���@�łĂ邩

        if (LegalMoves.Count == 0)
        {
            CurrentPlayer = Player.None;
            GameOver=true;
            Winner=FindWinner();
        }
    }

    public bool IsInsideBoard(int r,int c)
    {// �w��̈ʒu���{�[�h���ɂ��邩
        return r>=0&&r<Rows && c>=0 && c<Cols;
    }

    private List<Position> OutflankedInDir(Position pos,Player player,int rDelta,int cDelta)
    {// �w��̕����ɋ��܂ꂽ�f�B�X�N���擾
        List<Position>outflanked=new List<Position>();
        int r=pos.Row+rDelta;
        int c=pos.Col+cDelta;

        while (IsInsideBoard(r, c) && Board[r, c] != Player.None)
        { // �w������ɋ��߂�f�B�X�N�����邩
            if (Board[r, c] == player.Opponent())
            {
                outflanked.Add(new Position(r, c));
                r += rDelta;
                c += cDelta;
            }
            else if (Board[r, c] == player)
            {
                return outflanked;
            }
        }

        return new List<Position>();// ���߂�f�B�X�N���Ȃ��ꍇ�͋�̃��X�g��Ԃ�

    }

    private List<Position>Outflanked(Position pos,Player player)
    {
        List<Position>outflanked = new List<Position>();

        for(int rDelta = -1; rDelta <= 1; rDelta++)
        {
            for(int cDelte = -1; cDelte <= 1; cDelte++)
            {
                if (rDelta == 0 && cDelte == 0)
                {
                    continue;
                }

                outflanked.AddRange(OutflankedInDir(pos, player, rDelta, cDelte));

            }
        }
        return outflanked;
    }

    private bool IsMoveLegal(Player player, Position pos,out List<Position> outflanked)
    {
        if (Board[pos.Row, pos.Col] != Player.None)
        {
            outflanked = null;
            return false;
        }
        outflanked = Outflanked(pos, player);
        return outflanked.Count > 0;
    }


    private Dictionary<Position,List<Position>>FindLegalMoves(Player player)
    {
        Dictionary<Position,List<Position>> legalMoves = new Dictionary<Position,List<Position>>();

        for(int r=0; r<Rows; r++)
        {
            for(int c=0; c < Cols; c++)
            {
                Position pos=new Position(r,c);

                if(IsMoveLegal(player,pos, out List<Position> outflanked))
                {
                    legalMoves[pos]= outflanked;
                }
            }
        }

        return legalMoves;

    }


}
