using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class responsible for handling the network side (sending and handling letters) of instantiating network objects, updating holograms and destroying network objects
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
            if (!transceiver.isOwner || !transceiver.enabled) continue;
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
        return (ushort)UnityEngine.Random.Range(0, 9999_9999+1);
    }

    public static GameObject Instantiate(ushort prefabId, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject spawned = Instantiate(Instance.prefabs[prefabId], spawnPosition, spawnRotation);
        try
        {
            HologramTransceiver transceiver = spawned.GetComponent<HologramTransceiver>();
            ushort id = GenerateHologramId();
            Debug.LogAssertion($"Instantiated locally {id}, {prefabId}");
            transceiver.Initiate(id, prefabId);
            Instance.transceivers.Add(transceiver);
        }catch (NullReferenceException e)
        {
            Debug.LogError(e);
        }
        return spawned;
    }

    public static GameObject Instantiate(ushort prefabId)
    {
        return Instantiate(prefabId, Vector3.zero, Quaternion.identity);
    }

    public static void HandleCreate(ResidentRecord _, Letter letter)
    {
        ushort id = letter.ReadUShort();
        ushort prefabId = letter.ReadUShort();
        int ownerId = letter.ReadInt();
        HologramType hologramType = (HologramType)letter.ReadByte();
        Debug.LogAssertion($"Received create {id} {prefabId}");
        HologramTransceiver transceiver = Instance.transceivers.Where(t => t.Id == id).FirstOrDefault();
        if (transceiver != null) { return; }
        transceiver = Instantiate(Instance.prefabs[prefabId]).GetComponent<HologramTransceiver>();
        transceiver.Initiate(id,prefabId,ownerId, letter);
        Instance.transceivers.Add(transceiver);
    }

    public static void HandleUpdate(ResidentRecord _, Letter letter)
    {
        ushort id = letter.ReadUShort();
        //Debug.LogAssertion($"Received Update {id}");
        HologramTransceiver transceiver = Instance.transceivers.Find(t => t.Id == id);
        transceiver.Hologram.ApplyData(letter);
    }

    public static void HandleDestroy(ResidentRecord _, Letter letter)
    {
        ushort id = letter.ReadUShort();
        HologramTransceiver transceiver = Instance.transceivers.Where(t => t.Hologram.Id == id).FirstOrDefault();
        Instance.transceivers.Remove(transceiver);
        Destroy(transceiver.gameObject);
    }
}
