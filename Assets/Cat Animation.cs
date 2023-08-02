using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAnimation : MonoBehaviour
{
    CatBits catbits = new CatBits();
    // Start is called before the first frame update
    void Start()
    {
        catbits.fetchBones(transform);
        Debug.Log(catbits.Head.name);
    }

    // Update is called once per frame
    void Update()
    {
        catbits.Head.rotation = Quaternion.Euler(LookAtPos(Vector3.zero));
    }

    Vector3 LookAtPos(Vector3 lookPos)
    {
        lookPos = GetMouseWorldPosition(); //temporary measure until we actually have objects to look for
        Vector3 currentRot = catbits.Head.eulerAngles;
        Vector3 currentPos = catbits.Head.position;

        currentRot.y = 90 - (Mathf.Rad2Deg * Mathf.Atan2((lookPos.z - currentPos.z) , (lookPos.x - currentPos.x)));

        return currentRot;
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        Debug.Log("no hit :c");
        return Vector3.zero;
    }
}

public class CatBits
{
    Transform head;
    public Transform Head {get {return head;} private set {head = value;}}
    public void fetchBones(Transform transform)
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
