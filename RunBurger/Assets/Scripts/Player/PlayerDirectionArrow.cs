using UnityEngine;

public class PlayerDirectionArrow : MonoBehaviour
{

    [Header("Configurações de direção do player")]
    [SerializeField] private GameObject arrow;

    private Vector2 lastPosition;
    private Player player;

    void Start()
    {
        lastPosition = transform.position;
        player = transform.parent.GetComponent<Player>();
    }

    void Update()
    {
        Vector2 currentPosition = transform.position;
        Vector2 movementDirection = currentPosition - lastPosition;

        if (movementDirection != Vector2.zero)
        {
            arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, player.GetTargetAngle()));
        }

        lastPosition = currentPosition;
    }

}
