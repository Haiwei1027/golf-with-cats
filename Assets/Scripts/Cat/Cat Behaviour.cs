using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for changing and executing the cat state
/// </summary>
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
        catch (UnityException e) { Debug.LogException(e); }
    }
}
