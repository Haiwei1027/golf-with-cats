using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformHologram : Hologram
{
    protected Vector3 cachedPosition = Vector3.zero;
    protected Quaternion cachedRotation = Quaternion.identity;
    protected Vector3 cachedScale = Vector3.one;

    public TransformHologram(HologramTransceiver transceiver, ushort id, ushort prefabId, int ownerId) : base(transceiver, id, prefabId, ownerId)
    {

    }

    public override Letter WriteData(Letter letter)
    {
        base.WriteData(letter);

        if (transceiver != null)
        {
            cachedPosition = transceiver.transform.position;
            cachedRotation = transceiver.transform.rotation;
            cachedScale = transceiver.transform.localScale;
        }

        letter.Write(cachedPosition.x);
        letter.Write(cachedPosition.y);
        letter.Write(cachedPosition.z);

        letter.Write(cachedRotation.x);
        letter.Write(cachedRotation.y);
        letter.Write(cachedRotation.z);
        letter.Write(cachedRotation.w);

        letter.Write(cachedScale.x);
        letter.Write(cachedScale.y);
        letter.Write(cachedScale.z);
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
        CacheUpdate(letter);

        transceiver.transform.position = cachedPosition;
        transceiver.transform.rotation = cachedRotation;
        transceiver.transform.localScale = cachedScale;

        letter.Release();
    }
}