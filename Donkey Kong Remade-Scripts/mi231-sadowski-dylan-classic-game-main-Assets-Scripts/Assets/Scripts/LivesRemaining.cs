using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LivesRemaining : MonoBehaviour
{
    public Text countdownText;

    private int countdownValue = 2;

    private void Start()
    {
        countdownText.text = countdownValue.ToString();
    }

    public void UpdateCountdown()
    {
        countdownValue--;

        countdownText.text = countdownValue.ToString();

        if (countdownValue < 0)
        {
            gameObject.SetActive(false);

            SceneManager.LoadScene("GameOver");
        }
    }

    public void DecreaseLives()
    {
        countdownValue--;

        countdownText.text = countdownValue.ToString();

        if (countdownValue < 0)
        {
            gameObject.SetActive(false);

            SceneManager.LoadScene("GameOver");
        }
    }
}
