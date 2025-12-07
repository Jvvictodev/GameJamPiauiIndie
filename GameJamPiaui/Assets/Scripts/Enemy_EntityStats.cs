using UnityEngine;

public class Enemy_EntityStats : MonoBehaviour
{
    public float max_hp;
    public float hp;
    public float attack_damage = 10;
    public float moveSpeed;

    void Start()
    {
        hp = max_hp;
    }

    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        
        Destroy(gameObject);
    }
}
