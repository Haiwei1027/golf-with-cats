using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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

    private Vector3 moveVector = Vector3.zero;

    InputMap inputMap;

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

    // public void OnPan(InputValue value)
    // {
    //     Vector2 delta = value.Get<Vector2>();
    //     moveVector.x = delta.x;
    //     moveVector.z = delta.y;
    // }

    public void OnPan(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        moveVector.x = delta.x;
        moveVector.z = delta.y;
        transform.position = transform.position + transform.TransformVector(moveVector) * panSensitivity;
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        float delta = context.ReadValue<float>();
        cameraSize = Mathf.Clamp(cameraSize + delta * zoomSensitivity, sizeRange.x, sizeRange.y);
        cameraTransform.localRotation = Quaternion.Euler(angleOfDepression, 0, 0);
        cameraTransform.localPosition = cameraTransform.localRotation * -Vector3.forward * cameraDistance;
        camera.orthographicSize = cameraSize;
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

    private void Update()
    {
        //TakeInput();
        UpdateCamera();
    }

    // private void FixedUpdate()
    // {
    //     transform.position = transform.position + transform.TransformVector(moveVector) * panSensitivity;
    // }

    private void OnEnable()
    {
        inputMap = new InputMap();
        inputMap.Game.Enable();

        inputMap.Game.Pan.performed += OnPan;
        inputMap.Game.Zoom.performed += OnZoom;
    }

    private void OnDisable()
    {
        inputMap.Game.Disable();
    }
}
