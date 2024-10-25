using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float totalCoints = 0f;
    public Color playerColor;
    public int playerIndex;
    public bool Exited = false;
    public bool PlayerDied = false;
    public int PlayerCurrentLife = 0;
    public float PlayerCurrentTime = 0f;
}
