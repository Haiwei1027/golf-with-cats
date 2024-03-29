using UnityEngine;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Class responsible for abstracting the process of sending data over the network
/// </summary>
public class Postbox
{
    private Socket socket;
    
    private int expectLetterLength;
    private byte[] receiveBuffer = new byte[PostOffice.SocketBufferSize];
    private byte[] sendBuffer = new byte[PostOffice.SocketBufferSize];
    private byte[] headerBytes = new byte[sizeof(ushort)];

    public LetterHandler letterHandler;

    private ResidentRecord owner;
    public ResidentRecord Owner
    {
        get { return owner; }
        set { owner = value; }
    }
    private static Socket CreateSocket()
    {
        return new Socket(SocketType.Stream, ProtocolType.Tcp)
        {
            SendBufferSize = PostOffice.SocketBufferSize,
            ReceiveBufferSize = PostOffice.SocketBufferSize
        };
    }

    public Postbox(LetterHandler letterHandler) : this(CreateSocket(), letterHandler) { }

    public Postbox(Socket socket, LetterHandler letterHandler)
    {
        this.socket = socket;
        this.letterHandler = letterHandler;
    }

    public void Connect(IPEndPoint endpoint)
    {
        if (socket == null)
        {
            CreateSocket();
        }
        socket.Connect(endpoint);
    }

    public IPEndPoint GetIP()
    {
        return socket.RemoteEndPoint as IPEndPoint;
    }

    private void HandleData(int amount)
    {
        Letter letter = LetterFactory.Get();
        letter.Copy(receiveBuffer, amount);

        Debug.Log($"Received {(LetterType)receiveBuffer[0]}");

        letterHandler.Handle(owner, letter);
    }

    public bool ReceiveData()
    {
        if (!socket.Connected) { return false; }
        bool moreLetters = false;
        do
        {
            int receivedLetterSize = 0;
            try
            {
                // try receive content
                if (expectLetterLength > 0 )
                {
                    if (socket.Available >= expectLetterLength)
                    {
                        receivedLetterSize = socket.Receive(receiveBuffer, expectLetterLength, SocketFlags.None);
                        expectLetterLength = 0;
                        moreLetters = true;
                    }
                    
                }
                // try receive header
                else if (socket.Available >= Letter.HeaderSize)
                {
                    socket.Receive(headerBytes, Letter.HeaderSize, SocketFlags.None);
                    expectLetterLength = Letter.ReadHeader(headerBytes);
                    moreLetters = true;
                }
                else
                {
                    moreLetters = false;
                }
            }
            catch (SocketException ex)
            {
                Debug.LogException(ex);
            }
            if (receivedLetterSize > 0)
            {
                HandleData(receivedLetterSize);
            }
        } while (moreLetters);
        return true;
    }

    public void Send(Letter letter, bool release = true)
    {
        try
        {
            ushort amount = letter.Ready(sendBuffer, 0);

            Debug.Log($"Sending {(LetterType)sendBuffer[2]}");

            socket.Send(sendBuffer, amount, SocketFlags.None);
            if (!release)
            {
                return;
            }
            letter.Release();
        }
        catch (SocketException ex)
        {
            Debug.LogException(ex);
        }
    }

    public void Close()
    {
        letterHandler.Close(this);
        if (socket != null)
        {
            socket.Close();
        }
    }
}
