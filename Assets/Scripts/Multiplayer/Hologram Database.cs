using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class responsible for relaying hologram letters from the clients to mimick peer to peer.
/// It also keeps record of each hologram in use
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

    public void Joined(ResidentRecord newResident)
    {
        foreach (var hologram in holograms)
        {
            Letter letter = LetterFactory.Get();
            town.SendTo(hologram.WriteCreate(letter), newResident.Id);
        }
    }

    /// <summary>
    /// Add a new hologram to the database and also relays the letter
    /// </summary>
    /// <param name="sender">The client who created this network object</param>
    /// <param name="letter">The hologram create letter</param>
    public void Add(ResidentRecord sender, Letter letter)
    {
        ushort id = letter.ReadUShort();
        ushort prefabId = letter.ReadUShort();
        int ownerId = letter.ReadInt();
        HologramType hologramType = (HologramType)letter.ReadByte();
        Hologram hologram = Hologram.CreateHologram(hologramType, null, id, prefabId, ownerId);
        hologram.SetSpawn(letter);
        holograms.Add(hologram);

        letter = LetterFactory.Get();
        town.SendToAllButOne(hologram.WriteCreate(letter), sender.Id);
        Debug.Log($"Sent {hologram.Id} {hologram.PrefabId} to all but {sender.Id}");
    }

    /// <summary>
    /// Updates a hologram in the database and also relays the letter
    /// </summary>
    /// <param name="sender">The client who updated this network object (must be owner)</param>
    /// <param name="letter">The hologram update letter</param>
    public void Update(ResidentRecord sender, Letter letter)
    {
        ushort id = letter.ReadUShort();
        Hologram hologram = holograms.Find(h => h.Id == id);
        if (hologram == null)
        {
            return;
        }
        hologram.CacheUpdate(letter);
        
        letter = LetterFactory.Get();
        hologram.WriteData(letter);
        town.SendToAllButOne(letter, sender.Id);
        Debug.Log($"Sent {hologram.Id} to all but {sender.Id}");
    }

    /// <summary>
    /// Remove a hologram from the database and also relays the letter
    /// </summary>
    /// <param name="sender">The client who destroyed this network object (must be owner)</param>
    /// <param name="letter">The hologram destroy letter</param>
    public void Remove(ResidentRecord sender, Letter letter)
    {
        ushort id = letter.ReadUShort();
        Hologram hologram = holograms.Find(h => h.Id == id);
        holograms.Remove(hologram);
        town.SendToAllButOne(letter,sender.Id);
    }
}
