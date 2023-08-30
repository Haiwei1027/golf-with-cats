using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            focused = !focused;
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
            catch (UnityException e) { }
        }
        
    }
}
