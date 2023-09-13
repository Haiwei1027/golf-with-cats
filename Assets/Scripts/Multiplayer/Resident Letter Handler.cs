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
                break;
            case LetterType.TOWNWELCOME:
                HandleTownWelcome(postboxOwner, letter);
                break;
            case LetterType.HOLOGRAMCREATE:
                HologramSystem.HandleCreate(postboxOwner, letter);
                break;
            case LetterType.HOLOGRAMUPDATE:
                HologramSystem.HandleUpdate(postboxOwner, letter);
                break;
            case LetterType.STARTGAME:
                onStartGame?.Invoke();
                break;
            case LetterType.GOODBYE:
                onLeaveTown?.Invoke(letter.ReadUShort());
                break;
            default:
                Debug.LogError($"Unknown type {type}");
                break;
        }

        letter.Release();
    }

    public override void Close(Postbox postbox)
    {
        onDisconnected?.Invoke();
    }

    void HandleWelcome(ResidentRecord record, Letter letter)
    {
        Debug.Log("Connected");
        record.Id = letter.ReadInt();
        Debug.Log($"ID: {record.Id}");

        letter = LetterFactory.Get();    
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
                record.ColourId = displayColour;
                record.Town.AddResident(record);
            }
        }
        Debug.Log(record.Town.Population);
        onJoinTown?.Invoke(newResidentID);
    }
}
