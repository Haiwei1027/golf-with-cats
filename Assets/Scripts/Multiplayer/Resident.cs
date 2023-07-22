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

    public delegate void LetterHandler(Letter letter);
    public static Dictionary<byte, LetterHandler> letterHandlers;

    public static event Action onConnected;
    public static event Action onDisconnected;

    public static event Action onJoinTown;
    public static event Action onLeaveTown;

    ResidentRecord record;

    List<ResidentRecord> townResidents = new List<ResidentRecord>();

    Postbox postbox;

    private bool connected;

    #region Letter Handlers
    void HandleWelcome(Letter letter)
    {
        Debug.LogAssertion("Connected");
        record.Id = letter.ReadInt();
        Debug.LogAssertion($"ID: {record.Id}");
        
        letter.Clear();
        letter.WriteIntroduce(record.Username);
        postbox.Send(letter);

        onConnected?.Invoke();
    }

    void HandleTownWelcome(Letter letter)
    {
        int townId = letter.ReadInt();
        int population = letter.ReadInt();
        for (int i = 0; i < population; i++)
        {
            ResidentRecord otherResident = new ResidentRecord(letter.ReadInt(), letter.ReadString());
            otherResident.TownId = townId;
            townResidents.Add(otherResident);
        }
        record.TownId = townId;
        letter.Release();

        onJoinTown?.Invoke();
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
        postbox.Send(Letter.Get().Write(LetterType.LEAVETOWN));

        onLeaveTown?.Invoke();
    }
    #endregion

    void Initiate()
    {
        letterHandlers = new Dictionary<byte, LetterHandler>()
        {
            {(byte)LetterType.WELCOME,HandleWelcome },
            {(byte)LetterType.TOWNWELCOME,HandleTownWelcome }
        };

        postbox = new Postbox();
        postbox.onLetter += (postbox, letter) =>
        {
            letterHandlers[letter.ReadByte()](letter);
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
            if (connected && !postbox.ReceiveData())
            {
                onDisconnected?.Invoke();
            }
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
