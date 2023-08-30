using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatBehaviour : MonoBehaviour
{
    private CatAnimation catAnimation;

    private void Start()
    {
        catAnimation = GetComponent<CatAnimation>();
    }

    private void Update()
    {
        if (!PlayerCursor.Focused) return;
        try
        {
            catAnimation.LookAtPosition(CameraController.GetMouseWorldPosition());
        }
        catch (UnityException e) { }
    }
}
