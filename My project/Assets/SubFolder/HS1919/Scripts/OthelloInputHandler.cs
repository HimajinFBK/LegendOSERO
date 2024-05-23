using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OthelloInputHandler : MonoBehaviour
{
    public OthelloGameManager gameManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 hitPoint = hit.point;
                int x = Mathf.FloorToInt(hitPoint.x);
                int y = Mathf.FloorToInt(hitPoint.z);

                // �f�o�b�O���O��ǉ����ăN���b�N�ʒu���m�F
                Debug.Log($"Clicked Position: {hitPoint}, Board Indices: x = {x}, y = {y}");

                // �{�[�h�͈̔̓`�F�b�N
                if (x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    if (gameManager.PlacePiece(x, y, gameManager.GetCurrentPlayer()))
                    {
                        gameManager.SwitchPlayer();
                    }
                }
            }
        }
    }
}
