using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonVisualController : MonoBehaviour
{

    [SerializeField] private GameObject firstButton;

    // Start is called before the first frame update
    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            Debug.Log("No selected object");
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }

}
