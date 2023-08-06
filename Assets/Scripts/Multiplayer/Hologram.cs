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

    private HologramTransceiver transceiver;

    private Letter cachedCreate;
    private Letter cachedUpdate;

    /// <summary>
    /// Constructor for source hologram
    /// </summary>
    public Hologram(HologramTransceiver transceiver)
    {
        this.transceiver = transceiver;
    }
    /// <summary>
    /// Constructor for remote hologram
    /// </summary>
    public Hologram(ushort id, ushort prefabId)
    {
        Id = id;
        PrefabId = prefabId;
    }
    /// <summary>
    /// Constructor for projected hologram
    /// </summary>
    public Hologram(HologramTransceiver transceiver, ushort id)
    {
        this.transceiver = transceiver;
        Id = id;
    }

    /// <summary>
    /// Method for hologram system
    /// </summary>
    /// <param name="letter"></param>
    public virtual Letter WriteCreate(Letter letter, ushort prefabId)
    {
        PrefabId = prefabId;
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
    /// Method for hologram database to cache creation letter and most recent update letter
    /// </summary>
    /// <param name="letter"></param>
    public void CacheData(Letter letter)
    {
        
    }

    public void ApplyData()
    {

    }

    public void Clear()
    {
        cachedCreate?.Release();
        cachedUpdate?.Release();
    }
}
