using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlinkingText : MonoBehaviour
{
    Text textToBlink;
    void Start() {   

        textToBlink = GetComponent<Text>();
        StartBlinking();
    }
    private IEnumerator BlinkText() {

        while (true) {
            switch (textToBlink.color.a.ToString()) {

                case "0":
                    textToBlink.color = new Color(207, 107, 28, 1);
                    yield return new WaitForSeconds(1.0f);
                    break;
                case "1":
                    textToBlink.color = new Color(255, 255, 255, 0);
                    yield return new WaitForSeconds(1.0f);
                    break;
            }
        }        
    }
    void StartBlinking() { 
        
        StopCoroutine("BlinkText");
        StartCoroutine("BlinkText");
    }

    void StopBlinking() {

        StopCoroutine("BlinkText");
    }
}