using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI topText;//ターン
    [SerializeField]
    private TextMeshProUGUI blackScoreText;// 黒プレイヤーのスコア
    [SerializeField]
    private TextMeshProUGUI whiteScoreText;// 白プレイヤーのスコア
    [SerializeField]
    private TextMeshProUGUI winnerText;// 勝者
    [SerializeField]
    private Image blackOverlay;// 背景オーバーレイ（リザルト画面の背景）
    [SerializeField]
    private RectTransform playAgainButton;// 「もう一度プレイ」ボタン

    public void SetPlayerText(Player currentPlayer)
    {// 現在のプレイヤーを表示
        if (currentPlayer == Player.Black)
        {
            topText.text = "Black Turn";
        }
        else if(currentPlayer == Player.White)
        {
            topText.text = "White Turn";
        }
    }

    public void SetSkippedText(Player skippedPlayer)
    {// スキップされたプレイヤーを表示
        if (skippedPlayer == Player.Black)
        {
            topText.text = "Black skip";
        }
        else if(skippedPlayer== Player.White)
        {
            topText.text = "White skip";
        }
    }

    public void SetTopText(string message)
    {
        topText.text = message;// 任意のメッセージをトップテキストに表示
    }


    public IEnumerator AnimateTopText()
    {// トップテキストに拡大縮小アニメーションを適用
        topText.transform.LeanScale(Vector3.one * 1.2f, 0.25f).setLoopPingPong(4);
        yield return new WaitForSeconds(2);// テキストのスケールを1.2倍に拡大縮小（4回繰り返し）
    }

    private IEnumerator ScaleDown(RectTransform rect)
    {// 指定されたRectTransformを縮小し非表示にするコルーチン
        rect.LeanScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
        rect.gameObject.SetActive(false);
    }

    private IEnumerator ScaleUp(RectTransform rect)
    {// 指定されたRectTransformを拡大し表示する
        rect.gameObject.SetActive(true);
        rect.localScale = Vector3.zero;
        rect.LeanScale(Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator ShowScoreText()
    {// スコア表示テキストを順に表示
        yield return ScaleDown(topText.rectTransform);
        yield return ScaleUp(blackScoreText.rectTransform);
        yield return ScaleUp(whiteScoreText.rectTransform);
    }

    public void SetBlackScoreTect(int score)
    {
        blackScoreText.text = $"Black {score}";// 黒プレイヤーのスコアを更新
    }

    public void SetWhiteScoreText(int score)
    {
        whiteScoreText.text = $"White{score}";// 白プレイヤーのスコアを更新
    }

    private IEnumerator ShowOverlay()
    {// 黒オーバーレイをフェードイン表示
        blackOverlay.gameObject.SetActive(true);
        blackOverlay.color= Color.clear;
        blackOverlay.rectTransform.LeanAlpha(0.8f, 1);// 1秒かけて透明から0.8の不透明度まで表示
        yield return new WaitForSeconds(1);
    }

    private IEnumerator HideOverlay()
    {// 黒オーバーレイをフェードアウト非表示
        blackOverlay.rectTransform.LeanAlpha(0, 1); // 1秒かけてフェードアウト
        yield return new WaitForSeconds(1);
        blackOverlay.gameObject.SetActive(false);
    }

    private IEnumerator MoveScoresDown()
    {// スコア表示テキストを画面下に移動させる
        blackScoreText.rectTransform.LeanMoveY(0, 0.5f);
        whiteScoreText.rectTransform.LeanMoveY(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
    public void SetWinnerText(Player winner)
    {
        switch(winner) //勝者表示
        {// 勝者のテキストを設定
            case Player.Black:
                winnerText.text = "Black Win!";
                break;
            case Player.White:
                winnerText.text = "White Win!";
                    break;
            case Player None:
                winnerText.text = "Draw!";
                break;
        }
    }

    public IEnumerator ShowEndScreen()
    {// ゲーム終了時にリザルト画面を表示
        yield return ShowOverlay(); // 背景オーバーレイを表示
        yield return MoveScoresDown(); // スコアテキストを下に移動
        yield return ScaleUp(winnerText.rectTransform); // 勝者テキストを表示
        yield return ScaleUp(playAgainButton); // 「もう一度プレイ」ボタンを表示
    }

    public IEnumerator HideEndScreen()//リザルトのボタンを押したとき
    {
        StartCoroutine(ScaleDown(winnerText.rectTransform)); // 勝者テキストを非表示
        StartCoroutine(ScaleDown(blackScoreText.rectTransform)); // 黒スコアを非表示
        StartCoroutine(ScaleDown(whiteScoreText.rectTransform)); // 白スコアを非表示
        StartCoroutine(ScaleDown(playAgainButton)); // 「もう一度プレイ」ボタンを非表示

        yield return new WaitForSeconds(0.5f);
        yield return HideOverlay();//黒画面を消す
    }
}
