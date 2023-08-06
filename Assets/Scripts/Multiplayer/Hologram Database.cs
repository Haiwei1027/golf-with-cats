using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void Add(ResidentRecord sender, Letter letter)
    {
        ushort id = letter.ReadUShort();
        ushort prefabId = letter.ReadUShort();
        Hologram hologram = new Hologram(id, prefabId);
        hologram.CacheCreate(letter.Clear());
        holograms.Add(hologram);
        town.SendToAllButOne(letter, sender.Id, false);
    }

    public void Update(ResidentRecord sender, Letter letter)
    {
        ushort id = letter.ReadUShort();
        Hologram hologram = holograms.Where(h => h.Id == id).FirstOrDefault();
        if (hologram == null)
        {
            return;
        }
        hologram.CacheUpdate(letter.Clear());
        town.SendToAllButOne(letter, sender.Id, false);
    }

    public void Remove(ResidentRecord sender, Letter letter)
    {
        ushort id = letter.ReadUShort();
        holograms.Remove(holograms.Where(h => h.Id == id).FirstOrDefault());
        letter.Clear();
        town.SendToAllButOne(letter,sender.Id);
    }
}
