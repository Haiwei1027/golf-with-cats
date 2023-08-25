using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const int MiddleMouseButton = 2;

    public float zoomSensitivity;
    public float panSensitivity;

    public float angleOfDepression;
    public float cameraDistance;
    public Vector2 sizeRange;
    public float cameraSize;

    private Vector3 prevMousePos;
    private Transform cameraTransform;
    private new Camera camera;
    
    void Start()
    {
        cameraTransform = transform.GetChild(0);
        camera = cameraTransform.GetComponent<Camera>();
        prevMousePos = Vector3.zero;

        enabled = false;
        Resident.onStartGame += () => { enabled = true; };
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
        transform.position = transform.position + transform.TransformVector(mouseChange) * panSensitivity;
    }

    void EndPan()
    {

    }

    void Zoom(float delta)
    {
        cameraSize = Mathf.Clamp(cameraSize + delta * zoomSensitivity, sizeRange.x, sizeRange.y);
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
        if (Input.mouseScrollDelta.y != 0)
        {
            Zoom(Input.mouseScrollDelta.y);
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
