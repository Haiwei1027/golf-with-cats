using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using TMPro;
using System;
using UnityEngine.UIElements;

/// <summary>
/// This class contains client logic
/// It handles synchronisation and sharing actions to peers
/// </summary>
public class Resident : MonoBehaviour
{
    [SerializeField] string ServerIP;
    [SerializeField] bool useLoopback;

    public static Dictionary<byte, PostOffice.LetterHandler> letterHandlers;

    public static event Action onConnected;
    public static event Action onDisconnected;

    public static event Action onJoinTown;
    public static event Action onLeaveTown;

    Postbox postbox;

    private bool connected;

    #region Letter Handlers
    void HandleWelcome(Postbox postbox, Letter letter)
    {
        Debug.LogAssertion("Connected");
        postbox.Id = letter.ReadInt();
        Debug.LogAssertion($"ID: {postbox.Id}");
        onConnected?.Invoke();
        letter.Clear();
        letter.WriteIntroduce(postbox.Username);
        postbox.Send(letter);
    }

    void HandleTownWelcome(Postbox postbox, Letter letter)
    {

    }

    #endregion

    #region UI Methods
    public void Connect(TMP_InputField usernameField)
    {
        postbox.Username = usernameField.text;
        Debug.LogAssertion("Connecting");
        postbox.Connect(new IPEndPoint(useLoopback ? IPAddress.Loopback : IPAddress.Parse(ServerIP),PostOffice.Port));
    }

    public void Disconnect()
    {
        postbox.Close();
    }

    public void JoinTown(TMP_InputField lobbycodeField)
    {
        int townID = int.Parse(lobbycodeField.text);
        postbox.Send(Letter.Get().WriteJoinTown(townID));
    }

    public void CreateTown()
    {
        postbox.Send(Letter.Get().Write(LetterType.CREATETOWN));
    }

    public void StartGame()
    {
        postbox.Send(Letter.Get().Write(LetterType.STARTGAME));
    }

    public void LeaveTown()
    {
        postbox.Send(Letter.Get().Write(LetterType.LEAVETOWN));
    }
    #endregion

    void Start()
    {
        letterHandlers = new Dictionary<byte, PostOffice.LetterHandler>()
        {
            {(byte)LetterType.WELCOME,HandleWelcome },
            {(byte)LetterType.TOWNWELCOME,HandleTownWelcome }
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

    void FixedUpdate()
    {
        if (postbox != null) 
        {
            if (connected && !postbox.ReceiveData())
            {
                onDisconnected?.Invoke();
            }
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
