using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Numerics;
using System.Text;
using UnityEngine.Pool;
/// <summary>
/// This class acts as a data container and converter
/// It handles data formatting, serialisation and deserialisation
/// </summary> 

//this assumes big endian
public class Letter
{
    private static LinkedPool<Letter> pooledLetters = new LinkedPool<Letter>(() => new Letter(),(letter)=> letter.Clear());

    public static readonly ushort HeaderSize = sizeof(ushort);

    private byte[] bytes;
    private ushort pointer;
    

    public Letter()
    {
        bytes = new byte[4 * 1024];
        pointer = HeaderSize;
    }

    public string DebugBytes()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(ReadHeader(bytes));
        foreach (byte b in bytes)
        {
            sb.Append($" {b} ");
        }

        return sb.ToString();
    }

    public Letter WriteWelcome(int id)
    {
        Write(LetterType.WELCOME);
        Write(id);

        return this;
    }

    public Letter WriteIntroduce(string username)
    {
        Write(LetterType.INTRODUCE);
        Write(username);

        return this;
    }

    public Letter WriteJoinTown(int townID)
    {
        Write(LetterType.JOINTOWN); 
        Write(townID); 
        return this;
    }

    public Letter WriteTownWelcome(TownRecord town, ResidentRecord newResident)
    {
        Write(LetterType.TOWNWELCOME);
        Write(town.Id);
        Write(newResident.Id);
        Write(town.Population);
        foreach (ResidentRecord resident in town.Residents)
        {
            Write(resident.Id);
            Write(resident.Username);
        }
        return this;
    }

    public Letter WriteGoodbye(int id)
    {
        Write(LetterType.GOODBYE);
        Write(id);
        return this;
    }

    public static Letter Get()
    {
        return pooledLetters.Get();
    }

    public Letter Clear()
    {
        pointer = HeaderSize;
        return this;
    }

    public void Release()
    {
        pooledLetters.Release(this);
    }

    /// <summary>
    /// copies chunk of data into the letter directly without serialisation
    /// </summary>
    /// <param name="buffer">source array</param>
    /// <param name="amount">number of bytes to be copies starting from the front of the source array</param>
    public void Copy(byte[] buffer, int amount)
    {
        Array.Copy(buffer,0,bytes,HeaderSize, amount);
    }

    /// <summary>
    /// Prepares the letter by writing the header and copies the header and paylod into the array
    /// </summary>
    /// <param name="array">the buffer receiving the data</param>
    /// <param name="startIndex">the index the data will start to be written at</param>
    /// <returns></returns>
    public ushort Ready(byte[] array, int startIndex)
    {
        WriteHeader();
        Array.Copy(bytes, 0, array, startIndex, pointer);
        return pointer;
    }

    public Letter WriteHeader()
    {
        bytes[0] = (byte)((pointer - HeaderSize) >> 8);
        bytes[1] = (byte)(pointer - HeaderSize);
        return this;
    }

    public static ushort ReadHeader(byte[] headerBytes)
    {
        return (ushort)(headerBytes[1] | (byte)(headerBytes[1] << 8));
    }

    public Letter Write(byte value)
    {
        bytes[pointer] = value;
        pointer++;
        return this;
    }

    public byte ReadByte()
    {
        byte value = bytes[pointer];
        pointer++;
        return value;
    }

    public Letter Write(LetterType value)
    {
        Write((byte)value);
        return this;
    }

    public LetterType ReadType()
    {
        return (LetterType)ReadByte();
    }

    public Letter Write(float value)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, pointer);
        pointer += sizeof(float);
        return this;
    }

    public float ReadFloat()
    {
        float value = BitConverter.ToSingle(bytes, pointer);
        pointer += sizeof(float);
        return value;

    }

    public Letter Write(int value)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, pointer);
        pointer += sizeof(int);
        return this;
    }

    public int ReadInt()
    {
        int value = BitConverter.ToInt32(bytes, pointer);
        pointer += sizeof(int);
        return value;
    }

    public Letter Write(char value)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, pointer);
        pointer += sizeof(char);
        return this;
    }

    public char ReadChar()
    {
        char value = BitConverter.ToChar(bytes, pointer);
        pointer += sizeof(char);
        return value;

    }

    public Letter Write(string value)
    {
        Write(value.Length);
        foreach (char c in value)
        {
            Write(c);
        }
        return this;
    }

    public string ReadString()
    {
        StringBuilder sb = new StringBuilder();
        int length = ReadInt();
        for (int i = 0; i < length; i++)
        {
            sb.Append(ReadChar());
        }
        return sb.ToString();
    }

    public Letter Write(ushort value)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, pointer);
        pointer += sizeof(ushort);
        return this;
    }

    public ushort ReadUShort()
    {
        ushort value = BitConverter.ToUInt16(bytes, pointer);
        pointer += sizeof(ushort);
        return value;
    }

    public Letter Write(UnityEngine.Vector3 value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);

        return this;
    }

    public UnityEngine.Vector3 ReadVector3()
    {
        float x = ReadFloat();
        float y = ReadFloat();
        float z = ReadFloat();
        return new UnityEngine.Vector3(x,y,z);
    }
}
