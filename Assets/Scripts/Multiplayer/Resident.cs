using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// This class contains client logic
/// It handles synchronisation and sharing actions to peers
/// </summary>
public class Resident : MonoBehaviour
{
    float count = 0;

    Postbox postbox;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScreenConsole.Write("Starting");
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
            {
                SendBufferSize = PostOffice.SocketBufferSize,
                ReceiveBufferSize = PostOffice.SocketBufferSize
            };
            ScreenConsole.Write("Connecting");
            socket.Connect(new IPEndPoint(IPAddress.Loopback, PostOffice.Port));
            postbox = new Postbox(socket);
        }
        for (int i = 97; i <= 122; i++)
        {
            if (Input.GetKeyDown((KeyCode)i))
            {
                Letter letter = Letter.Get();
                letter.Write((byte)LetterType.GREET);
                letter.Write($"Hello, I pressed {(char)i}");
                postbox.Send(letter);
            }
        }
    }

    void FixedUpdate()
    {
        if (postbox != null) 
        {
            postbox.ReceiveData();
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
