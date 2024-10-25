using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvasVisual : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Configurações de visualização do player")]
    [SerializeField] private Image playerColor;
    [SerializeField] private Text coinsText;

    public void SetPlayerColor(Color color)
    {
        playerColor.color = color;
    }

    public void SetCoinsCount(int coins)
    {
        coinsText.text = coins.ToString();
    }
}
