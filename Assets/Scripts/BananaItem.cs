using UnityEngine;

public class BananaItem : MonoBehaviour
{
    public AudioClip slipSound;
    private AudioSource audioSource;
    [Header("Slip Settings")]
    public float slipDistance = 1f; // กำหนดว่าลื่นกี่ gridSize

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogError("BananaItem: No AudioSource found!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player != null)
        {
            // เล่นเสียงจาก AudioSource ใน Inspector
            if (slipSound && audioSource)
                audioSource.PlayOneShot(slipSound);

           Vector3 slipTarget = player.transform.position +
                     new Vector3(player.lastDir.x, player.lastDir.y, 0f) * player.gridSize * slipDistance;

            StartCoroutine(SlipPlayer(player, slipTarget));
        }
    }

    private System.Collections.IEnumerator SlipPlayer(PlayerController player, Vector3 targetPos)
    {
        player.ForceStopMovement();  
        float slipSpeed = player.moveSpeed * 1.5f;

        while ((targetPos - player.transform.position).sqrMagnitude > 0.001f)
        {
            player.transform.position = Vector3.MoveTowards(
                player.transform.position,
                targetPos,
                slipSpeed * Time.deltaTime
            );
            yield return null;
        }

        // รอให้เสียงเล่นจบก่อนค่อยลบ
        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }
}
