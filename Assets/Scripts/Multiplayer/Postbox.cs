using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    private byte[] headerBytes = new byte[sizeof(ushort)];

    public Postbox(Socket socket)
    {
        this.socket = socket;
    }

    private void HandleData()
    {

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
                    if (socket.Available >= expectLetterLength)
                    {
                        receivedLetterSize = socket.Receive(receiveBuffer, expectLetterLength, SocketFlags.None);
                        expectLetterLength = 0;
                        moreLetters = true;
                    }
                    
                }
                // try receive header
                else if (socket.Available >= sizeof(ushort))
                {
                    socket.Receive(headerBytes, sizeof(ushort), SocketFlags.None);
                    expectLetterLength = Letter.ReadHeader(headerBytes);
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine(ex);
            }
            if (receivedLetterSize > 0)
            {
                HandleData();
            }
        } while (moreLetters);
    }

    public void Send()
    {

    }
}
