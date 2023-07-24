using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using TMPro;
using System;
using UnityEngine.UIElements;

/// <summary>
/// This class contains client logic
/// It handles synchronisation and sharing actions to peers
/// </summary>
public class Resident : MonoBehaviour
{
    public static Resident Instance { get; private set; }

    [SerializeField] string ServerIP;
    [SerializeField] bool useLoopback;

    public static Dictionary<byte, PostOffice.LetterHandler> letterHandlers;

    public static event Action onConnected;
    public static event Action onDisconnected;

    public static event Action<int> onJoinTown;
    public static event Action onLeaveTown;

    public ResidentRecord record;
    public TownRecord town;

    Postbox postbox;

    private bool connected;

    #region Letter Handlers
    void HandleWelcome(ResidentRecord _, Letter letter)
    {
        Debug.LogAssertion("Connected");
        record.Id = letter.ReadInt();
        Debug.LogAssertion($"ID: {record.Id}");
        
        letter.Clear();
        letter.WriteIntroduce(record.Username);
        postbox.Send(letter);

        onConnected?.Invoke();
    }

    void HandleTownWelcome(ResidentRecord _, Letter letter)
    {
        int townId = letter.ReadInt();
        int newResidentID = letter.ReadInt();
        int population = letter.ReadInt();

        if (town == null)
        {
            town = new TownRecord(townId);
        }
        else if (townId == town.Id)
        {
            Debug.LogAssertion($"{newResidentID} Has Joined");
        }
        else
        {
            Debug.LogAssertion("Received Incorrect Town Welcome");
        }

        for (int i = 0; i < population; i++)
        {
            int id = letter.ReadInt();
            string username = letter.ReadString();
            if (id != record.Id)
            {
                ResidentRecord otherResident = new ResidentRecord(id, username);
                town.AddResident(otherResident);
            }
            else
            {
                
                town.AddResident(record);
            }
        }



        letter.Release();

        onJoinTown?.Invoke(newResidentID);
    }

    #endregion

    #region UI Methods
    public void Connect(TMP_InputField usernameField)
    {
        record.Username = usernameField.text;
        Debug.LogAssertion("Connecting");
        postbox.Connect(new IPEndPoint(useLoopback ? IPAddress.Loopback : IPAddress.Parse(ServerIP),PostOffice.Port));
    }

    public void Disconnect()
    {
        postbox.Close();
        record = null;
        town = null;
        onDisconnected?.Invoke();
    }

    public void JoinTown(TMP_InputField lobbycodeField)
    {
        int townID = int.Parse(lobbycodeField.text);
        postbox.Send(Letter.Get().WriteJoinTown(townID));
    }

    public void CreateTown()
    {
        postbox.Send(Letter.Get().Write(LetterType.CREATETOWN));
    }

    public void StartGame()
    {
        postbox.Send(Letter.Get().Write(LetterType.STARTGAME));
    }

    public void LeaveTown()
    {
        if (postbox == null)
        {
            Debug.LogAssertion("No Postbox");
        }
        postbox.Send(Letter.Get().Write(LetterType.LEAVETOWN));
        town = null;
        onLeaveTown?.Invoke();
    }
    #endregion

    void Initiate()
    {
        letterHandlers = new Dictionary<byte, PostOffice.LetterHandler>()
        {
            {(byte)LetterType.WELCOME,HandleWelcome },
            {(byte)LetterType.TOWNWELCOME,HandleTownWelcome }
        };

        postbox = new Postbox();
        postbox.onLetter += (_,letter) =>
        {
            letterHandlers[letter.ReadByte()](_,letter);
        };
        record = new ResidentRecord(postbox);
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Initiate();
    }

    void FixedUpdate()
    {
        if (postbox != null) 
        {
            bool stillConnected = postbox.ReceiveData();
            if (connected && !stillConnected)
            {
                onDisconnected?.Invoke();
            }
            connected = stillConnected;
        }
    }

    void OnApplicationQuit()
    {
        if (postbox != null)
        {
            postbox.Close();
        }
    }
}
