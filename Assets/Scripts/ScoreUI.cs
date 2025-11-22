using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public Text scoreText;
    public Text timeText;

    void Update()
    {
        if (ScoreManager.Instance != null)
        {
            scoreText.text = "Score: " + ScoreManager.Instance.score;
            timeText.text = " " + ScoreManager.Instance.timeElapsed.ToString("F2");
        }
    }
}
