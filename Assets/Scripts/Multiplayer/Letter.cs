using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
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

    public static Letter GetWelcome(int id)
    {
        Letter letter = Get();
        letter.Write(LetterType.WELCOME);
        letter.Write(id);

        return letter;
    }

    public static Letter GetIntroduce(string username)
    {
        Letter letter = Get();
        letter.Write(LetterType.INTRODUCE);
        letter.Write(username);
        
        return letter;
    }

    public static Letter Get()
    {
        return pooledLetters.Get();
    }

    public void Clear()
    {
        pointer = HeaderSize;
    }

    public void Release()
    {
        pooledLetters.Release(this);
    }

    public void Copy(byte[] buffer, int amount)
    {
        Array.Copy(buffer,0,bytes,HeaderSize, amount);
    }

    public ushort Ready(byte[] array, int startIndex)
    {
        WriteHeader();
        Array.Copy(bytes, 0, array, startIndex, pointer);
        return pointer;
    }

    public void WriteHeader()
    {
        bytes[0] = (byte)((pointer - HeaderSize) >> 8);
        bytes[1] = (byte)(pointer - HeaderSize);
    }

    public static ushort ReadHeader(byte[] headerBytes)
    {
        return (ushort)(headerBytes[1] | (byte)(headerBytes[1] << 8));
    }

    public void Write(byte value)
    {
        bytes[pointer] = value;
        pointer++;
    }

    public byte ReadByte()
    {
        byte value = bytes[pointer];
        pointer++;
        return value;
    }

    public void Write(LetterType value)
    {
        Write((byte)value);
    }

    public LetterType ReadType()
    {
        return (LetterType)ReadByte();
    }

    public void Write(float value)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, pointer);
        pointer += 4;
    }

    public float ReadFloat()
    {
        float value = BitConverter.ToSingle(bytes, pointer);
        pointer += 4;
        return value;

    }

    public void Write(int value)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, pointer);
        pointer += 4;
    }

    public int ReadInt()
    {
        int value = BitConverter.ToInt32(bytes, pointer);
        pointer += 4;
        return value;
    }

    public void Write(char value)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, pointer);
        pointer += 2;
    }

    public char ReadChar()
    {
        char value = BitConverter.ToChar(bytes, pointer);
        pointer += 2;
        return value;

    }

    public void Write(string value)
    {
        Write(value.Length);
        foreach (char c in value)
        {
            Write(c);
        }
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
}
