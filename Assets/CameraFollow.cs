using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followTarget;

    [SerializeField] float followHeight;
    [SerializeField] float followDistance;
    [SerializeField] float followSpeed;

    private Vector3 calculateOffset(float height, float depression, float direction)
    {
        float deviation = height / Mathf.Tan(depression * Mathf.Deg2Rad);
        Vector3 focus = new Vector3(-deviation * Mathf.Cos((90 - direction) * Mathf.Deg2Rad), height, -deviation * Mathf.Sin((90 - direction) * Mathf.Deg2Rad));
        return focus;
    }

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 offset = calculateOffset(followHeight, transform.eulerAngles.x, transform.eulerAngles.y);
        Vector3 focus = transform.position - offset;
        float distance = Vector3.Distance(focus,followTarget.position);
        if(distance > followDistance)
        {
            focus = Vector3.Lerp(focus,followTarget.position,Time.deltaTime*followSpeed);
            transform.position = focus + offset;
        }
    }
}
