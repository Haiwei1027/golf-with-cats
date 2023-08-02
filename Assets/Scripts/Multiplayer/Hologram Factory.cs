using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HologramFactory : MonoBehaviour
{
    private static GameObject[] prefabs;

    public void Awake()
    {
        prefabs = Resources.LoadAll<GameObject>("Prefabs");
    }

    public static void SpawnByName(string name, Vector3 position, Quaternion rotation)
    {
        int id = Array.FindIndex(prefabs,prefab => prefab.name.Equals(name));
        SpawnById(id, position, rotation);
    }

    public static void SpawnById(int id, Vector3 position, Quaternion rotation)
    {
        HologramTransmitter transmitter = Instantiate(prefabs[id], position, rotation).GetComponent<HologramTransmitter>();
        transmitter.SendCreate(id);
    }

    public static void SpawnByReference(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int id = Array.IndexOf(prefabs, prefab);
        SpawnById(id, position, rotation);
    }
}
