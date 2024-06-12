using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�Q�[���̐΂̏�ԂƓ�����Ǘ�
public class Stone : MonoBehaviour
{

    public enum Color
    {
        Black,
        White,
    }

    public enum State
    {
        None,
        Appearing,
        Reversing,
        Fix,
    }

    [SerializeField]
    private GameObject _black;
    [SerializeField]
    private GameObject _white;
    [SerializeField]
    private GameObject _dot;

    // �΂̌��݂̐F���擾�܂��͐ݒ肷��v���p�e�B�B
    // �����l��Black�ɐݒ�
    public Color CurrentColor { get; private set; } = Color.Black;

    // �΂̌��݂̏�Ԃ��擾�܂��͐ݒ肷��v���p�e�B�B
    // �����l��State.None�ɐݒ�
    public State CurrentState { get; private set; } = State.None;

    // �΂̉�]���擾���邽�߂̃v���p�e�B�B
    // �΂̐F�ɉ����ĈقȂ��]��Ԃ��B
    private Quaternion Rotation
    {
        get
        {
            switch (CurrentColor)
            {
                case Color.Black:
                    return Quaternion.Euler(0, 0, 0);
                case Color.White:
                default:
                    return Quaternion.Euler(0, 0, 180);
            }
        }
    }

    // �΂�L���܂��͖����ɂ��郁�\�b�h�B
    // �΂�L���ɂ���ꍇ�A�F�Ə�Ԃ�ݒ肵�A�Ή�����I�u�W�F�N�g��\������B
    public void SetActive(bool value, Color color)
    {
        if (value)
        {
            this.CurrentColor = color;
            this.CurrentState = State.Appearing;
            this._black.SetActive(true);
            this._white.SetActive(true);
            this._dot.SetActive(true);
        }
        else
        {
            this.CurrentState = State.None;
        }
        this.gameObject.SetActive(value);
    }

    // �h�b�g��L���ɂ��郁�\�b�h�B
    // ���Ɣ��̃I�u�W�F�N�g�𖳌��ɂ��A�h�b�g��\������B
    public void EnableDot()
    {
        this._black.SetActive(false);
        this._white.SetActive(false);
        this._dot.SetActive(true);
        gameObject.SetActive(true);
    }

    // �΂̐F�𔽓]�����郁�\�b�h�B
   
    public void Reverse()
    {
        if (CurrentState == State.None)
        {
            Debug.LogError("Invalid Stone State"); // �΂̏�Ԃ�State.None�̏ꍇ�A�G���[���O���o�͂���B
            return;
        }

        switch (CurrentColor)
        {
            case Color.Black:
                CurrentColor = Color.White;
                break;
            case Color.White:
                CurrentColor = Color.Black;
                break;
        }
        CurrentState = State.Reversing;
    }

    // ���t���[���Ăяo�����Update���\�b�h�B
    // �΂̏�Ԃɉ����ĉ�]��ݒ肵�A��Ԃ��X�V����B
    public void Update()
    {
        switch (CurrentState)
        {
            case State.Appearing:
                transform.localRotation = Rotation;
                CurrentState = State.Fix;
                break;

            case State.Reversing:
                transform.localRotation = Rotation;
                CurrentState = State.Fix;
                break;

            case State.None:
            case State.Fix:
            default:
                break;
        }
    }

}
