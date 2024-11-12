using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceColor { None, Black, White }

public class Cell : MonoBehaviour
{
    public GameObject blackPiecePrefab;
    public GameObject whitePiecePrefab;
    private PieceColor currentColor = PieceColor.None;

    public int x { get; private set; }
    public int y { get; private set; }

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetPiece(PieceColor color)
    {
       if (currentColor == PieceColor.None)
        { /*
            GameObject piece = null;
            if (color == PieceColor.Black)
            {
                piece = Instantiate(blackPiecePrefab, transform.position, Quaternion.identity);
            }
            else if (color == PieceColor.White)
            {
                piece = Instantiate(whitePiecePrefab, transform.position, Quaternion.identity);
            }
            if (piece != null)
            {
                piece.transform.SetParent(transform);
                currentColor = color;
            }
        */}
        else if (currentColor != color)
        {
            // ãÓÇó†ï‘Ç∑èàóù
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            GameObject piece = null;
            if (color == PieceColor.Black)
            {
                piece = Instantiate(blackPiecePrefab, transform.position, Quaternion.identity);
            }
            else if (color == PieceColor.White)
            {
                piece = Instantiate(whitePiecePrefab, transform.position, Quaternion.identity);
            }
            if (piece != null)
            {
                piece.transform.SetParent(transform);
                currentColor = color;
            }
        }
    }

    public PieceColor GetPieceColor()
    {
        return currentColor;
    }
}
