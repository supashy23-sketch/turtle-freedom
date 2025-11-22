using UnityEngine;
using System.Collections;

public class ItemScore : MonoBehaviour
{
    public int scoreValue = 10;

    public float disappearDuration = 0.5f; // เวลาหมุนและหายไป
    public float rotationSpeed = 360f;     // ความเร็วหมุน (องศาต่อวินาที)

    private AudioSource audioSource;
    private bool isCollected = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;
        if (collision.CompareTag("Player"))
        {
            isCollected = true;

            Debug.Log("ชน Player แล้ว + " + scoreValue + " คะแนน");

            // เพิ่มคะแนน
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(scoreValue);

            // เริ่ม Coroutine เล่นเสียงและหมุนหายไป
            StartCoroutine(PlaySoundAndDisappear());
        }
    }

    private IEnumerator PlaySoundAndDisappear()
    {
        // เล่นเสียงจาก AudioSource ที่กำหนดใน Inspector
        if (audioSource != null)
            audioSource.Play();

        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsed < disappearDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / disappearDuration;

            // หมุน
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            // ลดขนาดลง
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);

            yield return null;
        }

        // ทำลายตัวเอง
        Destroy(gameObject);
    }
}
