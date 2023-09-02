using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object for holding data on an item type
/// </summary>
[CreateAssetMenu()]
public class Item : ScriptableObject // TODO Refactor this to be ItemInfo that stores data on an item and have a class Item to be inherited for more complex functionalities
{
    public string displayName;
    public string description;
    public ushort prefabId;
    public GameObject previewPrefab;
    public Vector3 spawnOffset;
    public AnimationCurve flingFunction;

    public Sprite itemSprite;

}
