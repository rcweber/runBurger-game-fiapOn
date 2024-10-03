using UnityEngine;

public class InputManager : MonoBehaviour
{   
   private PlayerControls playerControls;

    public Vector2 Movement => playerControls.GamePlay.Movement.ReadValue<Vector2>();

   public InputManager()
   {
     playerControls = new PlayerControls();
     playerControls.GamePlay.Enable();
   }
}
