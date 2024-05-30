using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    public Enemy enemy;
    public int coins;
    public int totalCoins;
    [SerializeField] float speed;
    [SerializeField] Rigidbody2D rgBody;
    [SerializeField] Animator anim;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] bool isDead = false;

    [Header("Collectable Sound Settings")]
    [SerializeField] private AudioClip starCollectAudioAction;
    [SerializeField] private float starCollectAudioVolume = 0.8f;

    [Header("Death Sound Settings")]
    [SerializeField] private AudioClip deathAudioAction;
    [SerializeField] private float deathAudioVolume = 0.8f;

    private SpriteRenderer sprite;
    private Vector2 _direction;
    private AudioSource playerAudioSource;
    private GameController controller;
    private AudioManager audioManager;
    private CameraShake cameraShake;
    private GlobalController globalController;

    void Start()
    {
        rgBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        playerAudioSource = GetComponent<AudioSource>();
        controller = FindObjectOfType<GameController>();
        audioManager = FindObjectOfType<AudioManager>();
        cameraShake = FindAnyObjectByType<CameraShake>();
        globalController = FindObjectOfType<GlobalController>();

        enemy.gameObject.SetActive(false);
        controller.startTime = false;

        // Reseting the counters
        globalController.ResetCounts();
    }

    public void Update()
    {
        ProcessInput();
    }

    void FixedUpdate()
    {
        CoinsCount();
        OnMove();
    }


    void ProcessInput()
    {
        _direction = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void OnMove()
    {
        if (isDead) return;

        rgBody.MovePosition(rgBody.position + _direction
                    * speed * Time.fixedDeltaTime);

        // Tive que mudar a animação para o som funcionar, estava disparando sons, quando ele não andava.
        if (_direction.sqrMagnitude > 0)
        {
            anim.SetBool("IsRunning", true);
        }
        else
        {
            anim.SetBool("IsRunning", false);
        }

        float targetAngle = 0;

        if (_direction.x > 0 && _direction.y > 0)
        {
            targetAngle = 45;
        }
        else if (_direction.x < 0 && _direction.y > 0)
        {
            targetAngle = 135;
        }
        else if (_direction.x > 0 && _direction.y < 0)
        {
            targetAngle = -45;
        }
        else if (_direction.x < 0 && _direction.y < 0)
        {
            targetAngle = -135;
        }
        else if (_direction.x > 0)
        {
            targetAngle = 0;
        }
        else if (_direction.x < 0)
        {
            targetAngle = 180;
        }
        else if (_direction.y > 0)
        {
            targetAngle = 90;
        }
        else if (_direction.y < 0)
        {
            targetAngle = 270;
        }

        // Essa movimentação faz o personagem mudar de lado mais suavemente
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Exit")
        {
            //controller.coinsCount = coins + totalCoins;
            audioManager.StopPlaying();
            enemy.StopEnemy();
            Invoke(nameof(DestroyPlayerAndEnemy), 0f);
            SceneManager.LoadSceneAsync("WinnerGameOverFire");
        }

        if (collision.gameObject.tag == "ColliderEnemy")
        {
            Dead();
            Invoke(nameof(DestroyPlayerAndEnemy), 1.5f);
            playerAudioSource.PlayOneShot(deathAudioAction, deathAudioVolume);
            StartCoroutine(LoadGameOverFireScene());
        }

        if (collision.gameObject.tag == "Coin")
        {
            playerAudioSource.PlayOneShot(starCollectAudioAction, starCollectAudioVolume);
            Destroy(collision.gameObject);
            coins++;
            // Default value for icon is 1
            globalController.AddCoin(1);
        }
    }

    private void Dead() {
        cameraShake.ShakeCamera();
        isDead = true;
        audioManager.StopPlaying();
        anim.SetBool("IsRunning", false);
    }

    private void DestroyPlayerAndEnemy()
    {
        Destroy(enemy);
        Destroy(gameObject);
    }

    void CoinsCount()
    {
        controller.coinsCount = coins;
        totalCoins = (int)controller.timeCount;
    }

    IEnumerator LoadGameOverFireScene()
    {
        yield return new WaitForSeconds(0.9f);
        SceneManager.LoadSceneAsync("GameOverFire");
    }
}