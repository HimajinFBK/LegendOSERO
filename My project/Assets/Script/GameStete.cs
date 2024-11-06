
using Photon.Pun.Demo.Cockpit;
using System.Collections.Generic;

public class GameStete 
{
    public const int Rows = 8;
    public const int Cols = 8;

    public Player[,] Board { get; }
    public Dictionary<Player,int>DiscCount{ get; }
    public Player CurrentPlayer { get; private set; }
    public bool GameOver { get; private set; }
    public Player Winner {  get; private set; }
    public Dictionary<Position,List<Position>> LegalMoves { get; private set; }

    public GameStete()
    {
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

        CurrentPlayer=Player.Black;

    }

    public bool IsInsideBoard(int r,int c)
    {
        return r>=0&&r<Rows && c>=0 && c<Cols;
    }

    private List<Position> OutflankedInDir(Position pos,Player player,int rDelta,int cDelta)
    {
        List<Position>outflanked=new List<Position>();
        int r=pos.Row+rDelta;
        int c=pos.Col+cDelta;

        while (IsInsideBoard(r, c) && Board[r, c] != Player.None)
        {
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

        return new List<Position>();

    }


}
