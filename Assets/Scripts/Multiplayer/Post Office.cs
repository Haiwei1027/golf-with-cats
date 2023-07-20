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
        Debug.LogError($"{username} Connected");
        letter.Release();
    }

    void Start()
    {
        Debug.LogError("Initialising");

        letterHandlers = new Dictionary<byte, LetterHandler>()
        {
            {(byte)LetterType.INTRODUCE, HandleIntroduce}
        };

        Debug.LogError("Starting");
        serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp)
        {
            SendBufferSize = SocketBufferSize,
            ReceiveBufferSize = SocketBufferSize
        };
        Debug.LogError("Binding");
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
        Debug.LogError("Accepting");
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
        Debug.LogError("Accepted");
        Postbox newPostbox = new Postbox(clientSocket);
        newPostbox.onLetter += (postbox, letter) =>
        {
            letterHandlers[letter.ReadByte()](postbox, letter);
        };
        newPostbox.Id = GenerateUserID();
        Letter welcomeLetter = Letter.GetWelcome(newPostbox.Id);
        newPostbox.Send(welcomeLetter);

        postboxes.Add(newPostbox);
        accepting = false;
    }

    void FixedUpdate()
    {
        AcceptConnection();
        foreach (Postbox postbox in postboxes)
        {
            postbox.ReceiveData();
        }
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
