using UnityEngine.AI;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    // Start is called before the first frame update
    void Start() {

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void FixedUpdate() {

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
}
