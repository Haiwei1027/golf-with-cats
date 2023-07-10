using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const int MiddleMouseButton = 2;

    [Range(0,90)]
    public float angleOfDepression;
    [Range(0,10)]
    public float cameraDistance;
    [Range(0.1f, 10)]
    public float cameraSize;

    private Vector3 prevMousePos;
    private Transform cameraTransform;
    private new Camera camera;
    
    void Start()
    {
        cameraTransform = transform.GetChild(0);
        camera = cameraTransform.GetComponent<Camera>();
        prevMousePos = Vector3.zero;
    }

    void StartPan()
    {
        //no code here as freelook is the only mode
    }

    void Pan()
    {
        Vector3 mouseChange = Vector3.zero;
        mouseChange.x = prevMousePos.x - Input.mousePosition.x;
        mouseChange.z = prevMousePos.y - Input.mousePosition.y;
        transform.position = transform.position + mouseChange * 0.01f;
    }

    void EndPan()
    {
        
    }

    void TakeInput()
    {
        if (Input.GetMouseButtonDown(MiddleMouseButton))
        {
            StartPan();
        }
        else if (Input.GetMouseButton(MiddleMouseButton))
        {
            Pan();
        }
        else if (Input.GetMouseButtonUp(MiddleMouseButton))
        {
            EndPan();
        }

        prevMousePos = Input.mousePosition;
    }

    void UpdateCamera()
    {
        cameraTransform.localRotation = Quaternion.Euler(angleOfDepression, 0, 0);
        cameraTransform.localPosition = cameraTransform.localRotation * -Vector3.forward * cameraDistance;
        camera.orthographicSize = cameraSize;
    }

    void Update()
    {
        TakeInput();
        UpdateCamera();
    }
}
