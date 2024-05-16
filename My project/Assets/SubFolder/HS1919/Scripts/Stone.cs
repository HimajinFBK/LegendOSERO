using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public enum PieceColor { Black, White };
    public PieceColor color;

    public void Flip()
    {
        // 石を反転させるアニメーションなどを実装
        // 例えば、色を変えるなどの処理を行う
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
