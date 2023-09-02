using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatHologram : Hologram
{
    protected Vector3 cachedDestination = Vector3.zero;
    protected Vector3 cachedLook = Vector3.zero;

    protected CatAnimation catAnimation;
    protected NavMeshAgent navAgent;

    public CatHologram(HologramTransceiver transceiver, ushort id, ushort prefabId, int ownerId) : base(transceiver, id, prefabId, ownerId)
    {
        if (transceiver != null)
        {
            catAnimation = transceiver.GetComponent<CatAnimation>();
            navAgent = transceiver.GetComponent<NavMeshAgent>();
        }
    }

    public override Letter WriteCreate(Letter letter)
    {
        return base.WriteCreate(letter).Write((byte)HologramType.CAT);
    }

    public override Letter WriteData(Letter letter)
    {
        base.WriteData(letter);

        if (transceiver != null)
        {
            cachedDestination = navAgent.destination;
            cachedLook = catAnimation.lookingAt;
        }

        letter.Write(cachedDestination);

        letter.Write(cachedLook);

        return letter;
    }

    public override void ApplyData(Letter letter)
    {
        CacheUpdate(letter);

        catAnimation.LookAtPosition(cachedLook);
        navAgent.SetDestination(cachedDestination);
    }

    public override void CacheUpdate(Letter letter)
    {
        cachedDestination = letter.ReadVector3();
        cachedLook = letter.ReadVector3();

        letter.Release();
    }
}