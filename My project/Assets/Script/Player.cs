using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ゲームのプレイヤーまたはAIが利用できるポイントを計算
public abstract class Player : MonoBehaviour
{// StoneクラスのColorプロパティを抽象プロパティとして定義
    public abstract Stone.Color MyColor { get; }

    // 石が選択されているかどうかをチェックする仮想メソッド。
    // 初期状態では、選択されていないことを示すために常にfalseを返す。
    public virtual bool TryGetSelected(out int x, out int z)
    {
        x = 0;
        z = 0;
        return false;
    }

    // プレイヤーが石を置くことができる位置と、その位置で得られるポイント数を計算するメソッド。
    // 利用可能なポイントを格納するDictionaryを返す。
    public Dictionary<Tuple<int, int>, int> CalcAvailablePoints()
    {
        // ゲームのインスタンスを取得
        var game = Game.Instance;

        // 現在の石の配置を取得
        var stones = game.Stones;

        // 利用可能なポイントを格納するDictionaryを初期化
        var availablePoints = new Dictionary<Tuple<int, int>, int>();

        // ゲーム盤の全ての座標を走査
        for (var z = 0; z < Game.ZNum; z++)
        {
            for (var x = 0; x < Game.XNum; x++)
            {
                // 現在の座標に石が置かれていない場合
                if (stones[z][x].CurrentState == Stone.State.None)
                {                                                                           //データ捜査にバグあり
                    // 現在の色の石を置いた場合にひっくり返せる石の数を計算
                    var reverseCount = game.CalcTotalReverseCount(MyColor, x, z);

                    // ひっくり返せる石がある場合
                    if (reverseCount > 0)
                    {
                        // 利用可能なポイントとしてDictionaryに追加
                        availablePoints[Tuple.Create(x, z)] = reverseCount;
                    }
                }
            }
        }

        // 利用可能なポイントのDictionaryを返す
        return availablePoints;
    }


    // プレイヤーが石を置ける場所があるかどうかを判定するメソッド
    public bool CanPut()
    {
        // 利用可能なポイントを計算し、その数が0より大きいかどうかをチェック
        return CalcAvailablePoints().Count > 0;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
