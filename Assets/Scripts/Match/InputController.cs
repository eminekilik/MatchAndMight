using UnityEngine;

public class InputController : MonoBehaviour
{
    Vector2 startTouchPos;
    Gem selectedGem;

    void Update()
    {
        if (GameFlowController.Instance == null)
            return;

        if (!GameFlowController.Instance.CanInteract())
            return;

        if (CombatManager.Instance == null)
            return;

        if (CombatManager.Instance.currentState != CombatManager.CombatState.PlayerTurn)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            startTouchPos = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
                selectedGem = hit.collider.GetComponent<Gem>();
        }

        if (Input.GetMouseButtonUp(0) && selectedGem != null)
        {
            Vector2 endTouchPos = Input.mousePosition;
            Vector2 delta = endTouchPos - startTouchPos;

            if (delta.magnitude >= 30f)
            {
                Vector2 dir;

                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    dir = delta.x > 0 ? Vector2.right : Vector2.left;
                else
                    dir = delta.y > 0 ? Vector2.up : Vector2.down;

                BoardManager.Instance.TrySwapFromDirection(selectedGem, dir);
            }

            selectedGem = null;
        }
    }
}
