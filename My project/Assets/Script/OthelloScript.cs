using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Diagnostics;

public class OthelloScript : MonoBehaviour
{
    public GameObject OthellSprite;
    public GameObject Cube;

    const int FIELD_SIZE_X = 8; //const=不変
    const int FIELD_SIZE_Y = 8;

    public int Cube_SIZE_X = 2; //cube
    public int Cube_SIZE_Y = 2;

    private List<(int,int)> InfoLoist = new List<(int,int)> ();

    public enum spriteState //コマ
    {
        None,
        White,
        Black,
    }
    private spriteState PlayerTurn=spriteState.Black;


    private spriteState[,] FieldState= new spriteState[FIELD_SIZE_X,FIELD_SIZE_Y];
    private SpriteScript[,] FieldSpriteState = new SpriteScript[FIELD_SIZE_X, FIELD_SIZE_Y];
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < FIELD_SIZE_X; x++)
        {
            for (int y = 0; y < FIELD_SIZE_X; y++)
            {
                //コピーして並べる
                var sprite =Instantiate(OthellSprite, new Vector3(0.84f * x, 0.84f * y), Quaternion.Euler(90,0,0));

                FieldState[x, y] = spriteState.None;

                //コマ一つ一つをいじれるようにする
                FieldSpriteState[x, y] = sprite.GetComponent<SpriteScript>();
            }
        }//初期配置
        FieldState[3, 3] = spriteState.Black;
        FieldState[3, 4] = spriteState.White;
        FieldState[4, 3] = spriteState.White;
        FieldState[4, 4] = spriteState.Black;
    }

    // Update is called once per frame
    void Update()
    {
        #region　移動と配置
        var position = Cube.transform.localPosition;//世界座標をコピーしてそこに書き込み移動させる

        if (Input.GetKeyDown(KeyCode.RightArrow)&&Cube_SIZE_X　< 7)//右ボタンで右に移動&右に行き過ぎないようにする
        {
            Cube_SIZE_X++;           
            Cube.transform.localPosition = new Vector3(position.x  +0.84f, position.y, position.z);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && Cube_SIZE_X > 0)//左
        {
            Cube_SIZE_X--;           
            Cube.transform.localPosition = new Vector3(position.x  -0.84f, position.y, position.z);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && Cube_SIZE_Y < 7)//上
        {
            Cube_SIZE_Y++;            
            Cube.transform.localPosition = new Vector3(position.x , position. y+ 0.84f, position.z);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && Cube_SIZE_Y > 0)//下
        {
            Cube_SIZE_Y--;            
            Cube.transform.localPosition = new Vector3(position.x, position.y - 0.84f, position.z);
        }

        var turnCheck = false;
        if (Input.GetKeyDown(KeyCode.Return))//エンターで配置
        {
            for (int i = 0; i <= 7; i++)
            {

                //ひっくり返せるか判定
                if (TurnCheck(i))
                {
                    turnCheck = true;
                }
            }

            if (turnCheck)//ひっくり返す
            {
                foreach(var info in InfoLoist)//ひっくり返すコマの確認
                {
                    var positon_x = info.Item1;
                    var positon_y = info.Item2;
                    FieldState[positon_x, positon_y] = PlayerTurn;

                }


                FieldState[Cube_SIZE_X, Cube_SIZE_Y] = PlayerTurn;//コマ配置
                PlayerTurn = PlayerTurn == spriteState.Black ? spriteState.White : spriteState.Black;
                //Thread.Sleep(500);//0.5秒
                InfoLoist=new List<(int,int)>();
            }
        }
        #endregion


        for (int x=0; x < FIELD_SIZE_X; x++)
        {
            for(int y=0; y < FIELD_SIZE_X; y++)
            {
                FieldSpriteState[x, y].SetState(FieldState[x, y]);
            }
        } 
    }
   
    private bool TurnCheck(int diroction)//ひっくり返せるかどうか
    {
        var turnCheck=false;
        //座標をここでも使えるようにする
        var position_x = Cube_SIZE_X; 
        var position_y = Cube_SIZE_Y;

        var OpponetPlayerTurn=PlayerTurn==spriteState.Black?spriteState.White : spriteState.Black;//相手ターンの色

        var infoList = new List<(int, int)>();//情報を残すためのリスト

        while(0 <= position_x && 7 >= position_x && 0 <= position_y && 7 >= position_y) 
        {
            switch (diroction)
            {
                case 0://左
                    if (position_x == 0) {return turnCheck; }
                    position_x--;
                    break; 
                case 1://右
                    if (position_x == 7) { return turnCheck; }
                    position_x ++;
                    break;
                case 2://下
                    if (position_y == 0) { return turnCheck; }
                    position_y--;
                    break;
                case 3://上
                    if (position_y == 7) { return turnCheck; }
                    position_y++;
                    break;
                case 4://右上
                    if (position_x == 7) { return turnCheck; }
                    if (position_y == 7) { return turnCheck; }
                    position_x++;
                    position_y++;
                    break;
                case 5://左下
                    if (position_x == 0) { return turnCheck; }
                    if (position_y == 0) { return turnCheck; }
                    position_x--;
                    position_y--;
                    break;
                case 6://左上
                    if (position_x == 0) { return turnCheck; }
                    if (position_y == 7) { return turnCheck; }
                    position_x--;
                    position_y++;
                    break;
                case 7://右下
                    if (position_x == 7) { return turnCheck; }
                    if (position_y == 0) { return turnCheck; }
                    position_x++;
                    position_y--;
                    break;

            }
            position_x--;

            //もし左が相手のコマで　ひっくり返せたら　コマの情報を残す
            if (FieldState[position_x, position_y] == OpponetPlayerTurn)
            {
                infoList.Add((position_x, position_y));
            }

            //ループ１回目で終わる場合(自分のコマ、何もない　とき)
            if (infoList.Count == 0 && FieldState[position_x,position_y]==PlayerTurn ||
                FieldState[position_x, position_y] == spriteState.None)
            {
                turnCheck = false;
                break;
            }

            if (infoList.Count > 0 && FieldState[position_x, position_y] == PlayerTurn)
            {  //0以上で自分のコマまでの間のコマをみる
                turnCheck=true;
                foreach(var info in infoList)
                {
                    InfoLoist.Add(info);
                }
                break;
            }


        }
        return turnCheck;
    }
}
