using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionHologram : Hologram
{

    protected Vector3 cachedPosition = Vector3.zero;

    public PositionHologram(HologramTransceiver transceiver, ushort id, ushort prefabId, int ownerId) : base(transceiver, id, prefabId, ownerId)
    {

    }

    public override Letter WriteData(Letter letter)
    {
        base.WriteData(letter);

        if (transceiver != null)
        {
            cachedPosition = transceiver.transform.position;
        }

        letter.Write(cachedPosition.x);
        letter.Write(cachedPosition.y);
        letter.Write(cachedPosition.z);
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