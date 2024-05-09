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

    const int FIELD_SIZE_X = 8; //const=�s��
    const int FIELD_SIZE_Y = 8;

    public int Cube_SIZE_X = 2; //cube
    public int Cube_SIZE_Y = 2;

    private List<(int,int)> InfoLoist = new List<(int,int)> ();

    public enum spriteState //�R�}
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
                //�R�s�[���ĕ��ׂ�
                var sprite =Instantiate(OthellSprite, new Vector3(0.84f * x, 0.84f * y), Quaternion.Euler(90,0,0));

                FieldState[x, y] = spriteState.None;

                //�R�}�����������悤�ɂ���
                FieldSpriteState[x, y] = sprite.GetComponent<SpriteScript>();
            }
        }//�����z�u
        FieldState[3, 3] = spriteState.Black;
        FieldState[3, 4] = spriteState.White;
        FieldState[4, 3] = spriteState.White;
        FieldState[4, 4] = spriteState.Black;
    }

    // Update is called once per frame
    void Update()
    {
        #region�@�ړ��Ɣz�u
        var position = Cube.transform.localPosition;//���E���W���R�s�[���Ă����ɏ������݈ړ�������

        if (Input.GetKeyDown(KeyCode.RightArrow)&&Cube_SIZE_X�@< 7)//�E�{�^���ŉE�Ɉړ�&�E�ɍs���߂��Ȃ��悤�ɂ���
        {
            Cube_SIZE_X++;           
            Cube.transform.localPosition = new Vector3(position.x  +0.84f, position.y, position.z);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && Cube_SIZE_X > 0)//��
        {
            Cube_SIZE_X--;           
            Cube.transform.localPosition = new Vector3(position.x  -0.84f, position.y, position.z);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && Cube_SIZE_Y < 7)//��
        {
            Cube_SIZE_Y++;            
            Cube.transform.localPosition = new Vector3(position.x , position. y+ 0.84f, position.z);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && Cube_SIZE_Y > 0)//��
        {
            Cube_SIZE_Y--;            
            Cube.transform.localPosition = new Vector3(position.x, position.y - 0.84f, position.z);
        }

        var turnCheck = false;
        if (Input.GetKeyDown(KeyCode.Return))//�G���^�[�Ŕz�u
        {
            for (int i = 0; i <= 7; i++)
            {

                //�Ђ�����Ԃ��邩����
                if (TurnCheck(i))
                {
                    turnCheck = true;
                }
            }

            if (turnCheck)//�Ђ�����Ԃ�
            {
                foreach(var info in InfoLoist)//�Ђ�����Ԃ��R�}�̊m�F
                {
                    var positon_x = info.Item1;
                    var positon_y = info.Item2;
                    FieldState[positon_x, positon_y] = PlayerTurn;

                }


                FieldState[Cube_SIZE_X, Cube_SIZE_Y] = PlayerTurn;//�R�}�z�u
                PlayerTurn = PlayerTurn == spriteState.Black ? spriteState.White : spriteState.Black;
                //Thread.Sleep(500);//0.5�b
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
   
    private bool TurnCheck(int diroction)//�Ђ�����Ԃ��邩�ǂ���
    {
        var turnCheck=false;
        //���W�������ł��g����悤�ɂ���
        var position_x = Cube_SIZE_X; 
        var position_y = Cube_SIZE_Y;

        var OpponetPlayerTurn=PlayerTurn==spriteState.Black?spriteState.White : spriteState.Black;//����^�[���̐F

        var infoList = new List<(int, int)>();//�����c�����߂̃��X�g

        while(0 <= position_x && 7 >= position_x && 0 <= position_y && 7 >= position_y) 
        {
            switch (diroction)
            {
                case 0://��
                    if (position_x == 0) {return turnCheck; }
                    position_x--;
                    break; 
                case 1://�E
                    if (position_x == 7) { return turnCheck; }
                    position_x ++;
                    break;
                case 2://��
                    if (position_y == 0) { return turnCheck; }
                    position_y--;
                    break;
                case 3://��
                    if (position_y == 7) { return turnCheck; }
                    position_y++;
                    break;
                case 4://�E��
                    if (position_x == 7) { return turnCheck; }
                    if (position_y == 7) { return turnCheck; }
                    position_x++;
                    position_y++;
                    break;
                case 5://����
                    if (position_x == 0) { return turnCheck; }
                    if (position_y == 0) { return turnCheck; }
                    position_x--;
                    position_y--;
                    break;
                case 6://����
                    if (position_x == 0) { return turnCheck; }
                    if (position_y == 7) { return turnCheck; }
                    position_x--;
                    position_y++;
                    break;
                case 7://�E��
                    if (position_x == 7) { return turnCheck; }
                    if (position_y == 0) { return turnCheck; }
                    position_x++;
                    position_y--;
                    break;

            }
            position_x--;

            //������������̃R�}�Ł@�Ђ�����Ԃ�����@�R�}�̏����c��
            if (FieldState[position_x, position_y] == OpponetPlayerTurn)
            {
                infoList.Add((position_x, position_y));
            }

            //���[�v�P��ڂŏI���ꍇ(�����̃R�}�A�����Ȃ��@�Ƃ�)
            if (infoList.Count == 0 && FieldState[position_x,position_y]==PlayerTurn ||
                FieldState[position_x, position_y] == spriteState.None)
            {
                turnCheck = false;
                break;
            }

            if (infoList.Count > 0 && FieldState[position_x, position_y] == PlayerTurn)
            {  //0�ȏ�Ŏ����̃R�}�܂ł̊Ԃ̃R�}���݂�
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
