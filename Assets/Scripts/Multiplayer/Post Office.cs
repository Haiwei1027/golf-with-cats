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
    public static readonly int Port = 25569;
    public static readonly int SocketBufferSize = 1024 * 1024;
    public static readonly int Capacity = 64;

    private static Dictionary<int, ResidentRecord> residents = new Dictionary<int, ResidentRecord>();

    private static Dictionary<int,Town> towns = new Dictionary<int,Town>();

    private static Socket serverSocket;

    private static bool listening = false;
    private static bool accepting = false;

    public static System.Random randomGenerator;

    public static PostOfficeLetterHandler letterHandler;

    public static Town MakeTown(ResidentRecord founder)
    {
        Town town = new Town(founder);
        towns.Add(town.Id, town);
        return town;
    }

    public static Town GetTown(int id)
    {
        try
        {
            return towns[id];
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        return null;
    }

    public static void DestroyTown(int id)
    {
        try
        {
            towns.Remove(id);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public static ResidentRecord GetResident(int id)
    {
        try
        {
            return residents[id];
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        return null;
    }

    public static void KickResident(int id)
    {
        try
        {
            residents.Remove(id);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void Awake()
    {
        Application.runInBackground = true;
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
        letterHandler = new PostOfficeLetterHandler();
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
        
        Postbox newPostbox = new Postbox(clientSocket, letterHandler);

        int id = GenerateUserID();
        Letter welcomeLetter = LetterFactory.Get().WriteWelcome(id);
        newPostbox.Send(welcomeLetter);

        residents.Add(id, new ResidentRecord(newPostbox, id));
        accepting = false;
    }

    void Disconnect(ResidentRecord resident)
    {
        GetTown(resident.Town.Id).Leave(resident);
        resident.Postbox.Close();
    }

    void FixedUpdate()
    {
        AcceptConnection();
        bool stillConnected;
        foreach (ResidentRecord resident in residents.Values)
        {
            stillConnected = resident.Postbox.ReceiveData();
        }

        Town town;
        foreach (int townId in towns.Keys)
        {
            town = towns[townId];
            town.Update();
        }
    }

    void OnApplicationQuit()
    {
        if (serverSocket != null)
        {
            serverSocket.Close();
        }
        foreach (ResidentRecord resident in residents.Values)
        {
            Disconnect(resident);
        }
    }
}
