using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static event Action OnEscapePressed;
    public static event Action OnRotatePressed;
    public static event Action OnRightMouseButtonPressed;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapePressed?.Invoke();
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            OnRotatePressed?.Invoke();
        }

        if (Input.GetMouseButtonDown(0)) {
            OnRightMouseButtonPressed?.Invoke();
        }
    }
}
