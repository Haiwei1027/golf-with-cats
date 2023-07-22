using System;
using System.Collections;
using System.Collections.Generic;
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
        Join(Mayor);
    }

    private int GenerateID()
    {
        return new Random().Next(9999_9999 + 1);
    }
    public bool Join(ResidentRecord newResident)
    {
        if (record.Population >= record.Capacity)
        {
            return false;
        }

        newResident.Town = record;
        record.AddResident(newResident);
        
        newResident.Postbox.Send(Letter.Get().WriteTownWelcome(record));
        return true;
    }

    public void Leave(ResidentRecord resident)
    {
        resident.Town = null;
        record.Residents.Remove(resident);
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
