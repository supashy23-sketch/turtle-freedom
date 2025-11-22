using UnityEngine;
using UnityEngine.UI;

public class FinalScoreUI : MonoBehaviour
{
    public Text finalScoreText;
    public Text finalTimeText;
    public Text finalInputCountText; // Text สำหรับโชว์จำนวนการกด WASD + Space

    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            finalScoreText.text = "Final Score: " + ScoreManager.Instance.score;
            finalTimeText.text = "Time Used: " + ScoreManager.Instance.timeElapsed.ToString("F2");
            
            if (finalInputCountText != null)
                finalInputCountText.text = "Steps Used: " + ScoreManager.Instance.inputCount;
        }
    }
}
