using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is used to pack and unpack data from its target to and from bytes
/// In addition on hologram database it will be used to store creation details and recent state
/// </summary>
public class Hologram
{

    private ushort id;
    public ushort Id { get { return id; } protected set { id = value; } }
    private ushort prefabId;
    public ushort PrefabId { get { return prefabId; } protected set { prefabId = value; } }

    protected HologramTransceiver transceiver;

    /// <summary>
    /// Constructor for source hologram
    /// </summary>
    public Hologram(HologramTransceiver transceiver, ushort id, ushort prefabId)
    {
        this.transceiver = transceiver;
        this.id = id;
        this.prefabId = prefabId;
    }

    /// <summary>
    /// Method for hologram system
    /// </summary>
    /// <param name="letter"></param>
    public Letter WriteCreate(Letter letter)
    {
        letter.Write(LetterType.HOLOGRAMCREATE);
        letter.Write(Id);
        letter.Write(PrefabId);
        return letter;
    }

    public Letter WriteDestroy(Letter letter)
    {
        letter.Write(LetterType.HOLOGRAMDESTROY);
        letter.Write(Id);
        return letter;
    }

    /// <summary>
    /// Method for the transceiver to pack hologram object data into a letter
    /// </summary>
    /// <param name="letter"></param>
    public virtual Letter WriteData(Letter letter, bool useCache = false)
    {
        return letter;
    }

    /// <summary>
    /// Method for hologram database to cache create data to send to new players
    /// </summary>
    /// <param name="letter"></param>
    public virtual void CacheCreate(Letter letter)
    {

    }

    /// <summary>
    /// Method for hologram database to cache most recent update data for new players
    /// </summary>
    /// <param name="letter"></param>
    public virtual void CacheUpdate(Letter letter)
    {

    }

    public virtual void ApplyData(Letter letter)
    {

    }
}

public class PositionHologram : Hologram
{

    protected Vector3 cachedPosition = Vector3.zero;

    public PositionHologram(HologramTransceiver transceiver, ushort id, ushort prefabId) : base(transceiver, id, prefabId)
    {

    }

    public override Letter WriteData(Letter letter, bool useCache = false)
    {
        letter.Write(LetterType.HOLOGRAMUPDATE);
        letter.Write(Id);

        Vector3 position;
        if (useCache )
        {
            position = cachedPosition;
        }
        else
        {
            position = transceiver.transform.position;
        }
        
        Debug.LogAssertion($"Sending {Id} {position}");
        letter.Write(position.x);
        letter.Write(position.y);
        letter.Write(position.z);
        return letter;
    }

    public override void CacheCreate(Letter letter)
    {
        Id = letter.ReadUShort();
        PrefabId = letter.ReadUShort();
        letter.Release();
    }

    public override void CacheUpdate(Letter letter)
    {
        cachedPosition.x = letter.ReadFloat();
        cachedPosition.y = letter.ReadFloat();
        cachedPosition.z = letter.ReadFloat();
        letter.Release();
    }

    public override void ApplyData(Letter letter)
    {
        Vector3 position = Vector3.zero;
        position.x = letter.ReadFloat();
        position.y = letter.ReadFloat();
        position.z = letter.ReadFloat();
        Debug.LogAssertion($"Received {position}");
        transceiver.transform.position = position;

        letter.Release();
    }
}