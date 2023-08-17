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
        catAnimation.LookAtPosition(catAnimation.GetMouseWorldPosition());
    }
}
