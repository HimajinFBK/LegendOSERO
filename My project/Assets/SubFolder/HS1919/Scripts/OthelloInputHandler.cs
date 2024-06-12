using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OthelloInputHandler : MonoBehaviour
{
    public OthelloGameManager gameManager; // ゲームマネージャーへの参照
    public Slider powerGauge; // パワーゲージの参照

    private Vector2Int cursorPosition = new Vector2Int(3, 3); // カーソルの初期位置
    private float strongPlaceThreshold = 1.0f; // 強く置くための時間閾値
    private float keyHoldTime = 0f; // キー保持時間
    private bool isHoldingKey = false; // キーが保持されているかどうか

    void Start()
    {
        // 初期状態ではパワーゲージを非表示にする
        powerGauge.gameObject.SetActive(false);
    }

    void Update()
    {
        // ユーザー入力の処理
        HandleInput();
    }

    private void HandleInput()
    {
        // 矢印キーでカーソル位置を移動
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cursorPosition.y = Mathf.Clamp(cursorPosition.y + 1, 0, 7);
            gameManager.UpdateHighlight(cursorPosition); // ハイライトの更新
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            cursorPosition.y = Mathf.Clamp(cursorPosition.y - 1, 0, 7);
            gameManager.UpdateHighlight(cursorPosition); // ハイライトの更新
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cursorPosition.x = Mathf.Clamp(cursorPosition.x - 1, 0, 7);
            gameManager.UpdateHighlight(cursorPosition); // ハイライトの更新
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cursorPosition.x = Mathf.Clamp(cursorPosition.x + 1, 0, 7);
            gameManager.UpdateHighlight(cursorPosition); // ハイライトの更新
        }

        // エンターキーまたはスペースキーが押されたときの処理
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            keyHoldTime = 0f; // キー保持時間をリセット
            isHoldingKey = true; // キー保持状態にする
            powerGauge.gameObject.SetActive(true); // パワーゲージを表示する
        }

        // キーが保持されている間の処理
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Space))
        {
            keyHoldTime += Time.deltaTime; // キー保持時間を増加
            powerGauge.value = Mathf.Clamp01(keyHoldTime / strongPlaceThreshold); // パワーゲージを更新
        }

        // キーが離されたときの処理
        if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space))
        {
            bool isStrongPlace = keyHoldTime >= strongPlaceThreshold; // 強く置くかどうかを判定
            isHoldingKey = false; // キー保持状態を解除
            powerGauge.gameObject.SetActive(false); // パワーゲージを非表示にする

            // 駒を置く処理
            if (gameManager.PlacePiece(cursorPosition.x, cursorPosition.y, gameManager.GetCurrentPlayer(), isStrongPlace))
            {
                gameManager.SwitchPlayer(); // プレイヤーを交代

                // 次のプレイヤーが駒を置けるかどうかをチェック
                if (!gameManager.CanPlacePiece(gameManager.GetCurrentPlayer()))
                {
                    Debug.Log($"Player {gameManager.GetCurrentPlayer()} has no valid moves. Passing turn.");
                    gameManager.SwitchPlayer(); // 駒を置けない場合はターンをパス
                }
            }
        }
    }
}

