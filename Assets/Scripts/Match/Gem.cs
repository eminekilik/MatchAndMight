using UnityEngine;

public class Gem : MonoBehaviour
{
    public GemType gemType;
    public Tile currentTile;

    Vector2 startTouchPos;

    void OnMouseDown()
    {
        startTouchPos = Input.mousePosition;
    }

    void OnMouseUp()
    {
        Vector2 endTouchPos = Input.mousePosition;
        Vector2 delta = endTouchPos - startTouchPos;

        if (delta.magnitude < 30f)
            return; // çok kýsa sürükleme, iptal

        Vector2 dir;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            dir = delta.x > 0 ? Vector2.right : Vector2.left;
        else
            dir = delta.y > 0 ? Vector2.up : Vector2.down;

        BoardManager.Instance.TrySwapFromDirection(this, dir);
    }


}
