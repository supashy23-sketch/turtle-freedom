using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gridSize = 1f;

    private bool isMoving;
    private Vector2 input;
    private Vector2 lastDir;
    private Animator animator;

    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask battleLayer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            // ตรวจจับการกดปุ่มทีละครั้ง (เพื่อให้เดินทีละช่อง)
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) input = Vector2.up;
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) input = Vector2.down;
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) input = Vector2.left;
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) input = Vector2.right;
            else input = Vector2.zero;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                lastDir = input;

                var targetPos = transform.position + new Vector3(input.x, input.y, 0f) * gridSize;

                if (IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }
        }

        // ตั้งค่าแอนิเมชัน
        animator.SetBool("isMoving", isMoving);

        // กด Z เพื่อ Interact
        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Snap ตำแหน่งให้อยู่ตรงกลางช่อง Grid
        transform.position = new Vector3(
            Mathf.Round(transform.position.x / gridSize) * gridSize,
            Mathf.Round(transform.position.y / gridSize) * gridSize,
            transform.position.z
        );

        isMoving = false;
        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer | interactableLayer) == null;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, battleLayer))
        {
            if (Random.Range(1, 101) <= 20)
                Debug.Log("A battle has started!");
        }
    }

    void Interact()
    {
        var facingDir = new Vector3(lastDir.x, lastDir.y);
        var interactPos = transform.position + facingDir * gridSize;

        var collider = Physics2D.OverlapCircle(interactPos, 0.2f, interactableLayer);
        if (collider != null)
            collider.GetComponent<Interactable>()?.Interact();
    }
}
