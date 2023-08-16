using UnityEngine;
using System.Net;
using System.Net.Sockets;
/// <summary>
/// This class acts as an interface to another peer(client or server)
/// It handles sending and receiving of data from the connected socket
/// </summary>
public class Postbox
{
    private Socket socket;
    
    private int expectLetterLength;
    private byte[] receiveBuffer = new byte[PostOffice.SocketBufferSize];
    private byte[] sendBuffer = new byte[PostOffice.SocketBufferSize];
    private byte[] headerBytes = new byte[sizeof(ushort)];

    public event PostOffice.LetterHandler onLetter;

    private ResidentRecord owner;
    public ResidentRecord Owner
    {
        get { return owner; }
        set { owner = value; }
    }
    private void CreateSocket()
    {
        socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
        {
            SendBufferSize = PostOffice.SocketBufferSize,
            ReceiveBufferSize = PostOffice.SocketBufferSize
        };
    }

    public Postbox()
    {
        CreateSocket();
    }

    public Postbox(Socket socket)
    {
        this.socket = socket;
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
        Letter letter = Letter.Get();
        letter.Copy(receiveBuffer, amount);
        Debug.LogAssertion("Got Letter " + (LetterType)receiveBuffer[0]);
        onLetter?.Invoke(owner, letter);
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
                    //Debug.LogAssertion("Expect " + expectLetterLength);
                    if (socket.Available >= expectLetterLength)
                    {
                        //Debug.LogAssertion("Availiable " + socket.Available);
                        receivedLetterSize = socket.Receive(receiveBuffer, expectLetterLength, SocketFlags.None);
                        expectLetterLength = 0;
                        moreLetters = true;
                    }
                    
                }
                // try receive header
                else if (socket.Available >= Letter.HeaderSize)
                {
                    //Debug.LogAssertion("Header");
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
                Debug.LogAssertion(ex);
            }
            if (receivedLetterSize > 0)
            {
                HandleData(receivedLetterSize);
            }
        } while (moreLetters);
        return true;
    }

    public void Send(Letter letter, bool reusingLetter = false)
    {
        try
        {
            ushort amount = letter.Ready(sendBuffer, 0);
            Debug.LogAssertion("Prepared " + (LetterType)sendBuffer[2]);
            socket.Send(sendBuffer, amount, SocketFlags.None);
            //Debug.LogAssertion("Sent");
            if (reusingLetter)
            {
                return;
            }
            letter.Release();
            //Debug.LogAssertion("Released");
        }
        catch (SocketException ex)
        {
            Debug.LogAssertion(ex);
        }
    }

    public void Close()
    {
        if (socket != null)
        {
            socket.Close();
        }
    }
}
