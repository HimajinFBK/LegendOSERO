using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disc : MonoBehaviour
{
    [SerializeField]
    private Player up; // 現在のディスクの色

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
            animator.Play("BlackToWhite");// 黒から白への反転アニメーションを再生
            up = Player.White; // 表を白に変更
        }
        else
        {
            animator.Play("WhiteToBlack");//白から黒
            up = Player.Black; // 表を黒に変更
        }
    }

    public  void Twitch()
    {
        animator.Play("TwitchDisc");//カウント時のアニメーション
    }
}
