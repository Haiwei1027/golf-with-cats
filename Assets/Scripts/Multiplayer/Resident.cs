using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
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
    public static event Action onStartGame;

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
            int displayColour = letter.ReadInt();
            if (id != record.Id)
            {
                ResidentRecord otherResident = new ResidentRecord(id, username, displayColour);
                town.AddResident(otherResident);
            }
            else
            {
                record.ColourId = displayColour;
                town.AddResident(record);
            }
        }

        letter.Release();

        onJoinTown?.Invoke(newResidentID);
    }

    public void HandleStartGame(ResidentRecord _, Letter letter)
    {
        onStartGame?.Invoke();
    }

    #endregion

    #region UI Methods
    public static void Connect(string username)
    {
        Instance.record.Username = username;
        Debug.LogAssertion("Connecting");
        Instance.postbox.Connect(new IPEndPoint(Instance.useLoopback ? IPAddress.Loopback : IPAddress.Parse(Instance.ServerIP),PostOffice.Port));
    }

    public static void Disconnect()
    {
        Instance.postbox.Close();
        Instance.record = null;
        Instance.town = null;
        onDisconnected?.Invoke();
    }

    public static void JoinTown(string lobbyCode)
    {
        int townID = int.Parse(lobbyCode);
        Instance.postbox.Send(Letter.Get().WriteJoinTown(townID));
    }

    public static void CreateTown()
    {
        Instance.postbox.Send(Letter.Get().Write(LetterType.CREATETOWN));
    }

    public static void StartGame()
    {
        Instance.postbox.Send(Letter.Get().Write(LetterType.STARTGAME));
    }

    public static void LeaveTown()
    {
        Instance.postbox.Send(Letter.Get().Write(LetterType.LEAVETOWN));
        Instance.town = null;
        onLeaveTown?.Invoke();
    }
    #endregion

    public static void SendLetter(Letter letter)
    {
        Instance.postbox.Send(letter);
    }

    void Initiate()
    {
        letterHandlers = new Dictionary<byte, PostOffice.LetterHandler>()
        {
            {(byte)LetterType.WELCOME,HandleWelcome },
            {(byte)LetterType.TOWNWELCOME,HandleTownWelcome },
            {(byte)LetterType.HOLOGRAMCREATE, HologramSystem.HandleCreate },
            {(byte)LetterType.HOLOGRAMUPDATE, HologramSystem.HandleUpdate },
            {(byte)LetterType.HOLOGRAMDESTROY, HologramSystem.HandleDestroy },
            {(byte)LetterType.STARTGAME, HandleStartGame}
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
