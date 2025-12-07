using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 0.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;

    public Transform attackPoint;

    private float nextAttackTime = 0f;

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0))
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void Attack()
    {
        // Detecta todos os colisores na EnemyLayer dentro do raio de ataque.
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Tenta obter o script de estatísticas do inimigo no objeto atingido.
            Enemy_EntityStats enemyStats = enemy.GetComponent<Enemy_EntityStats>();

            if (enemyStats != null)
            {
                // Se o script existe, chama a função TakeDamage.
                enemyStats.TakeDamage(attackDamage);

                // Mensagem de Debug para confirmar que o dano foi aplicado (para teste).
                Debug.Log(enemy.name + " atingido! Vida atual: " + enemyStats.hp);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}