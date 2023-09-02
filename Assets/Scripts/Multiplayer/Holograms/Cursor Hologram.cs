using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHologram : Hologram
{
    private Vector3 position;
    private int colourId;

    public CursorHologram(HologramTransceiver transceiver, ushort id, ushort prefabId, int ownerId) : base(transceiver, id, prefabId, ownerId)
    {
    }

    public override Letter WriteCreate(Letter letter)
    {
        base.WriteCreate(letter);
        letter.Write((byte)HologramType.CURSOR);

        if (transceiver != null)
        {
            colourId = Resident.Instance.record.ColourId;
        }
        letter.Write(colourId);
        return letter;
    }

    public override void SetSpawn(Letter letter)
    {
        colourId = letter.ReadInt();

        if (transceiver != null)
        {
            transceiver.transform.GetChild(0).GetComponent<SpriteRenderer>().color = PlayerColour.Get(colourId);
        }
    }

    public override Letter WriteData(Letter letter)
    {
        base.WriteData(letter);

        if (transceiver != null)
        {
            position = transceiver.transform.position;
        }

        letter.Write(position);

        return letter;
    }

    public override void CacheUpdate(Letter letter)
    {
        position = letter.ReadVector3();

        letter.Release();
    }

    public override void ApplyData(Letter letter)
    {
        CacheUpdate(letter);

        transceiver.transform.position = position;
    }
}