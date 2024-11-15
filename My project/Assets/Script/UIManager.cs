using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI topText;//�^�[��
    [SerializeField]
    private TextMeshProUGUI blackScoreText;// ���v���C���[�̃X�R�A
    [SerializeField]
    private TextMeshProUGUI whiteScoreText;// ���v���C���[�̃X�R�A
    [SerializeField]
    private TextMeshProUGUI winnerText;// ����
    [SerializeField]
    private Image blackOverlay;// �w�i�I�[�o�[���C�i���U���g��ʂ̔w�i�j
    [SerializeField]
    private RectTransform playAgainButton;// �u������x�v���C�v�{�^��

    public void SetPlayerText(Player currentPlayer)
    {// ���݂̃v���C���[��\��
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
    {// �X�L�b�v���ꂽ�v���C���[��\��
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
        topText.text = message;// �C�ӂ̃��b�Z�[�W���g�b�v�e�L�X�g�ɕ\��
    }


    public IEnumerator AnimateTopText()
    {// �g�b�v�e�L�X�g�Ɋg��k���A�j���[�V������K�p
        topText.transform.LeanScale(Vector3.one * 1.2f, 0.25f).setLoopPingPong(4);
        yield return new WaitForSeconds(2);// �e�L�X�g�̃X�P�[����1.2�{�Ɋg��k���i4��J��Ԃ��j
    }

    private IEnumerator ScaleDown(RectTransform rect)
    {// �w�肳�ꂽRectTransform���k������\���ɂ���R���[�`��
        rect.LeanScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
        rect.gameObject.SetActive(false);
    }

    private IEnumerator ScaleUp(RectTransform rect)
    {// �w�肳�ꂽRectTransform���g�債�\������
        rect.gameObject.SetActive(true);
        rect.localScale = Vector3.zero;
        rect.LeanScale(Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator ShowScoreText()
    {// �X�R�A�\���e�L�X�g�����ɕ\��
        yield return ScaleDown(topText.rectTransform);
        yield return ScaleUp(blackScoreText.rectTransform);
        yield return ScaleUp(whiteScoreText.rectTransform);
    }

    public void SetBlackScoreTect(int score)
    {
        blackScoreText.text = $"Black {score}";// ���v���C���[�̃X�R�A���X�V
    }

    public void SetWhiteScoreText(int score)
    {
        whiteScoreText.text = $"White{score}";// ���v���C���[�̃X�R�A���X�V
    }

    private IEnumerator ShowOverlay()
    {// ���I�[�o�[���C���t�F�[�h�C���\��
        blackOverlay.gameObject.SetActive(true);
        blackOverlay.color= Color.clear;
        blackOverlay.rectTransform.LeanAlpha(0.8f, 1);// 1�b�����ē�������0.8�̕s�����x�܂ŕ\��
        yield return new WaitForSeconds(1);
    }

    private IEnumerator HideOverlay()
    {// ���I�[�o�[���C���t�F�[�h�A�E�g��\��
        blackOverlay.rectTransform.LeanAlpha(0, 1); // 1�b�����ăt�F�[�h�A�E�g
        yield return new WaitForSeconds(1);
        blackOverlay.gameObject.SetActive(false);
    }

    private IEnumerator MoveScoresDown()
    {// �X�R�A�\���e�L�X�g����ʉ��Ɉړ�������
        blackScoreText.rectTransform.LeanMoveY(0, 0.5f);
        whiteScoreText.rectTransform.LeanMoveY(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
    public void SetWinnerText(Player winner)
    {
        switch(winner) //���ҕ\��
        {// ���҂̃e�L�X�g��ݒ�
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
    {// �Q�[���I�����Ƀ��U���g��ʂ�\��
        yield return ShowOverlay(); // �w�i�I�[�o�[���C��\��
        yield return MoveScoresDown(); // �X�R�A�e�L�X�g�����Ɉړ�
        yield return ScaleUp(winnerText.rectTransform); // ���҃e�L�X�g��\��
        yield return ScaleUp(playAgainButton); // �u������x�v���C�v�{�^����\��
    }

    public IEnumerator HideEndScreen()//���U���g�̃{�^�����������Ƃ�
    {
        StartCoroutine(ScaleDown(winnerText.rectTransform)); // ���҃e�L�X�g���\��
        StartCoroutine(ScaleDown(blackScoreText.rectTransform)); // ���X�R�A���\��
        StartCoroutine(ScaleDown(whiteScoreText.rectTransform)); // ���X�R�A���\��
        StartCoroutine(ScaleDown(playAgainButton)); // �u������x�v���C�v�{�^�����\��

        yield return new WaitForSeconds(0.5f);
        yield return HideOverlay();//����ʂ�����
    }
}
