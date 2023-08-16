using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
/// <summary>
/// This class contains server logic
/// It handles incoming connections and scheduling
/// </summary>
public class PostOffice : MonoBehaviour
{
    public static readonly int Port = 25569;
    public static readonly int SocketBufferSize = 1024 * 1024;
    public static readonly int Capacity = 64;

    private List<ResidentRecord> residents = new List<ResidentRecord>();
    private List<ResidentRecord> leavers = new List<ResidentRecord>();

    private List<Town> towns = new List<Town>();

    private Socket serverSocket;

    public delegate void LetterHandler(ResidentRecord sender, Letter letter);
    public static Dictionary<byte, LetterHandler> letterHandlers;

    private bool listening = false;
    private bool accepting = false;

    private System.Random randomGenerator;

    #region Letter Handlers
    public void HandleIntroduce(ResidentRecord sender, Letter letter)
    {
        string username = letter.ReadString();
        sender.Username = username;
        Debug.LogAssertion($"{username} Connected");
        letter.Release();
    }

    public void HandleCreateTown(ResidentRecord sender, Letter letter)
    {
        towns.Add(new Town(sender, 4));
    }

    public void HandleJoinTown(ResidentRecord sender, Letter letter)
    {
        Town town = GetTown(letter.ReadInt());
        town.Join(sender);
    }

    public void HandleLeaveTown(ResidentRecord sender, Letter letter)
    {
        GetTown(sender.Town.Id).Leave(sender);
    }

    public void HandleHologramCreate(ResidentRecord sender, Letter letter)
    {
        GetTown(sender.Town.Id).hologramDatabase.Add(sender,letter);
    }

    public void HandleHologramUpdate(ResidentRecord sender, Letter letter)
    {
        if (GetTown(sender.Town.Id) == null) { Debug.LogAssertion("Town is null"); }
        if (GetTown(sender.Town.Id).hologramDatabase == null) { Debug.LogAssertion("HologramDataBase is null"); }
        GetTown(sender.Town.Id).hologramDatabase.Update(sender,letter);
    }

    public void HandleHologramDestroy(ResidentRecord sender, Letter letter)
    {
        GetTown(sender.Town.Id).hologramDatabase.Remove(sender,letter);
    }

    #endregion

    public Town GetTown(int id)
    {
        foreach (Town t in towns)
        {
            if (t.Id == id)
            {
                return t;
            }
        }
        Debug.LogAssertion($"No Town of ID {id} Was Found");
        return null;
    }

    public ResidentRecord GetResident(int id)
    {
        foreach (ResidentRecord r in residents)
        {
            if (r.Id == id)
            {
                return r;
            }
        }
        Debug.LogAssertion($"No Resident of ID {id} Was Found");
        return null;
    }

    public void Awake()
    {
        Application.runInBackground = true;
    }

    void Start()
    {
        Debug.LogAssertion("Initialising");

        letterHandlers = new Dictionary<byte, LetterHandler>()
        {
            {(byte)LetterType.INTRODUCE, HandleIntroduce},
            {(byte)LetterType.CREATETOWN, HandleCreateTown},
            {(byte)LetterType.JOINTOWN,HandleJoinTown },
            {(byte)LetterType.LEAVETOWN, HandleLeaveTown },
            {(byte)LetterType.HOLOGRAMCREATE, HandleHologramCreate},
            {(byte)LetterType.HOLOGRAMUPDATE, HandleHologramUpdate},
            {(byte)LetterType.HOLOGRAMDESTROY, HandleHologramDestroy }
        };

        Debug.LogAssertion("Starting");
        serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp)
        {
            SendBufferSize = SocketBufferSize,
            ReceiveBufferSize = SocketBufferSize
        };
        Debug.LogAssertion("Binding");
        serverSocket.Bind(new IPEndPoint(IPAddress.Any,Port));
        serverSocket.Listen(0);
        listening = true;
    }

    void AcceptConnection()
    {
        if (accepting || !listening) return;
        if (serverSocket == null) return;
        if (!serverSocket.IsBound) return;
        if (residents.Count >= Capacity) return;
        Debug.LogAssertion("Accepting");
        serverSocket.BeginAccept(AcceptCallback,null);
        accepting = true;
        
    }

    int GenerateUserID()
    {
        if (randomGenerator == null)
        {
            randomGenerator = new System.Random();
        }
        return randomGenerator.Next(9999_9999 + 1);
    }

    void AcceptCallback(IAsyncResult result)
    {
        Socket clientSocket = serverSocket.EndAccept(result);
        Debug.LogAssertion("Accepted");
        
        Postbox newPostbox = new Postbox(clientSocket);
        newPostbox.onLetter += (postbox, letter) =>
        {
            letterHandlers[letter.ReadByte()](postbox, letter);
        };

        int id = GenerateUserID();
        Letter welcomeLetter = Letter.Get().WriteWelcome(id);
        newPostbox.Send(welcomeLetter);

        residents.Add(new ResidentRecord(newPostbox, id));
        accepting = false;
    }

    void Disconnect(ResidentRecord resident)
    {
        resident.Postbox.Close();
        GetTown(resident.Town.Id).Leave(resident);   
    }

    void FixedUpdate()
    {
        AcceptConnection();
        bool stillConnected;
        foreach (ResidentRecord resident in residents)
        {
            stillConnected = resident.Postbox.ReceiveData();
            if (!stillConnected)
            {
                leavers.Add(resident);
                Debug.Log($"{resident.Username} Disconnected");
                Disconnect(resident);
            }
        }
        foreach (ResidentRecord leaver in leavers)
        {
            residents.Remove(leaver);
        }
        leavers.Clear();
    }

    void OnApplicationQuit()
    {
        if (serverSocket != null)
        {
            serverSocket.Close();
        }
        foreach (ResidentRecord resident in residents)
        {
            Disconnect(resident);
        }
    }
}
