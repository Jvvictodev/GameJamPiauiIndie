using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // === PATRULHA ===
    public float forcaPatrulha = 10f;
    public float velocidadeMaximaPatrulha = 2f;
    public float distanciaVerificacaoChao = 0.5f;
    public LayerMask chaoLayer;
    private int direcaoAtual = 1;

    // === ATAQUE E VISÃO ===
    public float raioVisao = 5f;
    public float forcaPerseguicao = 20f;
    public float velocidadeMaximaPerseguicao = 4f;
    public LayerMask playerLayer;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float attackCooldown = 1.5f;

    public float forcaRecuo = 5f;
    private Rigidbody2D rb;
    private bool playerDetectado = false;
    private float lastAttackTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        
        ChecarVisao();

        if (playerDetectado)
        {
            PerseguirPlayer();
        }
        else
        {
            Patrulhar();
            ChecarLimitesPatrulha(); // Função agora chamada corretamente
        }
        void OncollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                InverterDirecao();
            }
        }
    }

    // --- LÓGICA DE MOVIMENTO ---

    void Patrulhar()
    {
        Mover(forcaPatrulha, velocidadeMaximaPatrulha);
    }

    void PerseguirPlayer()
    {
        Mover(forcaPerseguicao, velocidadeMaximaPerseguicao);

        // Checar se está próximo o suficiente para atacar (usando o raio de ataque)
        if (Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer))
        {
            TentarAtacar();
        }
    }

    void Mover(float forca, float velocidadeMaxima)
    {
        // Substituído rb.velocity por rb.linearVelocity
        float velocidadeX = rb.linearVelocity.x;

        // 1. Aplica a Força
        if (Mathf.Abs(velocidadeX) < velocidadeMaxima)
        {
            Vector2 forcaMovimento = new Vector2(direcaoAtual * forca * rb.mass, 0f);
            rb.AddForce(forcaMovimento);
        }

        // 2. Limita a Velocidade
        if (Mathf.Abs(velocidadeX) > velocidadeMaxima)
        {
            // Substituído rb.velocity por rb.linearVelocity
            rb.linearVelocity = new Vector2(Mathf.Sign(velocidadeX) * velocidadeMaxima, rb.linearVelocity.y);
        }
    }

    // --- LÓGICA DE VISÃO E ATAQUE ---

    void ChecarVisao()
    {
        Vector2 pontoDeOrigem = transform.position;
        Vector2 direcao = new Vector2(direcaoAtual, 0f);

        RaycastHit2D hit = Physics2D.Raycast(pontoDeOrigem, direcao, raioVisao, playerLayer);

        if (hit.collider != null)
        {
            // Se detectar o player, vira para a direção dele para persegui-lo
            if (Mathf.Sign(hit.transform.position.x - transform.position.x) != direcaoAtual)
            {
                InverterDirecao();
            }
            playerDetectado = true;
        }
        else
        {
            playerDetectado = false;
        }
    }

    void TentarAtacar()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            FacaHitboxLogic(); // Chamando a função com o sufixo 'Logic' para evitar confusão de nome
            lastAttackTime = Time.time;
        }
    }



    // Função corrigida e renomeada para FacaHitboxLogic
    void FacaHitboxLogic()
    {

        // 1. OBTÉM O VALOR DE DANO DO PRÓPRIO INIMIGO
        // É essencial que o script Enemy_EntityStats esteja anexado ao mesmo objeto
        Enemy_EntityStats enemyStats = GetComponent<Enemy_EntityStats>();
        if (enemyStats == null)
        {
            Debug.LogError("O script Enemy_EntityStats não foi encontrado no Inimigo!");
            return;
        }
        float danoDoInimigo = enemyStats.attack_damage;


        // 2. CRIA O HITBOX E APLICA O DANO
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("interação de hit ocorreu");
            if (hit.gameObject.CompareTag("Player"))
            {
                // Tenta obter o script de saúde do Player
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                Debug.Log("obteve a vida do player");
                if (playerHealth != null)
                {
                    // Causa dano com base no status attack_damage do inimigo
                    playerHealth.TakeDamage(danoDoInimigo);
                    Debug.Log("Tartaruga atingida pela faca! Dano causado: " + danoDoInimigo);
                }
            }
        }
    }

    // --- LÓGICA DE PATRULHA PADRÃO ---

    // Função corrigida e chamada corretamente
    void ChecarLimitesPatrulha()
    {
        if (playerDetectado) return;

        Vector3 pontoDeChecagemChao = transform.position + new Vector3(direcaoAtual * 0.5f, 0f, 0f);
        RaycastHit2D hitChao = Physics2D.Raycast(pontoDeChecagemChao, Vector2.down, distanciaVerificacaoChao, chaoLayer);

        if (hitChao.collider == null)
        {
            InverterDirecao();
        }
    }

    void InverterDirecao()
    {
        direcaoAtual *= -1;
        transform.localScale = new Vector3(direcaoAtual, 1, 1);
    }

    private void OnDrawGizmos()
    {
        // ... (Gizmos para visualização)
    }
    public void nockback(Transform atacante)
    {


        // 2. Lógica de Empurrar o Inimigo (Knockback)
        if (rb != null && atacante != null)
        {
            // Calcula a direção: Posição do Inimigo - Posição do Player
            // Isso cria um vetor que aponta PARA LONGE do player
            Vector2 direcaoEmpurrao = (transform.position - atacante.position).normalized;

            // Zera a velocidade atual para o impacto ser seco e consistente
            rb.linearVelocity = Vector2.zero;

            // Aplica a força de impulso instantânea
            rb.AddForce(direcaoEmpurrao * forcaRecuo, ForceMode2D.Impulse);
        }

        
    }
}