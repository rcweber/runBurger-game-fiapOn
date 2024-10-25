using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    public List<Enemy> enemyList;
    public int coins;
    public int totalCoins;
    [SerializeField] float normalSpeed;
    [SerializeField] Rigidbody2D rgBody;
    [SerializeField] Animator anim;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] bool isDead = false;
    [SerializeField] private bool isBlocked = false;

    [Header("Collectable Sound Settings")]
    [SerializeField] private AudioClip starCollectAudioAction;
    [SerializeField] private float starCollectAudioVolume = 0.8f;

    [Header("Death Sound Settings")]
    [SerializeField] private AudioClip deathAudioAction;
    [SerializeField] private AudioClip OutSoundAction;
    [SerializeField] private float deathAudioVolume = 0.8f;
    [SerializeField] private AudioClip hurtAudioAction;

    [Header("Player Direction Arrow and base circle")]
    [SerializeField] private SpriteRenderer arrow;
    [SerializeField] private SpriteRenderer circle;

    [Header("Configurações de vida do player")]
    [SerializeField] private float PowerLife = 9.99f;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private float invulnerableTime = 1.5f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("ConfiguraçÕes de boost do Player")]
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float boostDuration = 2f;
    [SerializeField] private float boostCooldown = 5f;

    private SpriteRenderer sprite;
    private Vector2 _direction;
    private AudioSource playerAudioSource;
    private GameController controller;
    private AudioManager audioManager;
    private CameraShake cameraShake;
    private GlobalController globalController;
    private Gamepad gamePad;
    private float targetAngle = 0f;
    private float lastRotationAngle = 0f;
    private PlayerInput playerInput;
    private int PlayerIndex;
    private bool isInvulnerable = false;
    private Color playerColor;
    private float currentSpeed;
    private bool isBoosting = false;
    private bool isInCooldown = false;
    private float boostTimer;
    private float cooldownTimer;

    public float GetTargetAngle() => targetAngle;
    public PlayerInput GetPlayerDeviceInput() => playerInput;
    public int GetPlayerIndex() => PlayerIndex;
    public float GetPowerLife() => PowerLife;
    public float GetBoostDuration() => boostDuration;
    public bool GetIsBoosting() => isBoosting;
    public bool GetIsInCooldown() => isInCooldown;
    public float GetBoostTimer() => boostTimer;
    public float GetBoostCooldownTimer() => cooldownTimer;
    public float GetBoostCooldown() => boostCooldown;


    public void SetPlayerDeviceInput(PlayerInput input) => playerInput = input;
    public void SetPlayerIndex(int index) => PlayerIndex = index;
    public void GetPlayerColor(Color color) => playerColor = color;
    public bool GetPlayerIsAlive() => !isDead;

    private List<Enemy> enemiesInChase = new();

    public event EventHandler<OnPlayerHurtEventArgs> OnPlayerHurt;
    public event EventHandler OnPlayerDied;
    public event EventHandler<OnPlayerDoSomethingEventArgs> OnPlayerGetCoin;

    public event EventHandler<OnPlayerDoSomethingEventArgs> OnPleyerExit;

    public class OnPlayerDoSomethingEventArgs : EventArgs
    {
        public float CoinAmount;
        public int playerIndex;
    }

    public class OnPlayerHurtEventArgs : EventArgs
    {
        public int CurrentLife;
    }

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
        enemyList = FindObjectsOfType<Enemy>().ToList();
        currentSpeed = normalSpeed;

        //enemyList.ForEach(x => x.gameObject.SetActive(false));
        if (controller != null) controller.startTime = false;

        // Reseting the counters
        if (globalController != null) globalController.ResetCounts();
    }

    public void FixedUpdate()
    {
        OnMove();
    }

    public void SetMovement(CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>();
    }


    public void SetPauseCallbackContext(CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.instance.PauseGame();
        }
    }

    public void StartBoost(CallbackContext context)
    {
        if (GameManager.instance.IsPaused && !GameManager.instance.GetMatchStarted) return;

        if (context.performed && !isBoosting && !isInCooldown)
        {
            isBoosting = true;
            boostTimer = boostDuration;
            currentSpeed = normalSpeed * boostMultiplier;
        }
    }

    private void Update()
    {
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0)
            {
                EndBoost();
            }
        }

        if (isInCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                isInCooldown = false;
            }
        }
    }

    private void EndBoost()
    {
        isBoosting = false;
        currentSpeed = normalSpeed;

        // Inicia o cooldown após o boost terminar
        isInCooldown = true;
        cooldownTimer = boostCooldown;
    }

    private void OnMove()
    {
        if (isDead || isBlocked) return;

        _direction.Normalize();

        // Moving the player
        rgBody.velocity = _direction * currentSpeed;

        // Tive que mudar a animação para o som funcionar, estava disparando sons, quando ele não andava.
        if (_direction.sqrMagnitude > 0)
        {
            anim.SetBool("IsRunning", true);

            if (_direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                lastRotationAngle = angle;

                Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
        else
        {
            anim.SetBool("IsRunning", false);

            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, lastRotationAngle));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Exit")
        {
            PlayerExit();
        }

        if (collision.gameObject.tag == "ColliderEnemy")
        {
            var enemy = collision.gameObject.GetComponentInParent<Enemy>();
            PlayerHit(enemy);
        }

        if (collision.gameObject.tag == "Coin")
        {
            playerAudioSource.PlayOneShot(starCollectAudioAction, starCollectAudioVolume);
            Destroy(collision.gameObject);
            coins++;
            OnPlayerGetCoin?.Invoke(this, new OnPlayerDoSomethingEventArgs { CoinAmount = 1 });
        }
    }

    private void PlayerExit()
    {
        isBlocked = true;
        anim.SetBool("IsRunning", false);
        rgBody.velocity = Vector2.zero;
        playerAudioSource.PlayOneShot(OutSoundAction, deathAudioVolume);
        OnPleyerExit?.Invoke(this, new OnPlayerDoSomethingEventArgs { CoinAmount = coins, playerIndex = PlayerIndex });
        Invoke(nameof(DestroyPlayer), 1.1f);
    }

    private void Dead()
    {
        isDead = true;
        rgBody.velocity = Vector2.zero;
        cameraShake.ShakeCamera();
        anim.SetBool("IsRunning", false);
        OnPlayerDied?.Invoke(this, EventArgs.Empty);
        playerAudioSource.PlayOneShot(deathAudioAction, deathAudioVolume);
        Invoke(nameof(DestroyPlayer), 1.1f);
    }

    public void DestroyPlayer()
    {
        // Destroy(gameObject);
        playerInput.user.UnpairDevices();
        playerInput.enabled = false;
        PlayerInputManager.instance.DisableJoining();
        gameObject.SetActive(false);
    }

    void CoinsCount()
    {
        if (controller != null) controller.coinsCount = coins;
        if (controller != null) totalCoins = (int)controller.timeCount;
    }

    IEnumerator LoadGameOverFireScene()
    {
        yield return new WaitForSeconds(0.9f);
        SceneManager.LoadSceneAsync("GameOverFire");
    }

    public void SetGamePad(Gamepad playerGamePad)
    {
        gamePad = playerGamePad;
    }

    public void SetEnemyOnChasing(Enemy enemy)
    {
        enemiesInChase.Add(enemy);
    }

    public void SetEnemyOnStopChasing(Enemy enemy)
    {
        enemiesInChase.Remove(enemy);
    }

    public void SetPlayerColor(Color color)
    {
        arrow.color = color;
        circle.color = color;
        playerColor = color;
    }

    public Color GetPlayerColor() => arrow.color;
    public int GetCollectedCoins() => coins;

    private void PlayerHit(Enemy enemy)
    {
        if (!isInvulnerable)
        {
            PowerLife -= enemy.GetEnemyDamage();
            OnPlayerHurt?.Invoke(this, new OnPlayerHurtEventArgs { CurrentLife = (int)PowerLife });

            Debug.Log("Power Life: " + PowerLife);

            if (PowerLife <= 0)
            {
                enemy.StopEnemy();
                Dead();
            }
            else
            {
                // Aqui tem que fazer o player piscar quando for atingido
                StartCoroutine(BlinkAndRecover());
                playerAudioSource.PlayOneShot(hurtAudioAction, deathAudioVolume);
            }
        }
    }

    private IEnumerator BlinkAndRecover()
    {
        isInvulnerable = true;
        float timer = 0;
        while (timer < invulnerableTime)
        {
            Debug.Log("Está invulnerável: " + isInvulnerable);
            playerSprite.enabled = !playerSprite.enabled;
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }
        playerSprite.enabled = true;
        isInvulnerable = false;
        Debug.Log("Está invulnerável: " + isInvulnerable);
    }
}