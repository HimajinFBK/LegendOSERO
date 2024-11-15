
public enum Player 
{
   None, Black,White
}

public static class PlayerExtensions
{
    public static Player Opponent(this Player player)
    {
        if (player == Player.Black) // プレイヤーが黒なら、相手は白
        {
            return Player.White;
        }
        else if(player == Player.White)// プレイヤーが白なら、相手は黒
        {
            return Player.Black;
        }

        return Player.None;// どちらでもない場合
    }
}
