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
    public int Id { get { return hologram.Id; } }

    public int updateInterval;

    private Hologram hologram;
    public Hologram Hologram { get { return hologram; } private set {  hologram = value; } }

    public void Initiate(ushort id, ushort prefabId, bool isOwner)
    {
        this.isOwner = isOwner;
        switch (type)
        {
            case HologramType.POSITION:
                hologram = new PositionHologram(this,id,prefabId);
                break;
            default:
                Debug.LogAssertion("Unknown Hologram Type");
                break;
        }
        if (isOwner)
        {
            Letter letter = Letter.Get();
            Resident.SendLetter(hologram.WriteCreate(letter));
        }
    }
}
public enum HologramType
{
    POSITION
}
