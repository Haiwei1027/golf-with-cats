using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviour attached on each hologram gameobject to store its byte data and synchronisation state
/// </summary>
public class HologramTransceiver : MonoBehaviour
{
    [SerializeField] HologramType type;

    public bool isOwner { get; private set; }
    public ushort Id { get { return hologram.Id; } }

    public ushort ID;

    public int updateInterval;

    private Hologram hologram;
    public Hologram Hologram { get { return hologram; } private set {  hologram = value; } }

    public void Initiate(ushort id, ushort prefabId, bool isOwner)
    {
        ID = id;
        this.isOwner = isOwner;
        hologram = Hologram.CreateHologram(type, this, id, prefabId);
        if (isOwner)
        {
            Letter letter = Letter.Get();
            Resident.SendLetter(hologram.WriteCreate(letter));
        }
    }
}
public enum HologramType : byte
{
    POSITION,
    TRANSFORM
}
