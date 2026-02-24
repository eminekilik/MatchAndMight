using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldEnemy : MonoBehaviour
{
    [Header("References")]
    public GameObject fightObject;
    public float activationDistance = 1.5f;

    private Transform player;
    private bool playerInRange = false;
    private Camera mainCamera;

    void Start()
    {

        if (SceneManager.GetActiveScene().name == "Battle")
        {
            enabled = false;
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;

        if (fightObject != null)
            fightObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        HandleDistanceCheck();
        HandleClick();
    }

    void HandleDistanceCheck()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= activationDistance)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                fightObject.SetActive(true);
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                fightObject.SetActive(false);
            }
        }
    }

    void HandleClick()
    {
        if (!playerInRange) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == fightObject)
            {
                StartBattle();
            }
        }
    }

    void StartBattle()
    {
        SceneManager.LoadScene("Battle");
    }
}