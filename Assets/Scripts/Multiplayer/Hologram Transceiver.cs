using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for network object behaviours
/// </summary>
public class HologramTransceiver : MonoBehaviour
{
    [SerializeField] HologramType type;

    public bool isOwner { get; private set; }
    public ushort Id { get { return hologram.Id; } }

    public int updateInterval;

    [SerializeField] Behaviour[] ownerOnlyScripts;
    [SerializeField] GameObject[] ownerOnlyObjects;

    private Hologram hologram;
    public Hologram Hologram { get { return hologram; } private set {  hologram = value; } }


    /// <summary>
    /// Method for initiating this network object
    /// </summary>
    /// <param name="id">Assigned id of this network object by from hologram system</param>
    /// <param name="prefabId">index of this network object's prefab in the prefab list</param>
    public void Initiate(ushort id, ushort prefabId, int ownerId, Letter createLetter = null)
    {
        isOwner = false || isOwner;
        
        hologram = Hologram.CreateHologram(type, this, id, prefabId, ownerId);
        if (isOwner)
        {
            Letter letter = Letter.Get();
            Resident.SendLetter(hologram.WriteCreate(letter));
        }
        else
        {
            hologram.SetSpawn(createLetter);
            foreach (Behaviour behaviour in ownerOnlyScripts)
            {
                behaviour.enabled = false;
            }
            foreach (GameObject gameObject in ownerOnlyObjects)
            {
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Method for initiating this network object locally as the owner
    /// </summary>
    /// <param name="id">Assigned id of this network object by from hologram system</param>
    /// <param name="prefabId">index of this network object's prefab in the prefab list</param>
    public void Initiate(ushort id, ushort prefabId)
    {
        isOwner = true;
        Initiate(id, prefabId,Resident.Id);
    }

    void OnDestroy()
    {
        if (hologram == null) return;
        Resident.SendLetter(hologram.WriteDestroy(Letter.Get()));
    }
}
