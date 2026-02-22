using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stopDistance = 0.1f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private Animator animator;
    private Camera mainCamera;

    private Transform currentEnemyTarget;

    void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                //if (hit.collider.CompareTag("Enemy"))
                //{
                //    currentEnemyTarget = hit.collider.transform;
                //    targetPosition = currentEnemyTarget.position;
                //    isMoving = true;
                //}
                //else
                if (hit.collider.CompareTag("Ground"))
                {
                    currentEnemyTarget = null;
                    targetPosition = worldPoint;
                    isMoving = true;
                }
            }
        }
    }

    void HandleMovement()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        Vector3 direction = targetPosition - transform.position;

        if (direction.x > 0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance <= stopDistance)
        {
            isMoving = false;
            animator.SetBool("isWalking", false);

            if (currentEnemyTarget != null)
            {
                StartBattle();
            }
        }
        else
        {
            animator.SetBool("isWalking", true);
        }
    }

    void StartBattle()
    {
        Debug.Log("Battle Basladi");

        // Buraya Battle Scene load ekleyeceðiz
        // SceneManager.LoadScene("BattleScene");
    }
}