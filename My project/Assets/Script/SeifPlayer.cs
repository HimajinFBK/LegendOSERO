using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//プレイヤーのターン処理やカーソル操作
public class SeifPlayer : Player
{// プレイヤーの色を白に設定
    public override Stone.Color MyColor { get { return Stone.Color.Black; } }


    private int _processingPlayerTurn = 0;// プレイヤーのターン処理を追跡するための変数
    private Vector3Int _cursorPos = Vector3Int.zero;// カーソルの位置を保持する変数
    private Vector3Int? _desidedPos = null;// 決定された位置を保持する変数

    private float strongPlaceThreshold = 1.0f; // 強く置くための時間閾値
    private float keyHoldTime = 0f; // キー保持時間
    private bool isHoldingKey = false; // キーが保持されているかどうか



    // 石を置く位置が決定されているか確認するメソッド
    public override bool TryGetSelected(out int x, out int z)
    {
        if (_desidedPos.HasValue)
        {
            var pos = _desidedPos.Value;
            if (Game.Instance.Stones[pos.z][pos.x].CurrentState == Stone.State.None) // 追加
            {
                x = pos.x;
                z = pos.z;
                return true;
            }
        }
        x = 0;
        z = 0;
        return false;
    }



    // Update is called once per frame
    void Update()
    { // ゲームの現在の状態が黒のターンの場合、ターンを実行
        switch (Game.Instance.CurrentState)
        {
            case Game.State.BlackTurn:
                ExecTurn();
                break;

                default: 
                break;
        }
    }
    // プレイヤーのターンを実行する
    private void ExecTurn()//コマを置ける場所を表示
    {
        var currentTurn=Game.Instance.CurrentTurn;
        if(_processingPlayerTurn!=currentTurn)
        {
            // ターンが進んだ場合、ドットを表示しカーソルを有効にする
            ShowDots();
            Game.Instance.Cursor.SetActive(true);
            _desidedPos = null;
            _processingPlayerTurn = currentTurn;

        }

        var keyboard = Keyboard.current;//カーソルの移動
        if(keyboard.leftArrowKey.wasPressedThisFrame||keyboard.aKey.wasPressedThisFrame) 
        {
            TryCursorMove(-1, 0);//左
        }
        else if(keyboard.upArrowKey.wasPressedThisFrame|| keyboard.wKey.wasPressedThisFrame)
        {
            TryCursorMove(0, 1);//上
        }
        else if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame)
        {
            TryCursorMove(1, 0);//右
        }
        else if (keyboard.downArrowKey.wasPressedThisFrame || keyboard.sKey.wasPressedThisFrame)
        {
            TryCursorMove(0, -1);//下
        }
        else if (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
        {//エンターキーを押してコマ配置

            keyHoldTime = 0f; // キー保持時間をリセット
            isHoldingKey = true; // キー保持状態にする

            if (Game.Instance.CalcTotalReverseCount(MyColor, _cursorPos.x, _cursorPos.z) > 0)
            {


                //石を置こうとしている位置に既に石があるかどうかをチェックする
                if (Game.Instance.Stones[_cursorPos.z][_cursorPos.x].CurrentState == Stone.State.None)
                {
                    _desidedPos = _cursorPos;
                    Game.Instance.Cursor.SetActive(false);
                    HideDots();
                }
            }
        }
    }
    // カーソルを移動
    private bool TryCursorMove(int deltaX, int deltaZ)
    {
        var x = _cursorPos.x;
        var z = _cursorPos.z;
        x += deltaX;
        z += deltaZ;

        // カーソルの位置が範囲内か確認
        if (x < 0||Game.XNum<=x)
            return false;
        if(z<0||Game.ZNum<=z)
            return false;

        _cursorPos.x= x;
        _cursorPos.z= z;
        Game.Instance.Cursor.transform.localPosition = _cursorPos *10;
        return true;
    }
    // 石を置ける場所にドットを表示するメソッド
    private void ShowDots()
    {
        var availablePoints=CalcAvailablePoints();
        var stones = Game.Instance.Stones;

        // 利用可能なポイントにドットを表示
        foreach ( var availablePoint in availablePoints.Keys)
        {
            var x = availablePoint.Item1;
            var z = availablePoint.Item2;
            stones[z][x].EnableDot();
        }
    }
    // ドットを非表示にするメソッド
    private void HideDots()
    {
        // すべての石を非表示にする
        var stones = Game.Instance.Stones;
        for(var z = 0; z < Game.ZNum; z++)
        {
            for(var x = 0; x < Game.XNum; x++)
            {
                if (stones[z][x].CurrentState == Stone.State.None)
                {
                    stones[z][x].SetActive(false,Stone.Color.Black);
                }
            }
        }
    }
}
 