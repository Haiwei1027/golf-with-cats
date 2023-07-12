using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// This class acts as a data container and converter
/// It handles data formatting, serialisation and deserialisation
/// </summary> 

//this assumes big endian
public class Letter
{
    public Letter()
    {

    }

    public static ushort ReadHeader(byte[] headerBytes)
    {
        return (ushort)(headerBytes[1] | (byte)(headerBytes[1] << 8));
    }

    public static void WriteHeader(ushort header, byte[] headerBytes)
    {
        headerBytes[0] = (byte)(header >> 8);
        headerBytes[1] = (byte)header;
    }

    public static void WriteFloat(float value, byte[] array, int startIndex)
    {
        Span<byte> buffer = new Span<byte>(array);
        buffer.Slice(startIndex, 4);
        BitConverter.GetBytes(value).CopyTo(array, startIndex);
    }

    public static float ReadFloat(byte[] array, int startIndex)
    {
        return BitConverter.ToSingle(array, startIndex);
    }
}
