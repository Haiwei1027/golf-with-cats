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
    [SerializeField] string ServerIP;

    public static Dictionary<byte, PostOffice.LetterHandler> letterHandlers;

    Postbox postbox;

    void Start()
    {
        letterHandlers = new Dictionary<byte, PostOffice.LetterHandler>()
        {
            {(byte)LetterType.WELCOME,HandleWelcome }
        };

        Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
        {
            SendBufferSize = PostOffice.SocketBufferSize,
            ReceiveBufferSize = PostOffice.SocketBufferSize
        };
        postbox = new Postbox(socket);
        postbox.onLetter += (postbox, letter) =>
        {
            letterHandlers[letter.ReadByte()](postbox, letter);
        };
    }

    void HandleWelcome(Postbox postbox, Letter letter)
    {
        ScreenConsole.Write("Connected");
        postbox.Id = letter.ReadInt();
        ScreenConsole.Write($"ID: {postbox.Id}");

        letter.Release();
    }

    public void SetUsername(string username)
    {
        postbox.Username = username;
    }

    public void Connect()
    {
        ScreenConsole.Write("Connecting");
        postbox.Connect(new IPEndPoint(IPAddress.Parse(ServerIP),PostOffice.Port));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            postbox.Send(Letter.GetIntroduce(postbox.Username));
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
