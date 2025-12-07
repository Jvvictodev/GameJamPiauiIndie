using UnityEngine;
using UnityEngine.SceneManagement; // Opcional: Para reiniciar o nível ao morrer

public class PlayerHealth : MonoBehaviour
{
    public float maxHp = 100f; // Vida máxima do Player
    public float hp;           // Vida atual
    private float proximoTempoDano = 0f;
    public float intervalo = 2f;
    Enemy_EntityStats enemyStats;
    void Start()
    {
        hp = maxHp;
        enemyStats = new Enemy_EntityStats();
    }

    // Função pública chamada pelo inimigo para causar dano
    public void TakeDamage(float damage)
    {
        hp -= damage;
        Debug.Log("Vida do Player: " + hp);

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player morreu!");

        // Aqui você pode adicionar lógica de animação de morte, tela de Game Over, etc.
        // Por enquanto, vamos apenas desativar o objeto.
        gameObject.SetActive(false);

        // Exemplo: Reiniciar a cena após 2 segundos
        Invoke("RestartScene", 2f);
    }

    void RestartScene()
    {
        // Carrega a cena atual novamente
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hp -= enemyStats.attack_damage;

        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        // Verifica se o objeto que estamos tocando é o inimigo
        if (collision.gameObject.CompareTag("Enemy")) // Ou "Player", dependendo de onde está o script
        {
            // Verifica se o tempo atual já passou do tempo agendado
            if (Time.time >= proximoTempoDano)
            {
                hp -= enemyStats.attack_damage;
                proximoTempoDano = Time.time + intervalo;
                if (hp <= 0)
                {
                    Die();
                }
            }
        }
    }
}