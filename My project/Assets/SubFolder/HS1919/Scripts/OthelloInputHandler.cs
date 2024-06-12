using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OthelloInputHandler : MonoBehaviour
{
    public OthelloGameManager gameManager;
    public Slider powerGauge; // �Q�[�W�̎Q�Ƃ�ǉ�

    private Vector2Int cursorPosition = new Vector2Int(3, 3); // �J�[�\���̏����ʒu
    private float strongPlaceThreshold = 1.0f; // �����u�����Ԃ�臒l
    private float keyHoldTime = 0f;
    private bool isHoldingKey = false;

    void Start()
    {
        powerGauge.gameObject.SetActive(false); // ������Ԃł̓Q�[�W���\���ɂ���
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cursorPosition.y = Mathf.Clamp(cursorPosition.y + 1, 0, 7);
            gameManager.UpdateHighlight(cursorPosition);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            cursorPosition.y = Mathf.Clamp(cursorPosition.y - 1, 0, 7);
            gameManager.UpdateHighlight(cursorPosition);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cursorPosition.x = Mathf.Clamp(cursorPosition.x - 1, 0, 7);
            gameManager.UpdateHighlight(cursorPosition);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cursorPosition.x = Mathf.Clamp(cursorPosition.x + 1, 0, 7);
            gameManager.UpdateHighlight(cursorPosition);
        }

        // �L�[�������ꂽ��^�C�}�[�����Z�b�g���ĕێ���Ԃ��J�n����
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            keyHoldTime = 0f;
            isHoldingKey = true;
            powerGauge.gameObject.SetActive(true); // �Q�[�W��\������
        }

        // �L�[���ێ�����Ă���ԃ^�C�}�[�𑝉�������
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Space))
        {
            keyHoldTime += Time.deltaTime;
            powerGauge.value = Mathf.Clamp01(keyHoldTime / strongPlaceThreshold); // �Q�[�W���X�V����
        }

        // �L�[�������ꂽ�Ƃ��̏���
        if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space))
        {
            bool isStrongPlace = keyHoldTime >= strongPlaceThreshold;
            isHoldingKey = false;
            powerGauge.gameObject.SetActive(false); // �Q�[�W���\���ɂ���

            if (gameManager.PlacePiece(cursorPosition.x, cursorPosition.y, gameManager.GetCurrentPlayer(), isStrongPlace))
            {
                gameManager.SwitchPlayer();

                if (!gameManager.CanPlacePiece(gameManager.GetCurrentPlayer()))
                {
                    Debug.Log($"Player {gameManager.GetCurrentPlayer()} has no valid moves. Passing turn.");
                    gameManager.SwitchPlayer();
                }
            }
        }
    }
}
