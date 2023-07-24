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

    public int Id { get { return record.Id; }}

    public Town(ResidentRecord Mayor,int capacity)
    {
        record = new TownRecord(GenerateID());
        record.Capacity = capacity;
        record.MayorId = Mayor.Id;
        Debug.LogAssertion($"Town {Id} created");
        Join(Mayor);
    }

    private int GenerateID()
    {
        return new System.Random().Next(999_999 + 1);
    }
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
        Debug.LogAssertion($"Resident {newResident.Id} joined {record.Id}");
        return true;
    }

    public void SendToAllResidents(Letter letter)
    {
        SendToAllButOne(letter, -1);
    }

    public void SendToAllButOne(Letter letter, int except)
    {
        foreach (ResidentRecord resident in record.Residents)
        {
            if (resident.Id == except) continue;
            resident.Postbox.Send(letter, true);
        }
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

    public void Initiate()
    {
        
    }

    public void Start()
    {

    }

    public void Update()
    {
        
    }
}
