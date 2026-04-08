using UnityEngine;
using UnityEngine.InputSystem;

public class GameMaster : MonoBehaviour
{
    [SerializeField] GameObject settingCanvas;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            settingCanvas.SetActive(true);
    }
}
