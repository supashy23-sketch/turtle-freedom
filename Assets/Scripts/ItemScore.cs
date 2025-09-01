using UnityEngine;

public class ItemScore : MonoBehaviour
{
    public int scoreValue = 10; // ตั้งค่าใน Inspector ได้

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) // Player ต้องมี Tag = "Player"
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(scoreValue);
            }

            Destroy(gameObject); // ทำลายไอเท็มหลังชน
        }
    }
}
