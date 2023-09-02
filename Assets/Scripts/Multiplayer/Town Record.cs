using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class responsible for storing town data 
/// </summary>
public class TownRecord
{
    private List<ResidentRecord> residents;
    public List<ResidentRecord> Residents { get { return residents; }private set { residents = value; } }
    public int Population { get { return residents.Count; } }
    private int mayorId;
    public int MayorId { get { return mayorId; } set { mayorId = value; } }
    private int id;
    public int Id { get { return id; } set { id = value; } }
    private int capacity;
    public int Capacity { get { return capacity; } set { capacity = value; } }

    public void AddResident(ResidentRecord resident)
    {
        if (resident == null) return;
        foreach (ResidentRecord existingResident in residents)
        {
            if (existingResident.Id == resident.Id)
            {
                return;
            }
        }
        residents.Add(resident);
        resident.Town = this;
    }

    public void RemoveResident(ResidentRecord resident)
    {
        if (resident == null) return;
        if (residents.Contains(resident))
        {
            residents.Remove(resident);
            resident.Town = null;
        }
    }

    public TownRecord(int id)
    {
        residents = new List<ResidentRecord>();
        this.id = id;
    }
}
