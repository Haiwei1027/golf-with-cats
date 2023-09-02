using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class responsible for creating,updating and destroying network objects while also serialising,caching and deserialising their corresponding data
/// </summary>
public abstract class Hologram
{

    private ushort id;
    public ushort Id { get { return id; } protected set { id = value; } }
    private ushort prefabId;
    public ushort PrefabId { get { return prefabId; } protected set { prefabId = value; } }
    private int ownerId;
    public int OwnerId { get { return ownerId; } }

    protected HologramTransceiver transceiver;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="transceiver">The transceiver this hologram is going to be attached to (null for caching mode)</param>
    /// <param name="id">The id</param>
    /// <param name="prefabId">The index of the prefab this hologram represents in hologram system's prefab list</param>
    /// <param name="ownerId">The owner resident's id</param>
    public Hologram(HologramTransceiver transceiver, ushort id, ushort prefabId, int ownerId)
    {
        this.transceiver = transceiver;
        this.id = id;
        this.prefabId = prefabId;
        this.ownerId = ownerId;
    }

    /// <summary>
    /// Method to initialise a network object
    /// </summary>
    /// <param name="letter">The hologram create letter after the hologram type is read and network object is instantiated</param>
    public virtual void SetSpawn(Letter letter)
    {

    }
    
    public static Hologram CreateHologram(HologramType type, HologramTransceiver transceiver, ushort id, ushort prefabId, int ownerId)
    {
        switch (type)
        {
            case HologramType.POSITION:
                return new PositionHologram(transceiver, id, prefabId, ownerId);
            case HologramType.TRANSFORM:
                return new TransformHologram(transceiver, id, prefabId, ownerId);
            case HologramType.CAT:
                return new CatHologram(transceiver, id, prefabId, ownerId);
            case HologramType.CURSOR:
                return new CursorHologram(transceiver, id, prefabId, ownerId);
            default:
                Debug.LogAssertion("Unknown Hologram Type");
                return null;
        }
    }

    /// <summary>
    /// Method for writing a letter to inform other resident of the creation of this network object
    /// </summary>
    /// <param name="letter">Empty letter</param>
    /// <returns>Letter with generic hologram details for others to instantiate</returns>
    public virtual Letter WriteCreate(Letter letter)
    {
        letter.Write(LetterType.HOLOGRAMCREATE);
        letter.Write(Id);
        letter.Write(PrefabId);
        letter.Write(OwnerId);
        return letter;
    }

    /// <summary>
    /// Method for writing a letter to inform other residents of the deletion of this network object
    /// </summary>
    /// <param name="letter">Empty letter</param>
    /// <returns>Letter with hologram details for others to destroy</returns>
    public Letter WriteDestroy(Letter letter)
    {
        letter.Write(LetterType.HOLOGRAMDESTROY);
        letter.Write(Id);
        return letter;
    }

    /// <summary>
    /// Method for writing a letter to inform other residents this network object has updated
    /// </summary>
    /// <param name="letter"></param>
    /// <returns></returns>
    public virtual Letter WriteData(Letter letter)
    {
        letter.Write(LetterType.HOLOGRAMUPDATE);
        letter.Write(Id);
        return letter;
    }

    /// <summary>
    /// Method for reading an update letter and caching the data into the hologram
    /// </summary>
    /// <param name="letter">A hologram update letter</param>
    public virtual void CacheUpdate(Letter letter)
    {

    }

    /// <summary>
    /// Method for reading an update letter and applying its data to the hologram
    /// </summary>
    /// <param name="letter">An update letter</param>
    public virtual void ApplyData(Letter letter)
    {

    }
}



