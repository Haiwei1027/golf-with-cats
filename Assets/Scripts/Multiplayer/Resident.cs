using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using UnityEngine.Assertions;

/// <summary>
/// Class responsible for connecting to the server and handling network traffic to and from the server
/// </summary>
public class Resident : MonoBehaviour
{
    public static Resident Instance { get; private set; }

    [SerializeField] string ServerIP;
    [SerializeField] bool useLoopback;

    public static int Id { get { return Instance.record.Id; } }

    public ResidentRecord record;
    public TownRecord Town { get { return record.Town; } }

    public Postbox Postbox { get; private set; }
    public ResidentLetterHandler LetterHandler { get { return (ResidentLetterHandler)Postbox.letterHandler; } }

    private bool connected;

    

    #region UI Methods
    public static void Connect(string username)
    {
        if (Instance.record == null)
        {
            Instance.Initiate();
        }

        Instance.record.Username = username;
        Debug.Log("Connecting");
        Instance.Postbox.Connect(new IPEndPoint(Instance.useLoopback ? IPAddress.Loopback : IPAddress.Parse(Instance.ServerIP),PostOffice.Port));
    }

    public static void Disconnect()
    {
        Instance.Postbox.Close();
        Instance.record = null;
    }

    public static void JoinTown(string lobbyCode)
    {
        int townID = int.Parse(lobbyCode);
        Instance.Postbox.Send(LetterFactory.Get().WriteJoinTown(townID));
    }

    public static void CreateTown()
    {
        Instance.Postbox.Send(LetterFactory.Get().Write(LetterType.CREATETOWN));
    }

    public static void StartGame()
    {
        Instance.Postbox.Send(LetterFactory.Get().Write(LetterType.STARTGAME));
    }

    public static void LeaveTown()
    {
        Instance.Postbox.Send(LetterFactory.Get().Write(LetterType.LEAVETOWN));
    }
    #endregion

    public static void SendLetter(Letter letter)
    {
        Instance.Postbox.Send(letter);
    }

    void Initiate()
    {
        Postbox = new Postbox(new ResidentLetterHandler());
        record = new ResidentRecord(Postbox);
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        Initiate();
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (Postbox != null) 
        {
            bool stillConnected = Postbox.ReceiveData();
            if (connected && !stillConnected)
            {
                Postbox.Close();
            }
            connected = stillConnected;
        }
    }

    void OnApplicationQuit()
    {
        if (Postbox != null)
        {
            Postbox.Close();
        }
    }
}
