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

    private float stuckTimer = 0f;
    public float stuckTimeThreshold = 0.2f; // 0.2 saniye bekle

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

            PlayerTreeInteraction treeInteraction = GetComponent<PlayerTreeInteraction>();
            Tree currentTree = treeInteraction.CurrentTree;

            bool isCuttingNow = false;

            // Eðer yakýnýndaki aðaç var ve týklanan nokta aðaç collider'ýna giriyorsa
            if (currentTree != null)
            {
                Collider2D treeCollider = currentTree.GetComponent<Collider2D>();
                if (treeCollider == Physics2D.OverlapPoint(worldPoint))
                {
                    // Kesme baþlat
                    //treeInteraction.OnCutAnimationEnd();
                    //isCuttingNow = true;
                    treeInteraction.StartCutting();

                    // Karakter hareket etmesin
                    isMoving = false;
                    return; // erken return ile hareket hedefi deðiþtirilmesin
                }
            }

            // Eðer kesme baþlatýlmadýysa normal hareket
            if (!isCuttingNow)
            {
                targetPosition = worldPoint;
                isMoving = true;
            }
        }
    }

    void HandleMovement()
    {
        if (!isMoving)
        {
            rb.velocity = Vector2.zero;
            stuckTimer = 0f;
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
            isMoving = false;
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
        float movedDistance = Vector2.Distance(rb.position, lastPosition);

        bool isActuallyMoving = movedDistance > 0.001f;

        animator.SetBool("isWalking", isActuallyMoving);

        if (isMoving)
        {
            if (!isActuallyMoving)
            {
                stuckTimer += Time.fixedDeltaTime;

                if (stuckTimer >= stuckTimeThreshold)
                {
                    isMoving = false;
                    rb.velocity = Vector2.zero;
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
            }
        }

        lastPosition = rb.position;
    }

    void StartBattle()
    {
        SceneManager.LoadScene("Battle");
    }
}