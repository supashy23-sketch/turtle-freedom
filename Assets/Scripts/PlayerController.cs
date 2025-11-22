using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 previousPosition;
    private Coroutine moveRoutine;
    private Coroutine dashRoutine;
    public GameObject footstepPrefab;     // Prefab รอยเท้า
    public AudioClip footstepSound;       // เสียงเดิน
    private AudioSource audioSource;

    public AudioClip dashSound;            // เสียงตอน Dash
    public GameObject dashFootstepPrefab; // หากต้องการให้ต่างจากเดินปกติ (optional)


    public float moveSpeed = 5f;
    public float gridSize = 1f;

    private bool isMoving;
    private Vector2 input;
    public Vector2 lastDir;
    private Animator animator;

    public float dashDistanceMultiplier = 3f;       // ระยะพุ่ง = gridSize * 3
    public float dashSpeedMultiplier = 3f;          // ความเร็วพุ่งไวกว่าเดิน
    public float dashCooldown = 10f;                // 10 วิ
    private float dashCooldownTimer = 0f;           // ตัวจับเวลา

    public UnityEngine.UI.Slider dashCooldownUI;    // ลาก UI Slider มาใส่

    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask battleLayer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void HandleUpdate()
    {
        Vector3 delta = transform.position - previousPosition;

        if (delta.sqrMagnitude > 0.0001f)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                lastDir = new Vector2(Mathf.Sign(delta.x), 0);
            else
                lastDir = new Vector2(0, Mathf.Sign(delta.y));
        }

        previousPosition = transform.position;
        // อัปเดต UI และตัวจับเวลา
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
            dashCooldownUI.value = dashCooldown - dashCooldownTimer;
        }
        else
        {
            dashCooldownUI.value = dashCooldown;
        }

        if (!isMoving)
        {
            bool moved = false;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) { input = Vector2.up; moved = true; }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) { input = Vector2.down; moved = true; }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) { input = Vector2.left; moved = true; }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) { input = Vector2.right; moved = true; }
            else input = Vector2.zero;

            if (moved && ScoreManager.Instance != null)
                ScoreManager.Instance.AddInputCount();  // เพิ่ม count การกดปุ่มเดิน

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                lastDir = input;

                var targetPos = transform.position + new Vector3(input.x, input.y, 0f) * gridSize;
                if (IsWalkable(targetPos))
                    moveRoutine = StartCoroutine(Move(targetPos));
            }

            // Dash
            if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0f)
            {
                if (ScoreManager.Instance != null)
                    ScoreManager.Instance.AddInputCount();  // เพิ่ม count การกด Space
                dashRoutine = StartCoroutine(Dash());
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

        // 🎵 เล่นเสียงเดิน
        if (footstepSound && audioSource)
            audioSource.PlayOneShot(footstepSound);

        // 👣 สร้างรอยเท้า
        SpawnFootstep(footstepPrefab);


        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

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
    IEnumerator Dash()
    {
        dashCooldownTimer = dashCooldown;
        dashCooldownUI.value = 0;

        // 🎵 เล่นเสียง Dash แยกจากเสียงเดิน
        if (dashSound && audioSource)
            audioSource.PlayOneShot(dashSound);

        // พุ่งทีละช่อง 3 ครั้ง
        for (int i = 1; i <= dashDistanceMultiplier; i++)
        {
            Vector3 nextPos = transform.position + new Vector3(lastDir.x, lastDir.y, 0f) * gridSize;

            if (!IsWalkable(nextPos))
                break;

            // 👣 สร้างรอยเท้า Dash ที่จุดเริ่มของแต่ละช่อง
            SpawnFootstep(dashFootstepPrefab);

            isMoving = true;

            // พุ่งเร็วกว่าเดินปกติ
            while ((nextPos - transform.position).sqrMagnitude > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    nextPos,
                    moveSpeed * dashSpeedMultiplier * Time.deltaTime
                );
                yield return null;
            }

            // Snap Grid
            transform.position = new Vector3(
                Mathf.Round(transform.position.x / gridSize) * gridSize,
                Mathf.Round(transform.position.y / gridSize) * gridSize,
                transform.position.z
            );
        }

        isMoving = false;
    }


    void SpawnFootstep(GameObject prefab)
    {
        if (prefab == null) return;

        Vector3 spawnPos = transform.position;
        GameObject foot = Instantiate(prefab, spawnPos, Quaternion.identity);

        // หมุนตามทิศทางการเดิน
        if (lastDir == Vector2.up) foot.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (lastDir == Vector2.down) foot.transform.rotation = Quaternion.Euler(0, 0, 180);
        else if (lastDir == Vector2.left) foot.transform.rotation = Quaternion.Euler(0, 0, 90);
        else if (lastDir == Vector2.right) foot.transform.rotation = Quaternion.Euler(0, 0, -90);

        Destroy(foot, 3f);
    }

    

    public void ForceStopMovement()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }

        if (dashRoutine != null)
        {
            StopCoroutine(dashRoutine);
            dashRoutine = null;
        }

        isMoving = false;
    }

    





}//end
