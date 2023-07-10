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
    public static readonly int socketBufferSize = 1024 * 1024;

    private Socket serverSocket;

    void Start()
    {
        serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp)
        {
            SendBufferSize = socketBufferSize,
            ReceiveBufferSize = socketBufferSize
        };
        serverSocket.Bind(new IPEndPoint(IPAddress.Any,Port));
    }

    void FixedUpdate()
    {
        // checks if socket is readable
        if (serverSocket.Poll(0, SelectMode.SelectRead)){
            Socket clientSocket = serverSocket.Accept();
            Postbox postbox = new Postbox(clientSocket);
        }   
    }
}
