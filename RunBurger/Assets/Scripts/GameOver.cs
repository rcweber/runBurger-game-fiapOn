using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [Header("Sound settings")]
    [SerializeField] private AudioClip coinAudioClip;
    [SerializeField] private float coinAudioClipVoluyme = 0.5f;
    [SerializeField] private AudioClip bgmWinnerMusic;
    [SerializeField] private float bgmAudioClipVolume = 0.55f;


    [Header("Text Settings")]
    [SerializeField] private Text coinsText;
    [SerializeField] private Text timeText;

    [Header("Countdown settings")]
    [SerializeField] private float waitForSecondsToStarCount = 3f;
    [SerializeField] private float waitForSecondsBetweenCount = 0.05f;

    [Header("Prizes size and measure")]
    [SerializeField] private float prizeForMore75PercentageCoinsCollected = 0.4f;
    [SerializeField] private float priseForMoreEqual30PercentageCoinsCollected = 0.25f;
    [SerializeField] private float priseForLessThan30PercentageCoinsCollected = 0.2f;

    // Private variables
    private float totalTimeRemaining;
    private float timeRemaining;
    private float totalCoincCollected;
    private float totalCoinsInLevel;
    private float finalCoinCount;
    private float coinsPercentageCollected;
    private AudioManager audioManager;

    private GlobalController globalController;

    void Start()
    {
        globalController = FindObjectOfType<GlobalController>();
        audioManager = FindObjectOfType<AudioManager>();

        // Starting the BGM
        audioManager.PlayBGM(bgmWinnerMusic, bgmAudioClipVolume);

        // showing the time and coins of player
        totalCoincCollected = Mathf.CeilToInt(globalController.GetTotalCoins());
        totalTimeRemaining = Mathf.Ceil(globalController.GetTimeLeft());
        timeRemaining = totalTimeRemaining;

        totalCoinsInLevel = globalController.GetTotalCoinsInScene();
        // Calculating totals
        coinsPercentageCollected = totalCoincCollected / totalCoinsInLevel * 100;
        finalCoinCount = totalCoincCollected;

        UpdateText(totalCoincCollected, totalTimeRemaining);

        StartCoroutine(CountDownCoRoutine());
    }

    void RefreschScreen()
    {
        UpdateText(finalCoinCount, timeRemaining);
    }

    void Update()
    {
        RefreschScreen();
    }

    void UpdateText(float coin, float time)
    {
        // showing the current time and coin
        coinsText.text = coin.ToString("F2");
        timeText.text = time.ToString();
    }
    IEnumerator CountDownCoRoutine()
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
                audioManager.PlayOneShot(coinAudioClip, coinAudioClipVoluyme);
                yield return new WaitForSeconds(waitForSecondsBetweenCount);
            }
            else break;
        }

        if (coinsPercentageCollected >= 90f)
        {
            finalCoinCount += Mathf.FloorToInt(totalTimeRemaining * 0.5f);
        }
        RefreschScreen();
        Debug.Log("Total Final de Moedas: " + finalCoinCount.ToString("F2"));
    }

    void AddCoinsPerSecond()
    {
        if (coinsPercentageCollected > 75f)
        {
            finalCoinCount += prizeForMore75PercentageCoinsCollected;
        }
        else if (coinsPercentageCollected >= 30f)
        {
            finalCoinCount += priseForMoreEqual30PercentageCoinsCollected;
        }
        else finalCoinCount += priseForLessThan30PercentageCoinsCollected;
    }

    public void PlayGame()
    {
        audioManager.StopPlaying();
        SceneManager.LoadScene("FirstPhase");
    }
    public void QuitGame()
    {
        audioManager.StopPlaying();
        Application.Quit();
    }
}
