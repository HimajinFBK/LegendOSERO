using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OthelloInputHandler : MonoBehaviour
{
    public OthelloGameManager gameManager; // �Q�[���}�l�[�W���[�ւ̎Q��
    public Slider powerGauge; // �p���[�Q�[�W�̎Q��

    private Vector2Int cursorPosition = new Vector2Int(3, 3); // �J�[�\���̏����ʒu
    private float strongPlaceThreshold = 1.0f; // �����u�����߂̎���臒l
    private float keyHoldTime = 0f; // �L�[�ێ�����
    private bool isHoldingKey = false; // �L�[���ێ�����Ă��邩�ǂ���

    void Start()
    {
        // ������Ԃł̓p���[�Q�[�W���\���ɂ���
        powerGauge.gameObject.SetActive(false);
    }

    void Update()
    {
        // ���[�U�[���͂̏���
        HandleInput();
    }

    private void HandleInput()
    {
        // ���L�[�ŃJ�[�\���ʒu���ړ�
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cursorPosition.y = Mathf.Clamp(cursorPosition.y + 1, 0, 7);
            gameManager.UpdateHighlight(cursorPosition); // �n�C���C�g�̍X�V
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            cursorPosition.y = Mathf.Clamp(cursorPosition.y - 1, 0, 7);
            gameManager.UpdateHighlight(cursorPosition); // �n�C���C�g�̍X�V
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cursorPosition.x = Mathf.Clamp(cursorPosition.x - 1, 0, 7);
            gameManager.UpdateHighlight(cursorPosition); // �n�C���C�g�̍X�V
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cursorPosition.x = Mathf.Clamp(cursorPosition.x + 1, 0, 7);
            gameManager.UpdateHighlight(cursorPosition); // �n�C���C�g�̍X�V
        }

        // �G���^�[�L�[�܂��̓X�y�[�X�L�[�������ꂽ�Ƃ��̏���
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            keyHoldTime = 0f; // �L�[�ێ����Ԃ����Z�b�g
            isHoldingKey = true; // �L�[�ێ���Ԃɂ���
            powerGauge.gameObject.SetActive(true); // �p���[�Q�[�W��\������
        }

        // �L�[���ێ�����Ă���Ԃ̏���
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Space))
        {
            keyHoldTime += Time.deltaTime; // �L�[�ێ����Ԃ𑝉�
            powerGauge.value = Mathf.Clamp01(keyHoldTime / strongPlaceThreshold); // �p���[�Q�[�W���X�V
        }

        // �L�[�������ꂽ�Ƃ��̏���
        if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space))
        {
            bool isStrongPlace = keyHoldTime >= strongPlaceThreshold; // �����u�����ǂ����𔻒�
            isHoldingKey = false; // �L�[�ێ���Ԃ�����
            powerGauge.gameObject.SetActive(false); // �p���[�Q�[�W���\���ɂ���

            // ���u������
            if (gameManager.PlacePiece(cursorPosition.x, cursorPosition.y, gameManager.GetCurrentPlayer(), isStrongPlace))
            {
                gameManager.SwitchPlayer(); // �v���C���[�����

                // ���̃v���C���[�����u���邩�ǂ������`�F�b�N
                if (!gameManager.CanPlacePiece(gameManager.GetCurrentPlayer()))
                {
                    Debug.Log($"Player {gameManager.GetCurrentPlayer()} has no valid moves. Passing turn.");
                    gameManager.SwitchPlayer(); // ���u���Ȃ��ꍇ�̓^�[�����p�X
                }
            }
        }
    }
}

