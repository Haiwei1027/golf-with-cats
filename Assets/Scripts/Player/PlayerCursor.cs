using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class responsible for managing the cursor UI and keep track if the game is in focus
/// </summary>
public class PlayerCursor : MonoBehaviour
{

    private static bool focused = true;
    public static bool Focused { get { return focused; } }

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = PlayerColour.Get(Resident.Instance.record.ColourId);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame || Gamepad.current.startButton.wasPressedThisFrame)
        {
            focused = !focused;
            Debug.Log("Focus");
        }
        Cursor.visible = !focused;
        Cursor.lockState = focused ? CursorLockMode.Confined : CursorLockMode.None;
        if (focused)
        {
            try
            {
                transform.position = CameraController.GetMouseWorldPosition();
                transform.position += transform.forward * -5f;
            }
            catch (UnityException e) { Debug.LogException(e); }
        }
        
    }
}
