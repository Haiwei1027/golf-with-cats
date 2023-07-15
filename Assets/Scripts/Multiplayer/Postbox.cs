using System;
using System.Collections;
using System.Collections.Generic;
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

    public Postbox(Socket socket)
    {
        this.socket = socket;
    }

    public IPEndPoint GetIP()
    {
        return socket.RemoteEndPoint as IPEndPoint;
    }

    private void HandleData(int amount)
    {
        Letter letter = Letter.Get();
        letter.Copy(receiveBuffer, amount);
        byte type = letter.ReadByte();
        PostOffice.letterHandlers[type](this, letter);
    }

    public void ReceiveData()
    {
        bool moreLetters = false;
        do
        {
            int receivedLetterSize = 0;
            try
            {
                // try receive content
                if (expectLetterLength > 0 )
                {
                    ScreenConsole.Write("Expect " + expectLetterLength);
                    if (socket.Available >= expectLetterLength)
                    {
                        ScreenConsole.Write("Availiable " + socket.Available);
                        receivedLetterSize = socket.Receive(receiveBuffer, expectLetterLength, SocketFlags.None);
                        expectLetterLength = 0;
                        moreLetters = true;
                    }
                    
                }
                // try receive header
                else if (socket.Available >= Letter.HeaderSize)
                {
                    ScreenConsole.Write("Header");
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
                Console.WriteLine(ex);
            }
            if (receivedLetterSize > 0)
            {
                HandleData(receivedLetterSize);
            }
        } while (moreLetters);
    }

    public void Send(Letter letter)
    {
        try
        {
            ushort amount = letter.Ready(sendBuffer, 0);
            socket.Send(sendBuffer, amount, SocketFlags.None);
            ScreenConsole.Write("Sent");
        }
        catch (SocketException ex)
        {
            Console.WriteLine(ex);
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
