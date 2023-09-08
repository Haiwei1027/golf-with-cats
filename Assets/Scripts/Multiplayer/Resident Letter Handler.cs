using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidentLetterHandler : LetterHandler
{

    public event Action onConnected;
    public event Action onDisconnected;

    public event Action onStartGame;

    public event Action<int> onJoinTown;
    public event Action<int> onLeaveTown;

    public override void Handle(ResidentRecord postboxOwner, Letter letter)
    {
        LetterType type = letter.ReadType();
        switch (type)
        {
            case LetterType.WELCOME:
                HandleWelcome(postboxOwner, letter);
                return;
            case LetterType.TOWNWELCOME:
                HandleTownWelcome(postboxOwner, letter);
                return;
            case LetterType.HOLOGRAMCREATE:
                HologramSystem.HandleCreate(postboxOwner, letter);
                return;
            case LetterType.HOLOGRAMUPDATE:
                HologramSystem.HandleUpdate(postboxOwner, letter);
                return;
            case LetterType.STARTGAME:
                onStartGame?.Invoke();
                return;
            case LetterType.GOODBYE:
                onLeaveTown?.Invoke(letter.ReadUShort());
                return;
            default:
                Debug.LogError($"Unknown type {type}");
                return;
        }
    }

    public override void Close()
    {
        onDisconnected?.Invoke();
    }

    void HandleWelcome(ResidentRecord record, Letter letter)
    {
        Debug.Log("Connected");
        record.Id = letter.ReadInt();
        Debug.Log($"ID: {record.Id}");

        letter.Clear();
        letter.WriteIntroduce(record.Username);
        record.Postbox.Send(letter);

        onConnected?.Invoke();
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
}
