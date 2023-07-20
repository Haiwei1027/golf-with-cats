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

    private int id;

    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    private string username;

    public string Username
    {
        get { return username; }
        set { username = value; }
    }

    public Postbox(Socket socket)
    {
        this.socket = socket;
    }

    public void Connect(IPEndPoint endpoint)
    {
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
        onLetter?.Invoke(this, letter);
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
                    Debug.LogAssertion("Expect " + expectLetterLength);
                    if (socket.Available >= expectLetterLength)
                    {
                        Debug.LogAssertion("Availiable " + socket.Available);
                        receivedLetterSize = socket.Receive(receiveBuffer, expectLetterLength, SocketFlags.None);
                        expectLetterLength = 0;
                        moreLetters = true;
                    }
                    
                }
                // try receive header
                else if (socket.Available >= Letter.HeaderSize)
                {
                    Debug.LogAssertion("Header");
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

    public void Send(Letter letter)
    {
        try
        {
            ushort amount = letter.Ready(sendBuffer, 0);
            Debug.LogAssertion("Prepared");
            socket.Send(sendBuffer, amount, SocketFlags.None);
            Debug.LogAssertion("Sent");
            letter.Release();
            Debug.LogAssertion("Released");
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
