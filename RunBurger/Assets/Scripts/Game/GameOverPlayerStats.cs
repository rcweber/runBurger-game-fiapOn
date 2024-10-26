#nullable enable
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPlayerStats : MonoBehaviour
{
    [Header("Sound settings")]
    [SerializeField] private AudioClip coinAudioClip;
    [SerializeField] private float coinAudioClipVoluyme = 0.5f;
    
    [Header("Text Settings")]
    [SerializeField] private Text coinsText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text bonusText;

    [Header("Countdown settings")]
    [SerializeField] private float waitForSecondsToStarCount = 3f;
    [SerializeField] private float waitForSecondsBetweenCount = 0.05f;
    [SerializeField] private float waitForSecondsToStartCountBonus = 0.5f;

    [Header("Prizes size and measure")]
    [SerializeField] private float prizeForMore75PercentageCoinsCollected = 0.4f;
    [SerializeField] private float prizeForMoreEqual30PercentageCoinsCollected = 0.25f;
    [SerializeField] private float prizeForLessThan30PercentageCoinsCollected = 0.10f;
    [SerializeField] private float prizeCoefficientForBonusfor90PercenteCollectedCouins = 0.5f;
    
    [Header("Configurações de visualização do player")]
    [SerializeField] private Image playerColor;

    // Private variables
    public float totalTimeRemaining;
    public float timeRemaining;
    public float totalCoincCollected;
    public float totalCoinsInLevel;
    public float finalCoinCount;
    public float bonusTotal = 0f;
    public float bonusRemaining = 0f;
    public float coinsPercentageCollected;

    private PlayerCanvasVisual? playerCanvasVisual = null;
   
    void Start()
    {
        playerCanvasVisual = GetComponent<PlayerCanvasVisual>();
    }

    public void SetPlayerStats(PlayerData playerData, Color? color = null) {

        if (playerColor != null) playerColor.color = color ?? new Color(0.13f, 0.57f, 0.16f);

         // showing the time and coins of player
        totalCoincCollected = Mathf.CeilToInt(playerData.totalCoints);
        totalTimeRemaining = Mathf.Ceil( playerData.PlayerCurrentTime);
        timeRemaining = totalTimeRemaining;

        totalCoinsInLevel = GlobalController.instance != null ? GlobalController.instance.GetTotalCoinsInScene() : 0f;
        // Calculating totals
        coinsPercentageCollected = totalCoincCollected / totalCoinsInLevel * 100;
        finalCoinCount = totalCoincCollected;

        // Bonus calculation
        if (coinsPercentageCollected >= 90f)
        {
            bonusTotal = Mathf.FloorToInt(totalTimeRemaining * prizeCoefficientForBonusfor90PercenteCollectedCouins);
            bonusRemaining = bonusTotal;
        }
        UpdateText(totalCoincCollected, totalTimeRemaining, bonusTotal);

        StartCoroutine(CountDownTimeLeftCalculationCoRoutine());
    }

    void RefreschScreen()
    {
        UpdateText(finalCoinCount, timeRemaining, bonusRemaining);
    }

    void Update()
    {
        RefreschScreen();
    }

    void UpdateText(float coin, float time, float bonus)
    {
        // showing the current time and coin
        if (coinsText != null) coinsText.text = coin.ToString("F2");
        if (bonusText != null) bonusText.text = ((int)bonus).ToString();
        if (timeText != null) timeText.text = time.ToString();
    }
    IEnumerator CountDownTimeLeftCalculationCoRoutine()
    {
        yield return new WaitForSeconds(waitForSecondsToStarCount);

        float timeElapsed = 0f;
        while (timeElapsed < totalTimeRemaining)
        {
            if (timeRemaining > 0)
            {
                timeRemaining--;
                timeElapsed += 1f;

                AddCoinsPerSecond();
                RefreschScreen();
                AudioManager.instance.PlaySFX(coinAudioClip, coinAudioClipVoluyme);
                yield return new WaitForSeconds(waitForSecondsBetweenCount);
            }
            else break;
        }
        RefreschScreen();

        if (bonusTotal > 0) StartCoroutine(CountDownBonusCalculationCoRoutine());
    }

    IEnumerator CountDownBonusCalculationCoRoutine()
    {
        yield return new WaitForSeconds(waitForSecondsToStartCountBonus);

        while (bonusRemaining >= 0)
        {
            if (bonusRemaining > 0)
            {
                bonusRemaining--;

                finalCoinCount++;
                RefreschScreen();
                AudioManager.instance.PlaySFX(coinAudioClip, coinAudioClipVoluyme);
                yield return new WaitForSeconds(waitForSecondsBetweenCount);
            }
            else break;
            RefreschScreen();
        }
    }

    void AddCoinsPerSecond()
    {
        if (coinsPercentageCollected >= 75f)
        {
            finalCoinCount += prizeForMore75PercentageCoinsCollected;
        }
        else if (coinsPercentageCollected >= 30f)
        {
            finalCoinCount += prizeForMoreEqual30PercentageCoinsCollected;
        }
        else finalCoinCount += prizeForLessThan30PercentageCoinsCollected;
    }

    public void GoToMainMenu() {
         if (AudioManager.instance != null) AudioManager.instance.StopPlaying();
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayGame()
    {
        if (AudioManager.instance != null) AudioManager.instance.StopPlaying();
        SceneManager.LoadScene("FirstPhase_BattleArena");
    }
    public void QuitGame()
    {
        if (AudioManager.instance != null) AudioManager.instance.StopPlaying();
        Application.Quit();
    }
}
