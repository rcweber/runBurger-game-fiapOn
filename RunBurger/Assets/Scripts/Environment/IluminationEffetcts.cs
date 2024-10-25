using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class IluminationEffetcts : MonoBehaviour
{

    [Header("Ilumination Settings")]
    [SerializeField] private Vector2 areaMin;
    [SerializeField] private Vector2 areaMax;
    [SerializeField] private float waitTime = 0.1f;
    [SerializeField] private float startMoveDuration = 1f;
    [SerializeField] private float endMoveDuration = 3f;

    private Light2D light2D;
    private Vector2 targetPosition;
    private bool isMoving = true;
    

    public bool GetActiveStatus() => isMoving;

    // Start is called before the first frame update
    void Start()
    {
        light2D = GetComponent<Light2D>();
        StartCoroutine(MoveLightRandomly());
    }

    IEnumerator MoveLightRandomly()
    {
        while (true)
        {
            // Escolher uma posição aleatória dentro da área
            targetPosition = new Vector2(
                Random.Range(areaMin.x, areaMax.x),
                Random.Range(areaMin.y, areaMax.y)
            );

            yield return MoveToPosition(targetPosition);

            // Esperar um tempo antes de escolher a próxima posição
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator MoveToPosition(Vector2 target)
    {
        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;

        float moveDuration = Random.Range(startMoveDuration, endMoveDuration);
        while (elapsedTime < moveDuration)
        {
            transform.position = Vector2.Lerp(startPosition, target, EaseInOutQuad(elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
    }
}
