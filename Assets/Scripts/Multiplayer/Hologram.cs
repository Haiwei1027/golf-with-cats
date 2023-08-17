using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This class is used to pack and unpack data from its target to and from bytes
/// In addition on hologram database will also be used to cache creation details and recent state
/// </summary>
public class Hologram
{

    private ushort id;
    public ushort Id { get { return id; } protected set { id = value; } }
    private ushort prefabId;
    public ushort PrefabId { get { return prefabId; } protected set { prefabId = value; } }

    protected HologramTransceiver transceiver;


    public Hologram(HologramTransceiver transceiver, ushort id, ushort prefabId)
    {
        this.transceiver = transceiver;
        this.id = id;
        this.prefabId = prefabId;
    }

    public static Hologram CreateHologram(HologramType type, HologramTransceiver transceiver, ushort id, ushort prefabId)
    {
        switch (type)
        {
            case HologramType.POSITION:
                return new PositionHologram(transceiver, id, prefabId);
            case HologramType.TRANSFORM:
                return new TransformHologram(transceiver, id, prefabId);
            case HologramType.CAT:
                return new CatHologram(transceiver, id, prefabId);
            default:
                Debug.LogAssertion("Unknown Hologram Type");
                return null;
        }
    }

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

    /// <param name="letter"></param>
    public virtual Letter WriteData(Letter letter)
    {
        letter.Write(LetterType.HOLOGRAMUPDATE);
        letter.Write(Id);
        return letter;
    }

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

public class CatHologram : Hologram
{
    protected Vector3 cachedDestination = Vector3.zero;
    protected Vector3 cachedLook = Vector3.zero;

    protected CatAnimation catAnimation;
    protected NavMeshAgent navAgent;

    public CatHologram(HologramTransceiver transceiver, ushort id, ushort prefabId) : base(transceiver, id, prefabId)
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
            cachedLook = CameraController.GetMouseWorldPosition();
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