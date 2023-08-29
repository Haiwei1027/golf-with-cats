using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = PlayerColour.Get(Resident.Instance.record.ColourId);
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            transform.position = CameraController.GetMouseWorldPosition();
        }
        catch (UnityException e) {}
    }
}
