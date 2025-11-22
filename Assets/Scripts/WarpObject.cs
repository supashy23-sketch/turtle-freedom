using UnityEngine;
using System.Collections;

public class WarpObject : MonoBehaviour
{
    public Transform warpTarget;        // จุดปลายทางวาร์ป
    public AudioClip warpSound;         // เสียงวาร์ป
    public float delayBeforeDestroy = 0.5f;  // เวลาหน่วงก่อนลบตัวเอง (ให้เสียงเล่น)

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // หยุดการเคลื่อนที่ของผู้เล่น
            var playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
                playerController.ForceStopMovement();

            // วาร์ปผู้เล่นไปตำแหน่งปลายทาง
            collision.transform.position = warpTarget.position;

            // ลบปลายทางทันที
            if (warpTarget != null)
                Destroy(warpTarget.gameObject);

            // เรียก Coroutine เล่นเสียงและลบตัวเอง
            StartCoroutine(PlaySoundAndDestroy());
        }
    }

    private IEnumerator PlaySoundAndDestroy()
    {
        if (warpSound != null)
        {
            audioSource.PlayOneShot(warpSound);
            // รอจนกว่าเสียงจะเล่นจบ
            yield return new WaitForSeconds(warpSound.length);
        }
        else
        {
            yield return new WaitForSeconds(delayBeforeDestroy);
        }

        // ลบตัว WarpObject หลังเสียงเล่นจบ
        Destroy(gameObject);
    }
}
