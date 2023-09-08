using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Class responsible for managing a lobby and orchestrating its resident's game session events
/// </summary>
public class Town
{

    TownRecord record;
    public HologramDatabase hologramDatabase { get; private set; }

    public int Id { get { return record.Id; }}
    public int Population { get {  return record.Population; }}

    private bool started = false;
    private int colourPointer;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="Mayor">The creator of the town(lobby)</param>
    /// <param name="capacity">Capcity of the town(lobby)</param>
    public Town(ResidentRecord Mayor)
    {
        record = new TownRecord(GenerateID());
        record.MayorId = Mayor.Id;
        hologramDatabase = new HologramDatabase(this);
        colourPointer = new System.Random().Next(PlayerColour.COUNT);
        Debug.Log($"Town {Id} created");
        Join(Mayor);
    }
    /// <summary>
    /// Generates a random 6 digit id
    /// </summary>
    /// <returns>6 digit id</returns>
    private int GenerateID()
    {
        return new System.Random().Next(999_999 + 1);
    }

    /// <summary>
    /// Add a resident to this town
    /// </summary>
    /// <param name="newResident"></param>
    /// <returns></returns>
    public bool Join(ResidentRecord newResident)
    {
        if (record.Population >= TownRecord.Capacity)
        {
            return false;
        }

        newResident.Town = record;
        record.AddResident(newResident);

        newResident.ColourId = colourPointer;
        colourPointer = (colourPointer + 2)%PlayerColour.COUNT; //best prime
        Letter welcomeLetter = LetterFactory.Get().WriteTownWelcome(record,newResident);
        
        SendToAllResidents(welcomeLetter);
        hologramDatabase.Joined(newResident);
        Debug.Log($"Resident {newResident.Id} joined {record.Id}");
        return true;
    }

    public void SendToAllResidents(Letter letter, bool release = true)
    {
        SendToAllButOne(letter, -1, release);
    }

    public void SendToAllButOne(Letter letter, int except, bool release = true)
    {
        foreach (ResidentRecord resident in record.Residents)
        {
            if (resident.Id == except) continue;
            resident.Postbox.Send(letter, false);
        }
        if (!release) { return; }
        letter.Release();
    }

    public void SendTo(Letter letter, int id, bool release = true)
    {
        foreach (ResidentRecord resident in record.Residents)
        {
            if (resident.Id == id)
            {
                resident.Postbox.Send(letter, release);
                break;
            }
        }
        if (!release) { return; }
        letter.Release();
    }

    public void Leave(ResidentRecord resident)
    {
        record.RemoveResident(resident);
        Debug.Log($"Resident {resident.Id} left {record.Id}");
        if (resident.Id == record.MayorId)
        {
            ElectNewMayor();
        }
    }

    public void ElectNewMayor()
    {
        if (record.Population <= 0) { return; }
        record.MayorId = record.Residents.First().Id;
    }

    public void Start(ResidentRecord sender, Letter letter)
    {
        if (sender.Id != record.MayorId) { return; }

        SendToAllResidents(LetterFactory.Get().Write(LetterType.STARTGAME));
        started = true;
    }

    public void Update()
    {
        if (!started) { return; }
    }
}
