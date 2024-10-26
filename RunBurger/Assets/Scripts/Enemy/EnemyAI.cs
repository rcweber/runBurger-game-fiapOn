using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    [Header("Configurações de inteligencia do inimigo")]
    [Tooltip("Área de detecção do player")]
    [SerializeField] private float detectionRadius = 10f;

    [Tooltip("Cooldown para troca de alvo")]
    [SerializeField] private float targetChangeCoolDown = 5f;

    [Tooltip("Tempo par perder o alvo")]
    [SerializeField] private float timeToLoseTarget = 14f;

    [Header("Development configurations")]
    [Tooltip("Ativar modo de debug para exibir o raio de ação do inimigo")]
    [SerializeField] private bool debugMode = false;

    // propriedades privadadas
    private Transform currentTarget;
    private float lastTargetChangeTime;
    private CircleCollider2D detectionCollider;
    private Enemy enemy;
    private float tempTimeToLoseTarget = 0f;

    public bool IsSeeingPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        detectionCollider = gameObject.AddComponent<CircleCollider2D>();
        detectionCollider.radius = detectionRadius;
        detectionCollider.isTrigger = true;

        // Pegando a classe Enemy que está no GameObject pai
        enemy = GetComponent<Enemy>();
        tempTimeToLoseTarget = timeToLoseTarget;
    }

    // Quando um inimgo ver o player, ele vai setar o player que ele viu como alvo.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Neste momento, o inimigo viu o player, então setou ele como algo, mas não iniciou a perseguição
        // Verifica se o player é o alvo, 
        if (other.CompareTag("Player") && currentTarget == null)
        {
            // Esse é o player que o inimigo viu
            currentTarget = other.transform;
            IsSeeingPlayer = true;
            IniciaPerseguição();
        } else if (other.CompareTag("Player") && currentTarget != null && currentTarget != other.transform) {
            // Caso o inimigo já esteja perseguindo um player, ele vai trocar o alvo
            // Agora verifica se outro inimigo está mais perto do que o atual
            if (Time.time > lastTargetChangeTime + targetChangeCoolDown) {
                currentTarget = other.transform;
                IsSeeingPlayer = true;
                lastTargetChangeTime = Time.time;
                IniciaPerseguição();
            }
        }
    }

     private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Aqui o playeri saiu da vista do inimigo
            // por enquanto só aviso que não está vendo o inimigo
            IsSeeingPlayer = false;
            //StartCoroutine(CooldownBeforeLosingTarget());
        }
    }

    private void IniciaPerseguição()
    {
        var playerClassOnChasing = currentTarget.GetComponent<Player>();
        
        // Avisa o player que ele está sendo perseguido
        playerClassOnChasing.SetEnemyOnChasing(enemy);
        lastTargetChangeTime = Time.time;

        // Avisa o inimigo qual player ele ta perseguindo
        enemy.SetChaseState(currentTarget);
    }

    private void Update()
    {
        if (enemy.GetState() == Enemy.EnemyState.CHASE) {
            // Caso o inimigo esteja em processo de perseguição, preciso ir setando a nova posição do player
            if (currentTarget != null) {
                enemy.SetChaseState(currentTarget);
            }
        }

        // Se não estiver vendo o player, preciso verificar se o tempo de escapatória venceu, assim, altero o modo de patrulha novamente
        if (!IsSeeingPlayer && tempTimeToLoseTarget <= 0 && currentTarget != null) {
            // Neste caso, ja fazem os segundos necessários para perder o foco no player que estava sendo rastreado antes.
            tempTimeToLoseTarget = timeToLoseTarget;
            currentTarget = null;
            ChangeStateToPatrol();

            return;
        }
        tempTimeToLoseTarget -= Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Verifica se o playaer está dentro do raio de atuação do inimigo
        if (other.CompareTag("Player") && currentTarget == other.transform)
        {
            IsSeeingPlayer = true;
        }
    }

    private void ChangeStateToPatrol()
    {
        if (enemy.GetState() == Enemy.EnemyState.CHASE)
        {
            if (enemy != null) enemy.SetPatrolState();
            Debug.Log("Inimigo voltou a patrulhar");
        }
    }

    private void OnDrawGizmos()
    {
        if (debugMode)
        {
            // Define a cor do Gizmo (verde para o radar)
            Gizmos.color = Color.green;

            // Desenha um círculo ao redor do inimigo representando o alcance do radar
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }


}
