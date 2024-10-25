using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemiesController : MonoBehaviour
{

    [Header("Configurações de spwan do Inimigo")]
    [Tooltip("Prefab do inimigo")]
    [SerializeField] private GameObject enemyPrefab;
    [Tooltip("Quantidade máxima de inimigos")]
    [SerializeField] private int MaxCountEnemies = 5;

    private Transform[] spawnPoints;
    private List<int> usedSpwnPointsIndex = new();


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(0.01f);
        var spawnPoints = GameObject.FindGameObjectsWithTag("PointToSpawn").ToList();
        for (int i = 0; i < MaxCountEnemies && i < spawnPoints.Count; i++)
        {
            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            while (usedSpwnPointsIndex.Contains(spawnPoints.IndexOf(spawnPoint)))
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            }
            usedSpwnPointsIndex.Add(spawnPoints.IndexOf(spawnPoint));
            var enemyCreated = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.Euler(0f, 0f, 0f));
            enemyCreated.transform.parent = transform;
            enemyCreated.transform.Rotate(0f, 0f, 0f, Space.World);
        }
    }
}
