using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public float timeElapsed = 0f;

    private void Awake()
    {
        // ทำ Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // อยู่ข้ามซีน
        }
        else
        {
            Destroy(gameObject); // กันซ้ำ
        }
    }

    private void Update()
    {
        // นับเวลาในแต่ละซีน
        timeElapsed += Time.deltaTime;
    }

    private void OnEnable()
    {
        // ฟัง event เมื่อเปลี่ยนซีน
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Stage1")
        {
            ResetScoreAndTime();
        }
    }

    public void AddScore(int value)
    {
        score += value;
    }

    public void ResetScoreAndTime()
    {
        score = 0;
        timeElapsed = 0f;
    }
}
