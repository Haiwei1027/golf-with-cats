using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Singleton class used by the resident to coordinate sending and receiving hologram data
/// Also helps with cataloging hologram objects
/// </summary>
public class HologramSystem : MonoBehaviour
{
    private static HologramSystem instance;
    public static HologramSystem Instance { get { return instance; } 
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

    private int tickCounter;

    public void Awake()
    {
        Instance = this; 
    }

    public void Start()
    {
        transceivers = new List<HologramTransceiver>();
    }

    public void FixedUpdate()
    {
        int i=0;
        
        foreach (HologramTransceiver transceiver in transceivers)
        {
            if (!transceiver.isOwner) continue;
            if ((tickCounter - (i/2)) % transceiver.updateInterval == 0)
            {
                Letter updateLetter = Letter.Get();
                transceiver.Hologram.WriteData(updateLetter);
                Resident.SendLetter(updateLetter);
            }
            i++;
        }
        tickCounter++;
    }

    private static ushort GenerateHologramId()
    {
        return (ushort)Random.Range(0, 9999_9999+1);
    }

    public static void Instantiate(ushort prefabId)
    {
        HologramTransceiver transceiver = Instantiate(Instance.prefabs[prefabId]).GetComponent<HologramTransceiver>();
        ushort id = GenerateHologramId();
        Debug.LogAssertion($"Instantiated locally {id}, {prefabId}");
        transceiver.Initiate(id, prefabId, true);
        Instance.transceivers.Add(transceiver);
    }

    public static void HandleCreate(ResidentRecord _, Letter letter)
    {
        ushort id = letter.ReadUShort();
        ushort prefabId = letter.ReadUShort();
        Debug.LogAssertion($"received {id}, {prefabId}");
        HologramTransceiver transceiver = Instance.transceivers.Where(t => t.Id == id).FirstOrDefault();
        if (transceiver != null) { return; }
        transceiver = Instantiate(Instance.prefabs[prefabId]).GetComponent<HologramTransceiver>();
        transceiver.Initiate(id,prefabId,false);
        Instance.transceivers.Add(transceiver);
        letter.Release();
    }

    public static void HandleUpdate(ResidentRecord _, Letter letter)
    {
        foreach (var transceiver in Instance.transceivers)
        {
            transceiver.Hologram.ApplyData(letter);
        }
        letter.Release();
    }

    public static void HandleDestroy(ResidentRecord _, Letter letter)
    {
        ushort id = letter.ReadUShort();
        HologramTransceiver transceiver = Instance.transceivers.Where(t => t.Hologram.Id == id).FirstOrDefault();
        Instance.transceivers.Remove(transceiver);
        Destroy(transceiver.gameObject);
    }
}
