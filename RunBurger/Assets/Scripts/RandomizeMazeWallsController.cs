using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class RandomizeMazeWallsController : MonoBehaviour
{

    [Header("Actors Configuration")]
    [SerializeField] private GameObject playerGameObject;
    [SerializeField] private List<GameObject> enemyListGameObject;

    [Header("rRansition settings")]
    [SerializeField] private float proximityRange = 1f;
    [SerializeField] private float minTime = 1f;
    [SerializeField] private float maxTime = 5f;
    [SerializeField] private float disapearingLifeTimeStartSeconds = 4f;
    [SerializeField] private float disapearingLifeTimeEndSeconds = 7f;

    [SerializeField] private float fadeDuration = 0.5f;

    // Private variables
    private List<GameObject> hideableWalls;

    // Start is called before the first frame update
    void Start()
    {
        hideableWalls = new List<GameObject>(GameObject.FindGameObjectsWithTag("Hideable"));
        enemyListGameObject = FindObjectsOfType<Enemy>().ToList().Select(x => x.gameObject).ToList();

        StartCoroutine(ManageWalls());
    }

    IEnumerator ManageWalls()
    {
        while (true)
        {
            // Tempo aleatório entre as alternâncias
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);

            // Obter paredes próximas ao jogador e ao inimigo
            List<GameObject> nearbyWalls = GetNearbyWalls();

            if (nearbyWalls.Count > 0)
            {
                // Verifica se já existe uma parede desabilitada no alcance
                bool hasDisabledWall = false; // nearbyWalls.Exists(wall => !wall.activeSelf);

                if (!hasDisabledWall)
                {
                    // Seleciona uma parede aleatória da lista de paredes próximas
                    int randomIndex = Random.Range(0, nearbyWalls.Count);
                    GameObject wall = nearbyWalls[randomIndex];

                    if (wall.activeSelf)
                    {
                        StartCoroutine(FadeOut(wall));
                    }
                    else
                    {
                        StartCoroutine(FadeIn(wall));
                    }
                }
            }
        }
    }

    List<GameObject> GetNearbyWalls()
    {
        List<GameObject> nearbyWalls = new();

        if (playerGameObject == null || enemyListGameObject == null) return new List<GameObject>();

        foreach (GameObject wall in hideableWalls)
        {

            foreach (GameObject enemy in enemyListGameObject)
            {

                float playerDistance = Vector3.Distance(wall.transform.position, playerGameObject.transform.position);
                float enemyDistance = Vector3.Distance(wall.transform.position, enemy.transform.position);

                if (playerDistance <= proximityRange)
                {
                    nearbyWalls.Add(wall);
                }

                if (enemyDistance <= proximityRange)
                {
                    nearbyWalls.Add(wall);
                }
            }
        }

        return nearbyWalls;
    }

    IEnumerator FadeOut(GameObject wall)
    {
        Material material = wall.GetComponent<Renderer>().material;
        Color startColor = material.color;
        Color endColor = new(startColor.r, startColor.g, startColor.b, 0);
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            material.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        material.color = endColor;
        wall.SetActive(false);
        if (wall.TryGetComponent<NavMeshObstacle>(out var obstacle))
        {
            obstacle.carving = false;
        }

        yield return new WaitForSeconds(Random.Range(disapearingLifeTimeStartSeconds, disapearingLifeTimeEndSeconds));
        StartCoroutine(FadeIn(wall));
    }

    IEnumerator FadeIn(GameObject wall)
    {
        wall.SetActive(true);
        Material material = wall.GetComponent<Renderer>().material;
        Color startColor = new(material.color.r, material.color.g, material.color.b, 0);
        Color endColor = new(material.color.r, material.color.g, material.color.b, 1);
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            material.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        material.color = endColor;
        if (wall.TryGetComponent<NavMeshObstacle>(out var obstacle))
        {
            obstacle.carving = true;
        }
    }


}
