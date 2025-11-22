using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public float timeElapsed = 0f;
    public int inputCount = 0;      // ตัวแปรนับการกด WASD + Space
    private bool isTiming = false;

    public Text inputCountText;      // UI แสดงจำนวนการกด (ลาก Text ลง Inspector)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isTiming)
        {
            timeElapsed += Time.deltaTime;

            // อัปเดต UI ถ้ามี
            if (inputCountText != null)
                inputCountText.text = $"Inputs: {inputCount}";
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Stage1") // ซีนเกมหลัก
        {
            ResetScoreAndTime();
            StartTimer();
        }
        else if (scene.name == "EndScene")
        {
            StopTimer();
        }
        else
        {
            StopTimer();
        }

        // ลองเช็คว่าในซีนมี Text UI หรือไม่
        inputCountText = FindObjectOfType<Text>(); // หรือจะหาเฉพาะชื่อ Text ก็ได้
    }

    public void AddScore(int value)
    {
        score += value;
    }

    public void AddInputCount()
    {
        inputCount++;
    }

    public void ResetScoreAndTime()
    {
        score = 0;
        timeElapsed = 0f;
        inputCount = 0;
    }

    public void StartTimer()
    {
        timeElapsed = 0f;
        isTiming = true;
    }

    public void StopTimer()
    {
        isTiming = false;
    }
}
