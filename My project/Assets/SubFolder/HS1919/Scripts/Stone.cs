using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public enum PieceColor { Black, White };
    public PieceColor color;

    public void Flip()
    {
        // �΂𔽓]������A�j���[�V�����Ȃǂ�����
        // �Ⴆ�΁A�F��ς���Ȃǂ̏������s��
        if (color == PieceColor.Black)
        {
            color = PieceColor.White;
            GetComponent<Renderer>().material.color = Color.white;
        }
        else
        {
            color = PieceColor.Black;
            GetComponent<Renderer>().material.color = Color.black;
        }
    }
}
