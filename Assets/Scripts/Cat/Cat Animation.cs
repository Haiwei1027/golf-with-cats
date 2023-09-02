using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Class responsible for animating the cat based on the current cat state
/// </summary>
public class CatAnimation : MonoBehaviour
{
    CatBits catbits = new CatBits();

    public Vector3 lookingAt { get; private set; }

    void Start()
    {
        catbits.FetchBones(transform);
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Method to make the cat's neck and head rotate towards a world position
    /// </summary>
    /// <param name="position">The world position to look at</param>
    public void LookAtPosition(Vector3 position)
    {
        lookingAt = position;
        //params
        Quaternion targetRotation;
        Quaternion targetRotationNeck002;
        Quaternion defaultRotationNeck002 = Quaternion.Euler(catbits.Neck001.rotation.x + 31.891f, catbits.Neck001.rotation.y, catbits.Neck001.rotation.z);

        //target angles
        targetRotation = Quaternion.LookRotation(position - transform.position, transform.up);
        targetRotationNeck002 = targetRotation * Quaternion.AngleAxis(90, Vector3.right);

        //setting
        float angle = Quaternion.Angle(defaultRotationNeck002, targetRotationNeck002);
        float t = Mathf.Clamp01(angle / 180f);
        catbits.Neck002.rotation = Quaternion.Slerp(defaultRotationNeck002, targetRotationNeck002, t);
        catbits.Head.rotation = targetRotation;
    }
}

/// <summary>
/// Class responsible for fetching and storing transforms of cat "bits" for ease of manipulating during procedural animation
/// </summary>
public class CatBits
{
    Transform head;
    Transform neck002;
    Transform neck001;
    public Transform Head {get {return head;} private set {head = value;}}
    public Transform Neck002 {get {return neck002;} private set {neck002 = value;}}
    public Transform Neck001 {get {return neck001;} private set {neck001 = value;}}
    /// <summary>
    /// Searches recursively down the root transform's tree to find transform of each body parts
    /// </summary>
    /// <param name="transform">Root transform of the cat model</param>
    public void FetchBones(Transform transform)
    {
        Debug.Log(transform.name);
        head = FindRecursive(transform, "Head");
        neck002 = FindRecursive(transform, "Neck.002");
        neck001 = FindRecursive(transform, "Neck.001");
    }
    /// <summary>
    /// Searches recursively down the transform's tree to find transform of name
    /// </summary>
    /// <param name="transform">Transform being searched</param>
    public static Transform FindRecursive(Transform transform, string name)
    {
        if (transform.name == name)
        {
            return transform;
        }
        Transform tr = transform.Find(name);
        if (tr != null)
        {
            return tr;
        }
        foreach (Transform child in transform)
        {
            tr = FindRecursive(child, name);
            if (tr != null)
            {
                return tr;
            }
        }
        return null;
    }
    
}
