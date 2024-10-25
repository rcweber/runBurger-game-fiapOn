using UnityEngine.AI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Configuração do Inimigo")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator anim;

    [Header("Configuração de Navegação")]
    [Tooltip("Ponto de patrulha do inimigo")]
    [SerializeField] private Transform[] patrolPoints;

    [Header("Configuração da velocidade de patrulha")]
    [Tooltip("Velocidade mínima de patrulha")]
    [SerializeField] private float patrolSpeedMin = 0.1f;
    [Tooltip("Velocidade máxima de patrulha")]
    [SerializeField] private float patrolSpeedMax = 0.5f;

    [Header("Configuração da velocidade Perseguição")]
    [Tooltip("Velocidade de perseguição")]
    [SerializeField] private float chaseSpeed = 0.8f;

    [Tooltip("Distância mínima para determinar que chegou no patrol point")]
    [SerializeField] private float distanceThreshold = 0.01f;

    [Tooltip("Tempo de espera em cada ponto de patrulha")]
    [SerializeField] private float waitTimeMin = 1.1f;
    [SerializeField] private float waitTimeMax = 3.5f;

    [Header("Configuração que verifica inimigo travado")]
    [Tooltip("Intervalo de tempo para verificar se o inimigo está travado")]
    [SerializeField] private float stuckCheckInterval = 2.0f;

    [Tooltip("Distância máxima para determinar que o inimigo está travado")]
    [SerializeField] private float stuckDistanceThreshold = 0.1f;

    [Tooltip("Tempo máximo para determinar que o inimigo está travado")]
    [SerializeField] private float sutckTimeLimit = 3.0f;

    [Header("Configuração de hit no player")]
    [SerializeField] private float enemyDamage = 3.33f;

    private List<int> patrolIndexesToIgnore = new();
    private Transform playerInChasing = null;

    public int GetPartrolIndex() => currentPatrolIndex;
    public float GetEnemyDamage() => enemyDamage;

    // propriedades privadas
    private float waitCounter;
    private int currentPatrolIndex;
    private bool waiting;
    public enum EnemyState
    {
        PATROL,
        CHASE
    }
    private EnemyState enemyState;
    private Vector3 lastPosition;
    private float timeSinceLastMove;
    private float timeSinceStuckCheck;
    private Transform currentPatrolPoint => patrolPoints[currentPatrolIndex];

    public bool IsStopped => agent.isStopped;


    void Start()
    {
        // loading the patrol points
        patrolPoints = GameObject.FindGameObjectsWithTag("ControlPoint").Select(x => x.transform).ToArray();
        patrolIndexesToIgnore = FindObjectsOfType<Enemy>().ToList().Select(x => x.GetPartrolIndex()).ToList();
        
        anim = GetComponent<Animator>();
        
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.velocity = Vector3.one;
        agent.stoppingDistance = 0.01f;
        agent.radius = 0.005f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        ChangePatrolPoint();
        SetPatrolState();
        SetPatrolDetination();

        lastPosition = transform.position;
        timeSinceLastMove = 0f;
        timeSinceStuckCheck = 0f;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignorando a colisão com o inimigo
        if (collision.gameObject.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>());
        }
    }

    public void SetPatrolState()
    {
        enemyState = EnemyState.PATROL;
        agent.speed *= Random.Range(patrolSpeedMin, patrolSpeedMax);
        playerInChasing = null;

        ChangePatrolPoint();
        SetPatrolDetination();
    }

    public void SetChaseState(Transform playerTransform) {
        playerInChasing = playerTransform;
        enemyState = EnemyState.CHASE;
        agent.speed = chaseSpeed;

        agent.SetDestination(playerTransform.position);
    }

    public EnemyState GetState() => enemyState;

    void Update()
    {

        // if (transform.rotation.y)

        if (enemyState == EnemyState.PATROL)
        {
            HandlePatrolBehavior();
        }
        else if (enemyState == EnemyState.CHASE)
        { 
            // Neste caso o inimigo está em perseguição direta ao player, isso é gerenciado no script EnemyAI 
        }

        CheckIfStuck();
    }

    public void StopEnemy()
    {
        agent.isStopped = true;
        agent.SetDestination(transform.position);

        StartCoroutine(ReturningEnemyToPatrol());
    }

    IEnumerator ReturningEnemyToPatrol()
    {
        yield return new WaitForSeconds(2.0f);
        agent.isStopped = false;
        SetPatrolState();
    }

    public void ReleaseEnemy()
    {
        if (agent.isActiveAndEnabled) agent.isStopped = false;
    }

    private void SetPatrolDetination()
    {
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    private void HandlePatrolBehavior()
    {
        if (!waiting && VerificaSeEstaProximoPatrolPoint())
        {
            waiting = true;
            waitCounter = Random.Range(waitTimeMin, waitTimeMax); // define o tempo de espera antes de mover para o próximo ponto
        }

        if (waiting)
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0f)
            {
                waiting = false;
                ChangePatrolPoint();
                SetPatrolDetination();
            }
        }
    }

    private void ChangePatrolPoint()
    {
        // Escolhe um ponto aleatório diferente do atual para patrulhar
        var tempIndex = Random.Range(0, patrolPoints.Length);
        patrolIndexesToIgnore = FindObjectsOfType<Enemy>().ToList().Where(x => x.gameObject != gameObject).Select(x => x.GetPartrolIndex()).ToList();
        while (tempIndex == currentPatrolIndex || patrolIndexesToIgnore.Contains(tempIndex))
        {
            tempIndex = Random.Range(0, patrolPoints.Length);
        }
        currentPatrolIndex = tempIndex;
    }

    private bool VerificaSeEstaProximoPatrolPoint()
    {
        float distanceToPatrolPoint = Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position);
        return distanceToPatrolPoint <= distanceThreshold;
    }

    private void CheckIfStuck()
    {
        // Atualiza o tempo desde a última verificação de travamento
        if (VerificaSeEstaProximoPatrolPoint()) return;

        timeSinceStuckCheck += Time.deltaTime;

        if (timeSinceStuckCheck >= stuckCheckInterval)
        {
            // Calcula a distância movida desde a última posição conhecida
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);

            if (distanceMoved <= stuckDistanceThreshold)
            {
                timeSinceLastMove += stuckCheckInterval;

                if (timeSinceLastMove >= sutckTimeLimit)
                {
                    // Se o inimigo estiver travado, muda o ponto de patrulha
                    ChangePatrolPoint();
                    SetPatrolDetination();
                    timeSinceLastMove = 0f;
                }
            }
            else
            {
                timeSinceLastMove = 0f;
            }
            // Atualiza a última posição conhecida e reinicia o tempo desde a última verificação de travamento
            lastPosition = transform.position;
            timeSinceStuckCheck = 0f;
        }

    }
}
