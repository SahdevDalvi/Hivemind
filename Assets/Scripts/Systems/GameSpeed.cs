using UnityEngine;
using UnityEngine.InputSystem;

public class GameSpeed : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
            Time.timeScale = 1f;   // normal

        if (Keyboard.current.wKey.wasPressedThisFrame)
            Time.timeScale = 3f;   // fast

        if (Keyboard.current.eKey.wasPressedThisFrame)
            Time.timeScale = 10f;  // ultra fast
    }
}
