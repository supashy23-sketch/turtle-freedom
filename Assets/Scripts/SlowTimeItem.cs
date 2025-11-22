using UnityEngine;
using System.Collections;

public class SlowTimeItem : MonoBehaviour
{
    public float slowDuration = 10f;      // ระยะเวลาที่ศัตรูเดินช้า
    public float slowAmount = 0.5f;       // ลดความเร็วลงเป็น 50% ของเดิม
    public AudioClip pickupSound;         // เสียงเก็บไอเท็ม

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

            // เล่นเสียงถ้ามี
            if (pickupSound != null)
                audioSource.PlayOneShot(pickupSound);

            // ทำลายตัวเองหลังเล่นเสียงจบ
            float destroyDelay = (pickupSound != null) ? pickupSound.length : 0f;
            Destroy(gameObject, destroyDelay);

            // เรียก Coroutine ลดความเร็วศัตรู
            StartCoroutine(SlowEnemiesCoroutine());
        }
    }

    private IEnumerator SlowEnemiesCoroutine()
    {
        // ลดความเร็วศัตรูทุกตัวทันที
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.SetGlobalSpeedMultiplier(slowAmount);

        // รอช้า
        yield return new WaitForSeconds(slowDuration);

        // คืนความเร็วเดิม
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.SetGlobalSpeedMultiplier(1f);
    }
}
