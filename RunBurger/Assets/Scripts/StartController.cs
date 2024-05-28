using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartController : MonoBehaviour
{

    [Header("BGM Audio Settings")]
    [SerializeField] private AudioClip bgmSceneAudioClip;

    private AudioManager audioManager;
    private GameController controller;
    private Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        enemy = FindObjectOfType<Enemy>();
        controller = FindObjectOfType<GameController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
         if (collision.gameObject.CompareTag("Player"))
        {
            controller.startTime = true;
            if (enemy != null)  enemy.gameObject.SetActive(true);
            if (bgmSceneAudioClip != null && !audioManager.IsPlaying()) audioManager.PlayBGM(bgmSceneAudioClip);
        }
    }
   
}

