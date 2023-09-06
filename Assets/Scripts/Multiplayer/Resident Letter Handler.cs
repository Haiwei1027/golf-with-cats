using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidentLetterHandler : LetterHandler
{
    


    /*letterHandlers = new Dictionary<byte, PostOffice.LetterHandler>()
        {
            {(byte) LetterType.WELCOME, HandleWelcome },
            { (byte)LetterType.TOWNWELCOME,HandleTownWelcome },
            { (byte)LetterType.HOLOGRAMCREATE, HologramSystem.HandleCreate },
            { (byte)LetterType.HOLOGRAMUPDATE, HologramSystem.HandleUpdate },
            { (byte)LetterType.HOLOGRAMDESTROY, HologramSystem.HandleDestroy },
            { (byte)LetterType.STARTGAME, HandleStartGame}
        };*/

    public override void Handle(ResidentRecord postboxOwner, Letter letter)
    {
        LetterType type = letter.ReadType();
        switch (type)
        {
            default:
                Debug.LogError($"Unknown type {type}");
                return;
        }
    }

    void HandleWelcome(ResidentRecord record, Letter letter)
    {
        Debug.LogAssertion("Connected");
        record.Id = letter.ReadInt();
        Debug.LogAssertion($"ID: {record.Id}");

        letter.Clear();
        letter.WriteIntroduce(record.Username);
        record.Postbox.Send(letter);
    }

    void HandleTownWelcome(ResidentRecord record, Letter letter)
    {
        int townId = letter.ReadInt();
        int newResidentID = letter.ReadInt();
        int population = letter.ReadInt();

        if (record.Town == null)
        {
            record.Town = new TownRecord(townId);
        }

        for (int i = 0; i < population; i++)
        {
            int id = letter.ReadInt();
            string username = letter.ReadString();
            int displayColour = letter.ReadInt();
            if (id != record.Id)
            {
                ResidentRecord otherResident = new ResidentRecord(id, username, displayColour);
                record.Town.AddResident(otherResident);
            }
            else
            {
                Debug.LogAssertion(displayColour);
                record.ColourId = displayColour;
                record.Town.AddResident(record);
            }
        }

        letter.Release();

        onJoinTown?.Invoke(newResidentID);
    }

    public void HandleStartGame(ResidentRecord record, Letter letter)
    {
        onStartGame?.Invoke();
    }
}
