using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAnimation : MonoBehaviour
{
    CatBits catbits = new CatBits();


    void Start()
    {
        catbits.FetchBones(transform);
    }

    void Update()
    {
        
    }

    public void LookAtPosition(Vector3 position)
    {
        catbits.Head.rotation = Quaternion.LookRotation(position - transform.position, transform.up);
    }

    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}

public class CatBits
{
    Transform head;
    public Transform Head {get {return head;} private set {head = value;}}
    public void FetchBones(Transform transform)
    {
        Debug.Log(transform.name);
        head = FindRecursive(transform, "Head");
    }

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
