using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const int MiddleMouseButton = 2;

    public float angleOfDepression;
    public float cameraDistance;
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

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return Vector3.zero;
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

    public void UpdateCamera()
    {
        if (!cameraTransform)
        {
            cameraTransform = transform.GetChild(0);
            camera = cameraTransform.GetComponent<Camera>();
        }
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
