using System.Collections;
using System.Collections.Generic;
using System;

public class Converter
{

    public static int Write(float value, byte[] outputArray, int startIndex)
    {
        Array.Copy(BitConverter.GetBytes(value),0,outputArray,startIndex,4);
        return sizeof(float);
    }
    public static int ReadFloat(out float value, byte[] inputArray, int startIndex)
    {
        value = BitConverter.ToSingle(inputArray, startIndex);
        return sizeof(float);
    }
}
