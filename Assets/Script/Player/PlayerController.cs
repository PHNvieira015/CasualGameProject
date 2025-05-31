using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public void WhenPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
         Debug.Log("aaaa")
                ;
        }
    }


}
