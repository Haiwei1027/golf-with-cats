using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
/// <summary>
/// This class contains server logic
/// It handles incoming connections and scheduling
/// </summary>
public class PostOffice : MonoBehaviour
{
    public static readonly int Port = 7767;
    public static readonly int SocketBufferSize = 1024 * 1024;
    public static readonly int MaxPlayers = 10;

    private List<Postbox> postboxes = new List<Postbox>();

    private Socket serverSocket;

    void Start()
    {

        serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp)
        {
            SendBufferSize = SocketBufferSize,
            ReceiveBufferSize = SocketBufferSize
        };
        serverSocket.Bind(new IPEndPoint(IPAddress.Any,Port));
    }

    void AcceptConnection()
    {
        // checks if socket is readable
        if (!serverSocket.Poll(0, SelectMode.SelectRead)) return;
        // checks server not full
        if (postboxes.Count >= MaxPlayers) return;
        Socket clientSocket = serverSocket.Accept();
        postboxes.Add(new Postbox(clientSocket));
    }

    void FixedUpdate()
    {
        AcceptConnection();
        foreach(Postbox postbox in postboxes)
        {
            postbox.ReceiveData();
        }
    }
}
