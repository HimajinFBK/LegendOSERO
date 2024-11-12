using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Board board;
    private PieceColor currentPlayer = PieceColor.Black;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                if (cell != null && cell.GetPieceColor() == PieceColor.None)
                {
                    board.PlacePiece(cell.x, cell.y, currentPlayer);
                    SwitchPlayer();
                }
            }
        }
    }

    void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == PieceColor.Black) ? PieceColor.White : PieceColor.Black;
    }
}
