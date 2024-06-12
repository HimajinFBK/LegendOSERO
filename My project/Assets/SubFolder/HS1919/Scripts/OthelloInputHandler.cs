using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OthelloInputHandler : MonoBehaviour
{
    public OthelloGameManager gameManager;
    public Slider powerGauge; // ゲージの参照を追加

    private Vector2Int cursorPosition = new Vector2Int(3, 3); // カーソルの初期位置
    private float strongPlaceThreshold = 1.0f; // 強く置く時間の閾値
    private float keyHoldTime = 0f;
    private bool isHoldingKey = false;

    void Start()
    {
        powerGauge.gameObject.SetActive(false); // 初期状態ではゲージを非表示にする
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

        // キーが押されたらタイマーをリセットして保持状態を開始する
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            keyHoldTime = 0f;
            isHoldingKey = true;
            powerGauge.gameObject.SetActive(true); // ゲージを表示する
        }

        // キーが保持されている間タイマーを増加させる
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Space))
        {
            keyHoldTime += Time.deltaTime;
            powerGauge.value = Mathf.Clamp01(keyHoldTime / strongPlaceThreshold); // ゲージを更新する
        }

        // キーが離されたときの処理
        if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space))
        {
            bool isStrongPlace = keyHoldTime >= strongPlaceThreshold;
            isHoldingKey = false;
            powerGauge.gameObject.SetActive(false); // ゲージを非表示にする

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
