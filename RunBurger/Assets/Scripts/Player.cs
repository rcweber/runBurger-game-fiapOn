using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Rigidbody2D rgBody;
    [SerializeField] Animator anim; 
    private Vector2 moveDirection;
    float inputHorizontal;
    float inputVertical;
    bool facingUp = true;
    bool facingRight = true;
    [SerializeField] string direction = "";
    [SerializeField] string lastMovingDiretion = "";
    [SerializeField] Enemy enemy;
    private SpriteRenderer sprite;


    void Start() {

        rgBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        sprite = GetComponent<SpriteRenderer>();
    }
    void FixedUpdate() {

        Move();
        ProcessInput();
    }

    void ProcessInput() {

        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        moveDirection = new Vector2(inputHorizontal, inputVertical).normalized;

        if (inputHorizontal < 0 && facingRight) {

            //HorizontalFlip();
        }

        if (inputHorizontal > 0 && !facingRight) {

            //HorizontalFlip();
        }

        if (inputVertical < 0 && facingUp) {

            //VerticalFlip();
        }

        if (inputVertical > 0 && !facingUp) {

            //VerticalFlip();
        }
    }

    void HorizontalFlip() {
    
        facingRight = !facingRight;
        transform.Rotate(0, -90, 0);
    }

    void VerticalFlip() {

        facingUp = !facingUp;
        transform.Rotate(180, 0, 0);
    }

    void Move() {

        rgBody.velocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);

        //if (Input.GetAxis("Horizontal") != 0
        //    || Input.GetAxis("Vertical") != 0) {

        //    anim.SetBool("isWalking", true);
        //}
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
