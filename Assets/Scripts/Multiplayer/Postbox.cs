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

    public Postbox(Socket socket)
    {
        this.socket = socket;
    }

    public void Receive()
    {
        bool moreLetters = false;
        do
        {
            try
            {
                // try receive content
                if (expectLetterLength > 0 )
                {
                    if (socket.Available >= expectLetterLength)
                    {

                    }
                    
                }
                // try receive header
                else if (socket.Available >= sizeof(ushort))
                {
                    byte[] headerBytes = new byte[sizeof(ushort)];
                    socket.Receive(headerBytes, sizeof(ushort), SocketFlags.None);
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine(ex);
            }
        } while (moreLetters);
    }

    public void Send()
    {

    }
}
