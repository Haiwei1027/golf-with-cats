using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Item : ScriptableObject
{

    public ushort prefabId;
    public GameObject previewPrefab;
    public Vector3 spawnOffset;
    public AnimationCurve flingFunction;

    public Sprite itemSprite;

}
