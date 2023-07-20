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
    public static readonly int MaxPlayers = 10;

    private List<Postbox> postboxes = new List<Postbox>();
    private List<Postbox> disconnected = new List<Postbox>();

    private Socket serverSocket;

    public delegate void LetterHandler(Postbox postbox, Letter letter);
    public static Dictionary<byte, LetterHandler> letterHandlers;

    private bool listening = false;
    private bool accepting = false;

    private System.Random randomGenerator;

    public void HandleIntroduce(Postbox postbox, Letter letter)
    {
        string username = letter.ReadString();
        postbox.Username = username;
        Debug.LogAssertion($"{username} Connected");
        letter.Release();
    }

    void Start()
    {
        Debug.LogAssertion("Initialising");

        letterHandlers = new Dictionary<byte, LetterHandler>()
        {
            {(byte)LetterType.INTRODUCE, HandleIntroduce}
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
        if (postboxes.Count >= MaxPlayers) return;
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
        newPostbox.Id = GenerateUserID();
        Letter welcomeLetter = Letter.Get().WriteWelcome(newPostbox.Id);
        newPostbox.Send(welcomeLetter);

        postboxes.Add(newPostbox);
        accepting = false;
    }

    void FixedUpdate()
    {
        AcceptConnection();
        bool stillConnected;
        foreach (Postbox postbox in postboxes)
        {
            stillConnected = postbox.ReceiveData();
            if (!stillConnected)
            {
                disconnected.Add(postbox);
                Debug.Log($"{postbox.Username} Disconnected");
                postbox.Close();
            }
        }
        foreach (Postbox postbox in disconnected)
        {
            postboxes.Remove(postbox);
        }
        disconnected.Clear();
    }

    void OnApplicationQuit()
    {
        if (serverSocket != null)
        {
            serverSocket.Close();
        }
        foreach (Postbox postbox in postboxes)
        {
            postbox.Close();
        }
    }
}
