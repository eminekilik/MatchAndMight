using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool movingToEnemy = false;
    public float battleDistance = 0.5f;

    private Rigidbody2D rb;

    private Vector2 lastPosition;

    void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        targetPosition = transform.position;
        lastPosition = rb.position;
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        HandleMovement();
        UpdateAnimation();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            targetPosition = worldPoint;
            isMoving = true;
        }
    }

    void HandleMovement()
    {
        if (!isMoving) return;

        Vector2 newPosition = Vector2.MoveTowards
        (
            rb.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);

        Vector3 direction = targetPosition - transform.position;

        if (direction.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        float distance = Vector3.Distance(rb.position, targetPosition);

        if (distance <= stopDistance)
        {
            isMoving = false;
        }

        if (movingToEnemy && currentEnemyTarget != null)
        {
            float enemyDistance = Vector3.Distance(rb.position, currentEnemyTarget.position);

            if (enemyDistance <= battleDistance)
            {
                isMoving = false;
                movingToEnemy = false;

                StartBattle();
            }
        }
    }

    void UpdateAnimation()
    {
        float movedDistance = Vector2.Distance(rb.position, lastPosition);

        bool actuallyMoving = movedDistance > 0.001f;

        animator.SetBool("isWalking", actuallyMoving);

        lastPosition = rb.position;
    }

    void StartBattle()
    {
        SceneManager.LoadScene("Battle");
    }
}