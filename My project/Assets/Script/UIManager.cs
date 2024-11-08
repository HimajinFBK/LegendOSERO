using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI topText;
    [SerializeField]
    private TextMeshProUGUI blackScoreText;
    [SerializeField]
    private TextMeshProUGUI whiteScoreText;
    [SerializeField]
    private TextMeshProUGUI winnerText;
    [SerializeField]
    private Image blackOverlay;
    [SerializeField]
    private RectTransform playAgainButton;

    public void SetPlayerText(Player currentPlayer)
    {
        if (currentPlayer == Player.Black)
        {
            topText.text = "黒のターン";
        }
        else if(currentPlayer == Player.White)
        {
            topText.text = "白のターン";
        }
    }

    public void SetSkippedText(Player skippedPlayer)
    {
        if (skippedPlayer == Player.Black)
        {
            topText.text = "黒　スキップ";
        }
        else if(skippedPlayer== Player.White)
        {
            topText.text = "白　スキップ";
        }
    }


    public IEnumerator AnimateTopText()
    {
        topText.transform.LeanScale(Vector3.one * 1.2f, 0.25f).setLoopPingPong(4);
        yield return new WaitForSeconds(2);
    }
}
