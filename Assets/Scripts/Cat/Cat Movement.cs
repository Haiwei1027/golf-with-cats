using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class responsible for making the cat move in the world according to the current cat state
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class CatMovement : MonoBehaviour
{
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();   
    }

    void Update()
    {
        if (!PlayerCursor.Focused) { return; }
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out RaycastHit hitInfo, 50))
            {
                agent.SetDestination(hitInfo.point);
            }
        }
    }

    public void FixedUpdate()
    {
        
    }
}
