using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will be used by a town to store and handle hologram letters
/// It will help in receiving and sending hologram letter from multiple residents 
/// </summary>
public class HologramDatabase
{
    private List<Hologram> holograms;
    private Town town;
    public HologramDatabase(Town town)
    {
        holograms = new List<Hologram>();
        this.town = town;
    }

    public void Add(Letter letter)
    {
        ushort id = letter.ReadUShort();
        ushort prefabId = letter.ReadUShort();
        holograms.Add(new Hologram(id, prefabId));
    }

    public void Update(Letter letter)
    {

    }

    public void Remove(Letter letter)
    {

    }
}
