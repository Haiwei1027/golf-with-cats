using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemSpawner : MonoBehaviour
{

    Item selectedItemPrefab;
    GameObject spawnedItem;
    Rigidbody spawnedItemPhysics;
    Vector3 spawnedItemBase;
    bool flinging;
    public void setSelectedItem(Item item)
    {
        selectedItemPrefab = item;
    }

    private const int LeftMouseButton = 1;
    // Start is called before the first frame update
    void Start()
    {
        setSelectedItem(new Item(1));
    }

    // Update is called once per frame
    void Update()
    {
        TakeInput();
    }

    void TakeInput()
    {
        if (Input.GetMouseButtonDown(LeftMouseButton))
        {
            Debug.Log("started spawn");
            startSpawn();
        }

        if ((Input.GetMouseButtonUp(LeftMouseButton)) && flinging)
        {
            Debug.Log("Flinging");
            fling();
        }
    }

    void startSpawn()
    {
        if (selectedItemPrefab.Equals(null)) {return;}

        spawnedItem = HologramSystem.Instantiate(selectedItemPrefab.id);
        spawnedItemPhysics = spawnedItem.GetComponent<Rigidbody>();

        spawnedItemBase = CameraController.GetMouseWorldPosition();
        spawnedItem.transform.position = spawnedItemBase + Vector3.up;

        spawnedItemPhysics.isKinematic = true;
        spawnedItemPhysics.detectCollisions = false;
        flinging = true;
        
    }

    void fling()
    {
        Vector3 flingVector = spawnedItemBase - CameraController.GetMouseWorldPosition();

        flingVector *= 3;

        spawnedItemPhysics.isKinematic = false;
        spawnedItemPhysics.detectCollisions = true;

        spawnedItemPhysics.AddForce(flingVector, ForceMode.VelocityChange);

        flinging = false;
    }
}
