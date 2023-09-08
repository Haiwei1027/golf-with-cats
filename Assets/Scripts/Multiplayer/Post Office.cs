using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

/// <summary>
/// Class responsible for handling network traffic to and from clients and manage lobbies
/// </summary>
public class PostOffice : MonoBehaviour
{
    private static PostOffice instance;

    public static readonly int Port = 25569;
    public static readonly int SocketBufferSize = 1024 * 1024;
    public static readonly int Capacity = 64;

    private List<ResidentRecord> residents = new List<ResidentRecord>();
    private List<ResidentRecord> leavers = new List<ResidentRecord>();

    private List<int> abandonedTowns = new List<int>();
    private Dictionary<int,Town> towns = new Dictionary<int,Town>();

    private Socket serverSocket;

    private bool listening = false;
    private bool accepting = false;

    private System.Random randomGenerator;

    public static Town MakeTown(ResidentRecord founder)
    {
        Town town = new Town(founder);
        instance.towns.Add(town.Id, town);
        return town;
    }

    public static Town GetTown(int id)
    {
        return instance.towns.GetValueOrDefault(id);
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
        Debug.Log($"No Resident of ID {id} Was Found");
        return null;
    }

    public void Awake()
    {
        Application.runInBackground = true;
        instance = this;
    }

    void Start()
    {
        Debug.Log("Starting");
        serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp)
        {
            SendBufferSize = SocketBufferSize,
            ReceiveBufferSize = SocketBufferSize
        };
        Debug.Log("Binding");
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
        Debug.Log("Accepting");
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
        Debug.Log("Accepted");
        
        Postbox newPostbox = new Postbox(clientSocket, new PostOfficeLetterHandler());

        int id = GenerateUserID();
        Letter welcomeLetter = LetterFactory.Get().WriteWelcome(id);
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

        Town town;
        foreach (int townId in towns.Keys)
        {
            town = towns[townId];
            town.Update();
            if (town.Population <= 0)
            {
                abandonedTowns.Add(townId);
            }
        }

        
        foreach (int townId in abandonedTowns)
        {
            towns.Remove(townId);
        }
        abandonedTowns.Clear();
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
