using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemSpawner : MonoBehaviour
{

    int selectedId;
    Item selectedItem;
    

    private const int LeftMouseButton = 0;
    private const int RightMouseButton = 1;

    public float maxDragDist;
    public LineRenderer dragLine;

    private GameObject previewObject;
    private Vector3 spawnBase;
    private Plane spawnPlane;
    private Vector3 flingVector;
    private bool spawning;

    void Start()
    {
        Inventory.Instance.onSelect += OnSelect;
    }

    private void OnDestroy()
    {
        Inventory.Instance.onSelect -= OnSelect;
    }

    public void OnSelect(int id)
    {
        if (!enabled) return;

        selectedId = id;
        selectedItem = Inventory.Instance.BorrowItem(id);

        if (selectedItem != null)
        {
            if (previewObject != null) { Destroy(previewObject); }
            if (selectedItem.previewPrefab == null) { return; }

            previewObject = Instantiate(selectedItem.previewPrefab);
            Debug.LogAssertion("Spawned Preview");
        }
        else
        {
            Destroy(previewObject);
        }
    }

    public void OnUnselect()
    {
        if (previewObject == null) { return; }
        Destroy(previewObject );
    }

    // Update is called once per frame
    void Update()
    {
        TakeInput();
        if (previewObject != null)
        {
            if (!spawning)
            {
                if (selectedItem != null)
                {
                    try
                    {
                        previewObject.transform.position = CameraController.GetMouseWorldPosition() + selectedItem.spawnOffset;
                    }catch(UnityException e) { }
                }
            }
        }
    }

    void TakeInput()
    {
        if (Input.GetMouseButtonDown(LeftMouseButton))
        {
            CancelSpawn();
        }
        if (Input.GetMouseButtonDown(RightMouseButton))
        {
            StartSpawn();
        }
        if (Input.GetMouseButton(RightMouseButton))
        {
            Spawning();
        }
        if (Input.GetMouseButtonUp(RightMouseButton))
        {
            EndSpawn();
        }
    }

    private void CancelSpawn()
    {
        Inventory.Instance.ReturnItem(selectedId);
        CleanUp();

    }

    private void StartSpawn()
    {
        if (spawning) { CancelSpawn(); return; }
        if (selectedItem == null) return;
        try
        {
            spawnBase = CameraController.GetMouseWorldPosition();
        }
        catch (UnityException e)
        {
            
            return;
        }
        Debug.LogAssertion("Start Spawn");

        spawnPlane = new Plane(Vector3.up, spawnBase);
        dragLine.gameObject.SetActive(false);
        spawning = true;
    }

    private void Spawning()
    {
        if (!spawning) { return; }
        Ray mouseRay = CameraController.GetMouseRay();
        spawnPlane.Raycast(mouseRay, out float dist);
        Vector3 planarMousePosition = mouseRay.origin + mouseRay.direction * dist;
        dist = Vector3.Distance(planarMousePosition, spawnBase);
        planarMousePosition = Vector3.Lerp(spawnBase, planarMousePosition, Mathf.Clamp01(maxDragDist / dist));
        flingVector = spawnBase - planarMousePosition;
        dragLine.transform.position = spawnBase;
        dragLine.SetPosition(1, new Vector3(flingVector.x, flingVector.z, dragLine.GetPosition(1).z));

    }

    private void EndSpawn()
    {
        if (!spawning) { return; }
        Debug.LogAssertion("End Spawn");
        GameObject spawned = HologramSystem.Instantiate(selectedItem.prefabId, spawnBase + selectedItem.spawnOffset, Quaternion.LookRotation(-flingVector, Vector3.up));

        flingVector = flingVector.normalized * selectedItem.flingFunction.Evaluate(flingVector.magnitude / maxDragDist);
        Debug.LogAssertion(flingVector.magnitude);
        spawned.GetComponent<Rigidbody>().velocity = flingVector;

        Inventory.Instance.Used(selectedId);
        CleanUp();   
    }

    private void CleanUp()
    {
        selectedItem = null;
        if (previewObject != null) Destroy(previewObject);
        dragLine.gameObject.SetActive(false);
        spawning = false;
    }
}
