using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 prevMousePos;
    [SerializeField] private Transform freeLookTarget;
    
    // Start is called before the first frame update
    void Start()
    {
        prevMousePos = Vector3.zero;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            GetComponent<CameraFollow>().followTarget = freeLookTarget;
        }
        
        if (Input.GetMouseButton(2))
        {   
            Vector3 mouseChange = Vector3.zero;
            mouseChange.x = prevMousePos.x - Input.mousePosition.x;
            mouseChange.z = prevMousePos.y - Input.mousePosition.y;
            freeLookTarget.position = freeLookTarget.position + mouseChange*0.01f;
        }

        prevMousePos = Input.mousePosition;
    }
}
