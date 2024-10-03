using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class StartController : MonoBehaviour
{

    [Header("BGM Audio Settings")]
    [SerializeField] private AudioClip bgmSceneAudioClip;

    private AudioManager audioManager;
    private GameController controller;
    private List<Enemy> enemyList;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        enemyList = FindObjectsOfType<Enemy>().ToList();
        controller = FindObjectOfType<GameController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyList.All(x => x.GameObject().activeSelf)) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            controller.startTime = true;
            SpawnEnemies();
            if (enemyList != null && enemyList.Count > 0) enemyList.ForEach(x => x.gameObject.SetActive(true));
            Debug.Log("Can play the music? " + audioManager.TurnAudioOnOff());
            if (!audioManager.TurnAudioOnOff()) return;
            if (bgmSceneAudioClip != null && !audioManager.IsPlaying()) audioManager.PlayBGM(bgmSceneAudioClip, null);
        }
    }

    void SpawnEnemies()
    {
        var spawnPoints = GameObject.FindGameObjectsWithTag("PointToSpawn").ToList();  // Converte o array para lista

        if (spawnPoints != null && spawnPoints.Count > 0)
        {
            if (enemyList != null && enemyList.Count > 0)
            {
                int lastIndex = -1;  // Armazena o índice do último ponto de spawn utilizado

                foreach (var enemy in enemyList)
                {
                    int randomIndex;

                    // Se for o primeiro inimigo, escolha um índice aleatório sem restrições
                    if (lastIndex == -1)
                    {
                        randomIndex = Random.Range(0, spawnPoints.Count);
                    }
                    else
                    {
                        // Gere um novo índice com diferença de até 4 pontos do último ponto de spawn
                        int minIndex = Mathf.Max(0, lastIndex - 4);  // Garante que não seja menor que 0
                        int maxIndex = Mathf.Min(spawnPoints.Count - 1, lastIndex + 4);  // Garante que não ultrapasse o array

                        randomIndex = Random.Range(minIndex, maxIndex + 1);
                    }

                    // Atualiza a posição do inimigo
                    enemy.transform.position = spawnPoints[randomIndex].transform.position;

                    // Remove o ponto de spawn utilizado da lista
                    spawnPoints.RemoveAt(randomIndex);

                    // Armazena o índice do último ponto de spawn utilizado
                    lastIndex = randomIndex;

                    // Se não houver mais spawn points suficientes, pare o loop
                    if (spawnPoints.Count == 0)
                        break;
                }
            }
        }
    }


}

