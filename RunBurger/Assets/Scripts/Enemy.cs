using UnityEngine.AI;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator anim;

    public bool IsStopped => agent.isStopped;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    void FixedUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        if (agent != null)
        {
            agent.SetDestination(target.position);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }

    public void StopEnemy()
    {
        agent.isStopped = true;
    }

    public void ReleaseEnemy()
    {
        if (agent.isActiveAndEnabled) agent.isStopped = false;
    }
}
