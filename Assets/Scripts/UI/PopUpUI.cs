using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUI : MonoBehaviour
{

    [SerializeField] TMP_Text titleText;
    [SerializeField] Timer timer;
    [SerializeField] Button backButton;

    public void Initialize(string winnerText, float startTime, bool isGameover = false)
    {
        gameObject.SetActive(true);
        titleText.text = winnerText;
        if (isGameover)
        {
            backButton.gameObject.SetActive(true);
            timer.gameObject.SetActive(false);
        }
        else
        {
            backButton.gameObject.SetActive(false);
            timer.gameObject.SetActive(true);
            timer.StartTimer(startTime, () =>
            {
                gameObject.SetActive(false);
                GameManager.Instance.StartRound();
            });

        }
    }
}
