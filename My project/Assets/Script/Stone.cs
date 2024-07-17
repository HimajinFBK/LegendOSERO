using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ゲームの石の状態と動作を管理
public class Stone : MonoBehaviour
{

    public enum Color
    {
        Black,
        White,
    }

    public enum State
    {
        None,
        Appearing,
        Reversing,
        Fix,
    }

    [SerializeField]
    private GameObject _black;
    [SerializeField]
    private GameObject _white;
    [SerializeField]
    private GameObject _dot;

    // 石の現在の色を取得または設定するプロパティ。
    // 初期値はBlackに設定
    public Color CurrentColor { get; private set; } = Color.Black;

    // 石の現在の状態を取得または設定するプロパティ。
    // 初期値はState.Noneに設定
    public State CurrentState { get; private set; } = State.None;

    // 石の回転を取得するためのプロパティ。
    // 石の色に応じて異なる回転を返す。
    private Quaternion Rotation
    {
        get
        {
            switch (CurrentColor)
            {
                case Color.Black:
                    return Quaternion.Euler(0, 0, 0);
                case Color.White:
                default:
                    return Quaternion.Euler(0, 0, 180);
            }
        }
    }

    // 石を有効または無効にするメソッド。
    // 石を有効にする場合、色と状態を設定し、対応するオブジェクトを表示する。
    public void SetActive(bool value, Color color)
    {
        if (value)
        {
            this.CurrentColor = color;
            this.CurrentState = State.Appearing;
            this._black.SetActive(true);
            this._white.SetActive(true);
            this._dot.SetActive(true);
        }
        else
        {
            this.CurrentState = State.None;
        }
        this.gameObject.SetActive(value);
    }

    // ドットを有効にするメソッド。
    // 黒と白のオブジェクトを無効にし、ドットを表示する。
    public void EnableDot()
    {
        this._black.SetActive(false);
        this._white.SetActive(false);
        this._dot.SetActive(true);
        gameObject.SetActive(true);
    }

    // 石の色を反転させるメソッド。
   
    public void Reverse()
    {
        if (CurrentState == State.None)
        {
            Debug.LogError("Invalid Stone State"); // 石の状態がState.Noneの場合、エラーログを出力する。
            return;
        }

        switch (CurrentColor)
        {
            case Color.Black:
                CurrentColor = Color.White;
                break;
            case Color.White:
                CurrentColor = Color.Black;
                break;
        }
        CurrentState = State.Reversing;
    }

    // 毎フレーム呼び出されるUpdateメソッド。
    // 石の状態に応じて回転を設定し、状態を更新する。
    public void Update()
    {
        switch (CurrentState)
        {
            case State.Appearing:
                transform.localRotation = Rotation;
                CurrentState = State.Fix;
                break;

            case State.Reversing:
                transform.localRotation = Rotation;
                CurrentState = State.Fix;
                break;

            case State.None:
            case State.Fix:
            default:
                break;
        }
    }
    private void RandomReverseAdjacentStones(int x, int z)
    {
        // 周囲の8方向の座標を定義
        var directions = new (int, int)[]
        {
            (0, 1), (1, 0), (0, -1), (-1, 0), (1, 1), (1, -1), (-1, 1), (-1, -1)
        };

        foreach (var (dx, dz) in directions)
        {
            int newX = x + dx;
            int newZ = z + dz;

            // ボードの範囲内かチェック
            if (newX >= 0 && newX < Game.XNum && newZ >= 0 && newZ < Game.ZNum)
            {
                // ランダムにひっくり返すかどうかを決定
                if (UnityEngine.Random.value > 0.5f)
                {
                    var currentColor = Game.Instance.Stones[newZ][newX].GetColor();
                    if (currentColor != Stone.Color.None)
                    {
                        var newColor = currentColor == Stone.Color.Black ? Stone.Color.White : Stone.Color.Black;
                        Game.Instance.Stones[newZ][newX].SetActive(true, newColor);
                        Game.Instance.board[newX, newZ] = newColor == Stone.Color.Black ? 1 : 2;
                    }
                }
            }
        }
    }
}
