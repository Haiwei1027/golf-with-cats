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
    [SerializeField] bool useLoopback;

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
        Debug.LogAssertion("Connected");
        postbox.Id = letter.ReadInt();
        Debug.LogAssertion($"ID: {postbox.Id}");

        letter.Clear();
        letter.WriteIntroduce(postbox.Username);
        postbox.Send(letter);
    }

    public void SetUsername(string username)
    {
        postbox.Username = username;
    }

    public void Connect()
    {
        Debug.LogAssertion("Connecting");
        postbox.Connect(new IPEndPoint(useLoopback ? IPAddress.Loopback : IPAddress.Parse(ServerIP),PostOffice.Port));
    }

    void Update()
    {
        
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
