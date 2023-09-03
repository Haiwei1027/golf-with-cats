using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for controlling the camera based on player input and game state
/// </summary>
public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    public static CameraController Instance { get { return instance; } }

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

    public static Camera Camera { get { return instance.camera; } }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        cameraTransform = transform.GetChild(0);
        camera = cameraTransform.GetComponent<Camera>();
        prevMousePos = Vector3.zero;

        enabled = false;
        Resident.onStartGame += () => { enabled = true; };
    }

    /// <summary>
    /// Casts a ray using the mouse position from the camera to the world
    /// </summary>
    /// <returns>position the mouse is hovering over</returns>
    /// <exception cref="UnityException">the mouse isn't hovering over the world</exception>
    public static Vector3 GetMouseWorldPosition() 
    {
        Ray ray = Instance.camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        throw new UnityException("Mouse over void");
    }

    public static Ray GetMouseRay()
    {
        return Instance.camera.ScreenPointToRay(Input.mousePosition);
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

        Vector3 wasdDelta = new Vector3(Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0), 0, Input.GetKey(KeyCode.S) ? -1 : (Input.GetKey(KeyCode.W) ? 1 : 0));
        transform.position = transform.position + transform.TransformVector(wasdDelta.normalized) * panSensitivity;
    }

    public void UpdateCamera()
    {
        if (!cameraTransform) // TODO remove redundant code
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
        //TakeInput();
        UpdateCamera();
    }
}
