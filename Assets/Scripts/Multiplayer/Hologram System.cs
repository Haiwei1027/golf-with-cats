using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class used by the resident to coordinate sending and receiving hologram data
/// Also helps with cataloging hologram objects
/// </summary>
public class HologramSystem : MonoBehaviour
{
    private HologramSystem instance;
    public HologramSystem Instance { get { return instance; } 
        private set 
        {
            if (instance != null)
            {
                Debug.LogAssertion("Multiple HologramSystem Instances");
                return;
            }
            instance = value; 
        } 
    }

    [SerializeField] GameObject[] prefabs;

    private List<HologramTransceiver> transceivers;

    public void Awake()
    {
        Instance = this; 
    }

    public static void HandleCreate(ResidentRecord _, Letter letter)
    {

    }

    public static void HandleUpdate(ResidentRecord _, Letter letter)
    {

    }

    public static void HandleDestroy(ResidentRecord _, Letter letter)
    {

    }
}
