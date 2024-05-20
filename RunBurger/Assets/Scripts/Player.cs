using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Rigidbody2D rgBody;
    [SerializeField] Animator anim; 
    [SerializeField] Enemy enemy;
    private SpriteRenderer sprite;


    void Start() {

        rgBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        sprite = GetComponent<SpriteRenderer>();
    }
    void FixedUpdate() {

        ProcessInput();
    }

    void ProcessInput() {

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
        
        anim.SetFloat("Horizontal", movement.x);
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Speed", movement.magnitude);

        transform.position = transform.position + movement * speed * Time.deltaTime;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Exit")
        {

            anim.SetBool("winner", true);
            anim.Play("Victory");
            Destroy(enemy);
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "ColliderEnemy")
        {
            sprite.color = Color.black;            
            Invoke("DestroyPlayerAndEnemy", 1.5f);
            SceneManager.LoadScene("GameOverFire");
        }
    }

    private void DestroyPlayerAndEnemy() {

        Destroy(enemy);
        Destroy(gameObject);
    }
}
