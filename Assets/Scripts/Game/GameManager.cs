using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
