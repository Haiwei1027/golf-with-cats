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

    public float zoomSensitivity;
    public float panSensitivity;

    public float angleOfDepression;
    public float cameraDistance;
    public Vector2 sizeRange;
    public float cameraSize;

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

    public void OnPan(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        moveVector.x = delta.x;
        moveVector.z = delta.y;
        
        //camera.orthographicSize/sizeRange.y is used to make pan speed relative to zoom level 
        transform.position = transform.position + transform.TransformVector(moveVector) * panSensitivity * (camera.orthographicSize/sizeRange.y);
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
        cameraTransform.localRotation = Quaternion.Euler(angleOfDepression, 0, 0);
        cameraTransform.localPosition = cameraTransform.localRotation * -Vector3.forward * cameraDistance;
        camera.orthographicSize = cameraSize;
    }

    public void cameraMovement()
    {
        Vector2 delta = inputMap.Game.Pan.ReadValue<Vector2>();
        moveVector.x = delta.x;
        moveVector.z = delta.y;
        
        //camera.orthographicSize/sizeRange.y is used to make pan speed relative to zoom level 
        transform.position = transform.position + transform.TransformVector(moveVector) * panSensitivity * (camera.orthographicSize/sizeRange.y);
    }

    private void Update()
    {
        //TakeInput();
        UpdateCamera();
        var keyboard = Keyboard.current;
        if (keyboard != null)
            cameraMovement();
    }

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
