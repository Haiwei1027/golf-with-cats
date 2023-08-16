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
    /// factory method for hologram class
    /// </summary>
    /// <param name="type">type of hologram</param>
    /// <param name="transceiver">the transceiver this hologram is attaced too (if left null it will use cache data instead)</param>
    /// <param name="id">id of hologram</param>
    /// <param name="prefabId">id of the prefab this belongs to</param>
    /// <returns></returns>
    public static Hologram CreateHologram(HologramType type, HologramTransceiver transceiver, ushort id, ushort prefabId)
    {
        switch (type)
        {
            case HologramType.POSITION:
                return new PositionHologram(transceiver, id, prefabId);
            case HologramType.TRANSFORM:
                return new TransformHologram(transceiver, id, prefabId);
            default:
                Debug.LogAssertion("Unknown Hologram Type");
                return null;
        }
    }

    /// <summary>
    /// Method for hologram system
    /// </summary>
    /// <param name="letter"></param>
    public virtual Letter WriteCreate(Letter letter)
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

    public override Letter WriteData(Letter letter)
    {
        letter.Write(LetterType.HOLOGRAMUPDATE);
        letter.Write(Id);

        Vector3 position;
        if (transceiver == null)
        {
            position = cachedPosition;
        }
        else
        {
            position = transceiver.transform.position;
        }
        letter.Write(position.x);
        letter.Write(position.y);
        letter.Write(position.z);
        return letter;
    }

    public override Letter WriteCreate(Letter letter)
    {
        base.WriteCreate(letter);
        letter.Write((byte)HologramType.POSITION);
        return letter;
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
        transceiver.transform.position = position;

        letter.Release();
    }
}

public class TransformHologram : Hologram
{
    protected Vector3 cachedPosition = Vector3.zero;
    protected Quaternion cachedRotation = Quaternion.identity;
    protected Vector3 cachedScale = Vector3.one;

    public TransformHologram(HologramTransceiver transceiver, ushort id, ushort prefabId) : base(transceiver, id, prefabId)
    {

    }

    public override Letter WriteData(Letter letter)
    {
        letter.Write(LetterType.HOLOGRAMUPDATE);
        letter.Write(Id);

        Vector3 position;
        Quaternion rotation;
        Vector3 scale;
        if (transceiver == null)
        {
            position = cachedPosition;
            rotation = cachedRotation;
            scale = cachedScale;
        }
        else
        {
            position = transceiver.transform.position;
            rotation = transceiver.transform.rotation;
            scale = transceiver.transform.localScale;
        }
        letter.Write(position.x);
        letter.Write(position.y);
        letter.Write(position.z);

        letter.Write(rotation.x);
        letter.Write(rotation.y);
        letter.Write(rotation.z);
        letter.Write(rotation.w);

        letter.Write(scale.x);
        letter.Write(scale.y);
        letter.Write(scale.z);
        return letter;
    }

    public override Letter WriteCreate(Letter letter)
    {
        base.WriteCreate(letter);
        letter.Write((byte)HologramType.TRANSFORM);
        return letter;
    }

    public override void CacheUpdate(Letter letter)
    {
        cachedPosition.x = letter.ReadFloat();
        cachedPosition.y = letter.ReadFloat();
        cachedPosition.z = letter.ReadFloat();

        cachedRotation.x = letter.ReadFloat();
        cachedRotation.y = letter.ReadFloat();
        cachedRotation.z = letter.ReadFloat();
        cachedRotation.w = letter.ReadFloat();

        cachedScale.x = letter.ReadFloat();
        cachedScale.y = letter.ReadFloat();
        cachedScale.z = letter.ReadFloat();
        letter.Release();
    }

    public override void ApplyData(Letter letter)
    {
        cachedPosition.x = letter.ReadFloat();
        cachedPosition.y = letter.ReadFloat();
        cachedPosition.z = letter.ReadFloat();
        transceiver.transform.position = cachedPosition;

        cachedRotation.x = letter.ReadFloat();
        cachedRotation.y = letter.ReadFloat();
        cachedRotation.z = letter.ReadFloat();
        cachedRotation.w = letter.ReadFloat();
        transceiver.transform.rotation = cachedRotation;

        cachedScale.x = letter.ReadFloat();
        cachedScale.y = letter.ReadFloat();
        cachedScale.z = letter.ReadFloat();
        transceiver.transform.localScale = cachedScale;

        letter.Release();
    }
}