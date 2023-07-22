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

    List<ResidentRecord> residents;
    int founderID;
    private int id;
    public int Id { get; private set; }
    private int capacity;

    public Town(ResidentRecord founder,int capacity)
    {
        residents = new List<ResidentRecord>();
        GenerateID();

        founderID = founder.Id;
        Join(founder);
    }

    private void GenerateID()
    {
        id = new Random().Next(9999_9999 + 1);
    }
    public bool Join(ResidentRecord newResident)
    {
        if (residents.Count >= capacity)
        {
            return false;
        }

        newResident.Town = this;
        residents.Add(newResident);
        
        newResident.Postbox.Send(Letter.Get().WriteTownWelcome(residents, id));
        return true;
    }

    public void Leave(ResidentRecord resident)
    {
        resident.Town = null;
        residents.Remove(resident);
        if (resident.Id == founderID)
        {
            founderID = residents.First().Id;
        }
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
