using UnityEngine;

public class MatchStartController : MonoBehaviour
{
   void OnCollisionEnter2D(Collision2D other)
   {
        // if (other.gameObject.CompareTag("Player") && GameManager.instance.GetMatchStarted == false)
        // {
        //     // Start the match
        //     GameManager.instance.SetMatchStarted();
        // }
   }
}
