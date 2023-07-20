using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class acts as a container for a group of clients playing together
/// It handles game logic for the group and synchronisation
/// </summary>
public class Town
{

    Postbox[] residents;
    private int id;

    public Town(int capacity)
    {
        residents = new Postbox[capacity];
        GenerateID();
    }

    private void GenerateID()
    {
        id = new Random().Next(9999_9999 + 1);
    }

    public int GetID()
    {
        return id;
    }

    void Join(Postbox newResident)
    {

    }

    void Leave(Postbox resident)
    {

    }

    void Start()
    {
        
    }



    void FixedUpdate()
    {
        
    }
}
