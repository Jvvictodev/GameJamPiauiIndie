using UnityEngine;

public class BasicMovementWithJump : MonoBehaviour
{
    public float movementForce = 20f;
    public float jumpForce = 400f;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded = false;

    // --- NOVA VARIÁVEL PARA O FLIP ---
    // Define se o personagem começa olhando para a direita
    private bool olhandoParaDireita = true;

    // Variáveis de Knockback (Se o player tomar dano)
    public float knockbackForce = 500f;
    public float knockbackDuration = 0.15f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // --- LÓGICA DE VIRAR O PERSONAGEM ---
        // Se move para a direita e não está olhando para direita -> Vira
        if (horizontalInput > 0 && !olhandoParaDireita)
        {
            Flip();
        }
        // Se move para a esquerda e está olhando para direita -> Vira
        else if (horizontalInput < 0 && olhandoParaDireita)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        ApplyHorizontalForce();
    }

    void ApplyHorizontalForce()
    {
        Vector2 forceToApply = new Vector2(horizontalInput * movementForce, 0f);
        rb.AddForce(forceToApply);
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;
    }

    // --- NOVA FUNÇÃO DE FLIP ---
    void Flip()
    {
        // Inverte o estado da variável
        olhandoParaDireita = !olhandoParaDireita;

        // Pega a escala atual do objeto
        Vector3 escala = transform.localScale;

        // Multiplica o X por -1 para espelhar
        escala.x *= -1;

        // Aplica a nova escala
        transform.localScale = escala;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // --- CORREÇÃO NO KNOCKBACK DO PLAYER ---
    // Mudei o nome do parâmetro para 'posicaoAtacante', pois 'playerPosition' 
    // confundiria a conta (já que este script ESTÁ no player)
    public void ApplyKnockback(Vector3 posicaoAtacante)
    {
        if (rb == null) return;

        // Calcula a direção: Do Player PARA LONGE do Atacante (Inimigo)
        Vector2 direction = (transform.position - posicaoAtacante).normalized;

        Vector2 knockbackVector = new Vector2(direction.x, direction.y * 0.2f).normalized;

        rb.linearVelocity = Vector2.zero; // Zera a velocidade antes do impacto
        rb.AddForce(knockbackVector * knockbackForce, ForceMode2D.Impulse);

        Invoke("StopKnockbackForce", knockbackDuration);
    }

    void StopKnockbackForce()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }
}
