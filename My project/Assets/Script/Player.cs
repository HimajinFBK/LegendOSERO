
public enum Player 
{
   None, Black,White
}

public static class PlayerExtensions
{
    public static Player Opponent(this Player player)
    {
        if (player == Player.Black) // �v���C���[�����Ȃ�A����͔�
        {
            return Player.White;
        }
        else if(player == Player.White)// �v���C���[�����Ȃ�A����͍�
        {
            return Player.Black;
        }

        return Player.None;// �ǂ���ł��Ȃ��ꍇ
    }
}
