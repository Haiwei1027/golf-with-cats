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
    public ushort Id { get { return id; } private set { id = value; } }
    private ushort prefabId;
    public ushort PrefabId { get { return prefabId; } private set { prefabId = value; } }

    protected HologramTransceiver transceiver;

    private Letter cachedCreate;
    private Letter cachedUpdate;

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
    public virtual Letter WriteData(Letter letter)
    {
        return letter;
    }

    /// <summary>
    /// Method for hologram database to cache creation letter
    /// </summary>
    /// <param name="letter"></param>
    public void CacheCreate(Letter letter)
    {
        if (cachedCreate != null)
        {
            cachedCreate.Release();
        }
        cachedCreate = letter;
    }

    /// <summary>
    /// Method for hologram database to cache most recent update letter
    /// </summary>
    /// <param name="letter"></param>
    public void CacheUpdate(Letter letter)
    {
        if (cachedUpdate != null)
        {
            cachedUpdate.Release();
        }
        cachedUpdate = letter;
    }

    public virtual void ApplyData(Letter letter)
    {

    }

    public void Clear()
    {
        cachedCreate?.Release();
        cachedUpdate?.Release();
    }
}

public class PositionHologram : Hologram
{
    public PositionHologram(HologramTransceiver transceiver, ushort id, ushort prefabId) : base(transceiver, id, prefabId)
    {

    }

    public override Letter WriteData(Letter letter)
    {
        letter.Write(LetterType.HOLOGRAMUPDATE);
        letter.Write(Id);

        Vector3 position = transceiver.transform.position;
        letter.Write(position.x);
        letter.Write(position.y);
        letter.Write(position.z);
        return letter;
    }

    public override void ApplyData(Letter letter)
    {
        ushort updateId = letter.ReadUShort();
        if (Id != updateId) { return; }

        Vector3 position = Vector3.zero;
        position.x = letter.ReadFloat();
        position.y = letter.ReadFloat();
        position.z = letter.ReadFloat();
        transceiver.transform.position = position;

        letter.Release();
    }
}