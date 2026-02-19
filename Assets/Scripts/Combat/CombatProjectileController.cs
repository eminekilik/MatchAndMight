using UnityEngine;

public class CombatProjectileController : MonoBehaviour
{
    public GameObject playerProjectilePrefab;
    public GameObject enemyProjectilePrefab;

    public void FirePlayerProjectile(Vector3 startPos, int damage, Transform enemyTarget)
    {
        if (playerProjectilePrefab == null || enemyTarget == null)
            return;

        GameObject proj = Instantiate(playerProjectilePrefab, startPos, Quaternion.identity);
        EnergyProjectile ep = proj.GetComponent<EnergyProjectile>();

        if (ep != null)
        {
            ep.SetTarget(enemyTarget, () =>
            {
                CombatManager.Instance.enemyHP -= damage;
                CombatManager.Instance.ClampValues();
                CombatManager.Instance.UpdateUI();

                CombatManager.Instance.effects.TriggerEnemyHitJuice();
                CombatManager.Instance.effects.ShowDamageText(damage, enemyTarget.position);
            });
        }
    }

    public void FireEnemyProjectile(Transform enemyTarget, Transform playerTarget)
    {
        if (enemyProjectilePrefab == null || playerTarget == null)
            return;

        GameObject proj = Instantiate(enemyProjectilePrefab, enemyTarget.position, Quaternion.identity);
        EnergyProjectile ep = proj.GetComponent<EnergyProjectile>();

        if (ep != null)
        {
            ep.SetTarget(playerTarget, () =>
            {
                int damage = Random.Range(6, 14);
                CombatManager.Instance.playerHP -= damage;

                CombatManager.Instance.ClampValues();
                CombatManager.Instance.UpdateUI();

                CombatManager.Instance.effects.TriggerPlayerHitJuice();
                CombatManager.Instance.effects.ShowDamageText(damage, playerTarget.position);
                CombatManager.Instance.effects.ScreenFlash();
            });
        }
    }
}
