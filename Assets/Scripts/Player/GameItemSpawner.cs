using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemSpawner : MonoBehaviour
{

    Item selectedItem { get { return Inventory.Instance.SelectedItem; } }
    

    private const int LeftMouseButton = 0;
    private const int RightMouseButton = 1;

    private GameObject previewObject;

    void Start()
    {
        Inventory.Instance.onSelect += OnSelect;
    }

    private void OnDestroy()
    {
        Inventory.Instance.onSelect -= OnSelect;
    }

    public void OnSelect()
    {
        if (!enabled) return;
        if (selectedItem != null)
        {
            if (previewObject != null) { Destroy(previewObject); }
            if (selectedItem.previewPrefab == null) { return; }

            previewObject = Instantiate(selectedItem.previewPrefab);
        }
        else
        {
            Destroy(previewObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedItem == null) { return; }
        TakeInput();
    }

    void TakeInput()
    {
        if (Input.GetMouseButtonDown(RightMouseButton))
        {
            StartCoroutine(SpawnSelected());
        }
    }

    IEnumerator SpawnSelected()
    {
        if (selectedItem != null) yield break;

        Vector3 spawnBase;
        try
        {
            spawnBase = CameraController.GetMouseWorldPosition();
        } catch (UnityException e)
        {
            Debug.LogError(e);
            yield break;
        }

        if (selectedItem.previewPrefab != null) preview = Instantiate(selectedItem.previewPrefab);
        Vector3 flingVector = Vector3.zero;

        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        while (!Input.GetMouseButtonUp(RightMouseButton)) 
        {

            flingVector = spawnBase - CameraController.GetMouseWorldPosition();

            for (int i=0;i<2;i++) yield return waitForEndOfFrame;
        }

        if (preview != null) Destroy(preview);

        GameObject spawned = HologramSystem.Instantiate(selectedItem.prefabId, spawnBase + selectedItem.spawnOffset, Quaternion.LookRotation(-flingVector,Vector3.up));
        Rigidbody spawnedPhysics = spawned.GetComponent<Rigidbody>();

        flingVector = flingVector.normalized * selectedItem.flingFunction.Evaluate(flingVector.magnitude);

        spawnedPhysics.velocity = flingVector;

        Inventory.Instance.Used(selectedItem);
    }
}
