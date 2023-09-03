using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public class PrefabList : ScriptableObject
{
    [SerializeField] private GameObject[] prefabs;

    public GameObject Get(int index)
    {
        return prefabs[index];
    }

    public GameObject Get(string name)
    {
        return prefabs.FirstOrDefault(a => a.name.Equals(name));
    }
}
