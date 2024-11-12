
using Photon.Pun.Demo.Cockpit;
using System.Collections.Generic;

public class GameStete 
{
    public const int Rows = 8;// ゲームボードの行数
    public const int Cols = 8;//列

    public Player[,] Board { get; }// ボード上の各位置のプレイヤー情報
    public Dictionary<Player,int>DiscCount{ get; }// 各プレイヤーのディスク数
    public Player CurrentPlayer { get; private set; }// 現在のプレイヤー（ターン）
    public bool GameOver { get; private set; }// ゲームが終了しているかどうか
    public Player Winner {  get; private set; }// 勝者を表す
    public Dictionary<Position,List<Position>> LegalMoves { get; private set; }// 現在のプレイヤーの合法手を保持する

    public GameStete()
    {// 8x8のボードを初期化
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

        CurrentPlayer=Player.Black; // 黒プレイヤーから
        LegalMoves = FindLegalMoves(CurrentPlayer); // 初期状態での黒プレイヤーの合法手を取得
    }


    public bool MakeMove(Position pos,out MoveInfo moveInfo)// 指定位置にディスクを置く
    {// 指定位置が合法手でない場合は配置できない
        if (!LegalMoves.ContainsKey(pos))
        {
            moveInfo = null;
            return false;
        }
        Player movePlayer = CurrentPlayer;
        List<Position> outflanked = LegalMoves[pos];
        // ディスクを置き、反転処理
        Board[pos.Row, pos.Col] = movePlayer;
        FlipDiscs(outflanked);
        // ディスク数の更新とターンの交代
        UpdeteDiscCounts(movePlayer, outflanked.Count);
        PassTurn();
        // 移動情報
        moveInfo = new MoveInfo { Player = movePlayer, Position = pos, Outflanked = outflanked };
        return true;

    }
    // ボード上の占有された位置
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
    {// 指定した位置のディスクを反転させる
        foreach (Position pos in positions) 
        {
            // 各位置のディスクを相手の色に反転
            Board[pos.Row, pos.Col] = Board[pos.Row, pos.Col].Opponent();
        }
    }

    private void UpdeteDiscCounts(Player movePlayer,int outflankedCount)
    {// ディスク数を更新
        DiscCount[movePlayer] += outflankedCount + 1;
        DiscCount[movePlayer.Opponent()]-= outflankedCount;
    }


    private void ChangePlayer()
    {// ターンを交代し、合法手を取得
        // 相手のプレイヤーにターンを交代
        CurrentPlayer = CurrentPlayer.Opponent();
        LegalMoves=FindLegalMoves(CurrentPlayer);
    }

    private Player FindWinner()
    {// ゲームの勝者を判定
        if (DiscCount[Player.Black] > DiscCount[Player.White])
        {
            return Player.Black;
        }
        if (DiscCount[Player.White] > DiscCount[Player.Black])
        {
            return Player.White;
        }

        return Player.None;// 引き分け
    }

    private void PassTurn()
    {// ターンを進行
        ChangePlayer(); // 現在のプレイヤーを交代
        if (LegalMoves.Count > 0) // 新しいプレイヤーに打てる手があればそのまま継続
        {
            return;
        }

        ChangePlayer();  // 相手プレイヤーに再度交代　打てるか

        if (LegalMoves.Count == 0)
        {
            CurrentPlayer = Player.None;
            GameOver=true;
            Winner=FindWinner();
        }
    }

    public bool IsInsideBoard(int r,int c)
    {// 指定の位置がボード内にあるか
        return r>=0&&r<Rows && c>=0 && c<Cols;
    }

    private List<Position> OutflankedInDir(Position pos,Player player,int rDelta,int cDelta)
    {// 指定の方向に挟まれたディスクを取得
        List<Position>outflanked=new List<Position>();
        int r=pos.Row+rDelta;
        int c=pos.Col+cDelta;

        while (IsInsideBoard(r, c) && Board[r, c] != Player.None)
        { // 指定方向に挟めるディスクがあるか
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

        return new List<Position>();// 挟めるディスクがない場合は空のリストを返す

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
