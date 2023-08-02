using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that handles sending synchronised object data
/// </summary>
public class HologramTransmitter : MonoBehaviour
{

    public Hologram hologram;
    public HologramType hologramType;

    [SerializeField] ushort sendInterval;

    private int sendOffset;

    private int prefabId = -1;
    private int tick;

    private void Initate()
    {
        switch (hologramType)
        {
            case HologramType.TRANSFORM:
                hologram = new TransformHologram(this);
                break;
            default:
                Debug.LogAssertion($"Unknown hologram type: {hologramType}");
                break;
        }
        sendOffset = hologram.Id % sendInterval;
        tick = Mathf.CeilToInt(Time.fixedTime * 50);
    }

    public void SendCreate(int prefabId)
    {
        if (hologram == null) { Debug.LogAssertion("Missing Hologram Data"); }
        Letter letter = Letter.Get();
        letter.Write(LetterType.HOLOGRAMCREATE);
        letter.Write(prefabId);
        letter.Write(hologram.Id);
        Resident.SendLetter(letter);
    }

    private void SendUpdate()
    {
        if ((tick-sendOffset) % sendInterval == 0)
        {
            Letter letter = Letter.Get();
            letter.Write(LetterType.HOLOGRAMUPDATE);
            letter.Write(hologram);
            Resident.SendLetter(letter);
        }
    }

    private void SendDestroy()
    {
        Letter letter = Letter.Get();
        letter.Write(LetterType.HOLOGRAMDESTROY);
        letter.Write(hologram.Id);
        Resident.SendLetter(letter);
    }

    public void Start()
    {
        Initate();
    }

    public void FixedUpdate()
    {
        SendUpdate();
    }

    public void OnDestroy()
    {
        SendDestroy();
    }
}

public enum HologramType
{
    TRANSFORM
}
