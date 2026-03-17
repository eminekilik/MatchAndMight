using UnityEngine;
using UnityEngine.EventSystems;
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

        if (PlayerPositionData.hasSavedPosition)
        {
            rb.position = PlayerPositionData.lastPosition;
            targetPosition = PlayerPositionData.lastPosition;
            PlayerPositionData.hasSavedPosition = false;
        }
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
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            targetPosition = worldPoint;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    void HandleMovement()
    {
        if (!isMoving)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 direction = (targetPosition - transform.position);
        float distance = direction.magnitude;

        direction.Normalize();

        rb.velocity = direction * moveSpeed;

        // Flip
        if (direction.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // STOP kontrolü (KRÝTÝK)
        if (distance <= stopDistance)
        {
            rb.velocity = Vector2.zero;
        }

        // Enemy kontrolü aynen kalsýn
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
        float speed = rb.velocity.magnitude;
        animator.SetBool("isWalking", speed > 0.05f);
    }

    void StartBattle()
    {
        SceneManager.LoadScene("Battle");
    }
}