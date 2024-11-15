using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disc : MonoBehaviour
{
    [SerializeField]
    private Player up; // ���݂̃f�B�X�N�̐F

    private Animator animator;
    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Flip()
    {
        if (up == Player.Black)
        {
            animator.Play("BlackToWhite");// �����甒�ւ̔��]�A�j���[�V�������Đ�
            up = Player.White; // �\�𔒂ɕύX
        }
        else
        {
            animator.Play("WhiteToBlack");//�����獕
            up = Player.Black; // �\�����ɕύX
        }
    }

    public  void Twitch()
    {
        animator.Play("TwitchDisc");//�J�E���g���̃A�j���[�V����
    }
}
