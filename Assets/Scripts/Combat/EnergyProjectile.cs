using UnityEngine;

public class EnergyProjectile : MonoBehaviour
{
    public float speed = 12f;

    Transform target;
    System.Action onHit;

    public void SetTarget(Transform t, System.Action hitCallback)
    {
        target = t;
        onHit = hitCallback;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            onHit?.Invoke();
            Destroy(gameObject);
        }
    }
}
