using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//�v���C���[�̃^�[��������J�[�\������
public class SeifPlayer : Player
{// �v���C���[�̐F�𔒂ɐݒ�
    public override Stone.Color MyColor { get { return Stone.Color.Black; } }


    private int _processingPlayerTurn = 0;// �v���C���[�̃^�[��������ǐՂ��邽�߂̕ϐ�
    private Vector3Int _cursorPos = Vector3Int.zero;// �J�[�\���̈ʒu��ێ�����ϐ�
    private Vector3Int? _desidedPos = null;// ���肳�ꂽ�ʒu��ێ�����ϐ�

    private float pressStartTime;
    private const float longPressThreshold = 1.0f; // �������Ƃ݂Ȃ�臒l�i�b�j

    public bool IsLongPress()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Space))
        {
            if (pressStartTime == 0f)
            {
                pressStartTime = Time.time; // �L�[��������n�߂����Ԃ��L�^
            }
            else if (Time.time - pressStartTime > longPressThreshold)
            {
                return true; // �������Ɣ���
            }
        }
        else
        {
            pressStartTime = 0f; // �L�[�������ꂽ�玞�Ԃ����Z�b�g
        }

        return false; // �������łȂ�
    }



    // �΂�u���ʒu�����肳��Ă��邩�m�F���郁�\�b�h
    public override bool TryGetSelected(out int x, out int z)
    {
        if (_desidedPos.HasValue)
        {
            var pos = _desidedPos.Value;
            if (Game.Instance.Stones[pos.z][pos.x].CurrentState == Stone.State.None) // �ǉ�
            {
                x = pos.x;
                z = pos.z;
                return true;
            }
        }
        x = 0;
        z = 0;
        return false;
    }



    // Update is called once per frame
    void Update()
    { // �Q�[���̌��݂̏�Ԃ����̃^�[���̏ꍇ�A�^�[�������s
        switch (Game.Instance.CurrentState)
        {
            case Game.State.BlackTurn:
                ExecTurn();
                break;

                default: 
                break;
        }
    }
    // �v���C���[�̃^�[�������s����
    private void ExecTurn()//�R�}��u����ꏊ��\��
    {
        var currentTurn=Game.Instance.CurrentTurn;
        if(_processingPlayerTurn!=currentTurn)
        {
            // �^�[�����i�񂾏ꍇ�A�h�b�g��\�����J�[�\����L���ɂ���
            ShowDots();
            Game.Instance.Cursor.SetActive(true);
            _desidedPos = null;
            _processingPlayerTurn = currentTurn;

        }

        var keyboard = Keyboard.current;//�J�[�\���̈ړ�
        if(keyboard.leftArrowKey.wasPressedThisFrame||keyboard.aKey.wasPressedThisFrame) 
        {
            TryCursorMove(-1, 0);//��
        }
        else if(keyboard.upArrowKey.wasPressedThisFrame|| keyboard.wKey.wasPressedThisFrame)
        {
            TryCursorMove(0, 1);//��
        }
        else if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame)
        {
            TryCursorMove(1, 0);//�E
        }
        else if (keyboard.downArrowKey.wasPressedThisFrame || keyboard.sKey.wasPressedThisFrame)
        {
            TryCursorMove(0, -1);//��
        }
        else if (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
        {//�G���^�[�L�[�������ăR�}�z�u

           

            if (Game.Instance.CalcTotalReverseCount(MyColor, _cursorPos.x, _cursorPos.z) > 0)
            {


                //�΂�u�����Ƃ��Ă���ʒu�Ɋ��ɐ΂����邩�ǂ������`�F�b�N����
                if (Game.Instance.Stones[_cursorPos.z][_cursorPos.x].CurrentState == Stone.State.None)
                {
                    _desidedPos = _cursorPos;
                    Game.Instance.Cursor.SetActive(false);
                    HideDots();
                }
                // �����������o���ꂽ�ꍇ�̒ǉ�����
                if (IsLongPress())
                {
                  //  RandomReverseAdjacentStones(_cursorPos.x, _cursorPos.z);
                }
            }
        }
    }
    // �J�[�\�����ړ�
    private bool TryCursorMove(int deltaX, int deltaZ)
    {
        var x = _cursorPos.x;
        var z = _cursorPos.z;
        x += deltaX;
        z += deltaZ;

        // �J�[�\���̈ʒu���͈͓����m�F
        if (x < 0||Game.XNum<=x)
            return false;
        if(z<0||Game.ZNum<=z)
            return false;

        _cursorPos.x= x;
        _cursorPos.z= z;
        Game.Instance.Cursor.transform.localPosition = _cursorPos *10;
        return true;
    }
    // �΂�u����ꏊ�Ƀh�b�g��\�����郁�\�b�h
    private void ShowDots()
    {
        var availablePoints=CalcAvailablePoints();
        var stones = Game.Instance.Stones;

        // ���p�\�ȃ|�C���g�Ƀh�b�g��\��
        foreach ( var availablePoint in availablePoints.Keys)
        {
            var x = availablePoint.Item1;
            var z = availablePoint.Item2;
            stones[z][x].EnableDot();
        }
    }
    // �h�b�g���\���ɂ��郁�\�b�h
    private void HideDots()
    {
        // ���ׂĂ̐΂��\���ɂ���
        var stones = Game.Instance.Stones;
        for(var z = 0; z < Game.ZNum; z++)
        {
            for(var x = 0; x < Game.XNum; x++)
            {
                if (stones[z][x].CurrentState == Stone.State.None)
                {
                    stones[z][x].SetActive(false,Stone.Color.Black);
                }
            }
        }
    }


}
 