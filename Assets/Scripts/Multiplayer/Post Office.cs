using System.Collections;
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

    private bool accepting = false;

    public void HandleGreet(Postbox postbox, Letter letter)
    {
        ScreenConsole.Write($"{postbox.GetIP()}:{ letter.ReadString() }");
    }

    void Start()
    {
        ScreenConsole.Write("Initialising");

        letterHandlers = new Dictionary<byte, LetterHandler>()
        {
            {(byte)LetterType.GREET, HandleGreet}
        };

        ScreenConsole.Write("Starting");
        serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp)
        {
            SendBufferSize = SocketBufferSize,
            ReceiveBufferSize = SocketBufferSize
        };
        ScreenConsole.Write("Binding");
        serverSocket.Bind(new IPEndPoint(IPAddress.Any,Port));
        serverSocket.Listen(0);
    }

    void AcceptConnection()
    {
        if (accepting) return;
        if (serverSocket == null) return;
        if (!serverSocket.IsBound) return;
        // checks server not full
        if (postboxes.Count >= MaxPlayers) return;
        ScreenConsole.Write("Accepting");
        serverSocket.BeginAccept(AcceptCallback,null);
        accepting = true;
        
    }

    void AcceptCallback(IAsyncResult result)
    {
        Socket clientSocket = serverSocket.EndAccept(result);
        postboxes.Add(new Postbox(clientSocket));
        accepting = false;
    }

    void FixedUpdate()
    {
        AcceptConnection();
        foreach(Postbox postbox in postboxes)
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
