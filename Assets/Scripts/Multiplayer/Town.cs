using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// This class acts as a container for a group of clients playing together
/// It handles game logic for the group and synchronisation
/// </summary>
public class Town
{

    TownRecord record;
    public HologramDatabase hologramDatabase { get; private set; }

    public int Id { get { return record.Id; }}

    private bool started = false;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="Mayor">the owner of the room/town</param>
    /// <param name="capacity">capcity of the room/town</param>
    public Town(ResidentRecord Mayor,int capacity)
    {
        record = new TownRecord(GenerateID());
        record.Capacity = capacity;
        record.MayorId = Mayor.Id;
        hologramDatabase = new HologramDatabase(this);
        Debug.LogAssertion($"Town {Id} created");
        Join(Mayor);
    }
    /// <summary>
    /// Generates a random 6 digit room/town id
    /// </summary>
    /// <returns>6 digit room/town id</returns>
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
        if (record.Population >= record.Capacity)
        {
            return false;
        }

        newResident.Town = record;
        record.AddResident(newResident);
        
        Letter welcomeLetter = Letter.Get().WriteTownWelcome(record,newResident);
        SendToAllResidents(welcomeLetter);
        hologramDatabase.Joined(newResident);
        Debug.LogAssertion($"Resident {newResident.Id} joined {record.Id}");
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
        Debug.LogAssertion($"Resident {resident.Id} left {record.Id}");
        if (resident.Id == record.MayorId)
        {
            ElectNewMayor();
        }
    }

    public void ElectNewMayor()
    {
        record.MayorId = record.Residents.First().Id;
    }

    public void Start(ResidentRecord sender, Letter letter)
    {
        if (sender.Id != record.MayorId) { return; }

        SendToAllResidents(Letter.Get().Write(LetterType.STARTGAME));
        started = true;
    }

    public void Update()
    {
        if (!started) { return; }
    }
}
