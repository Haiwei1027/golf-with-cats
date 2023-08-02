using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Data.Common;

/// <summary>
/// Class for storing synchronised object data and formatting them to be sent by the transmitter class
/// </summary>
public abstract class Hologram
{

    private int id;
    public int Id { get { return id; } private set { id = value; } }

    protected HologramTransmitter source;
    protected byte[] dataBuffer;
    protected int bufferSize;
    
    protected Hologram(HologramTransmitter source)
    {
        id = new System.Random().Next(9999_9999 + 1);
        this.source = source;
        // child then need to initate the dataBuffer
    }

    protected Hologram(HologramTransmitter source, int id)
    {
        this.id = id;
        this.source = source;
    }
    /// <summary>
    /// Method to write data into letter
    /// </summary>
    /// <param name="outputArray">array to contain the bytes</param>
    /// <param name="startIndex">first index to be written</param>
    /// <returns>how much has been written</returns>
    public virtual ushort GetData(byte[] outputArray, int startIndex)
    {
        Converter.Write(Id, outputArray, startIndex);
        Array.Copy(dataBuffer, 0, outputArray, startIndex, dataBuffer.Length);
        return (ushort)dataBuffer.Length; //return number of bytes used to encode this hologram
    }
    /// <summary>
    /// Method to input letter data into the hologram
    /// </summary>
    /// <param name="inputArray">array containing the bytes</param>
    /// <param name="startIndex">first byte to read</param>
    public virtual void SetData(byte[] inputArray, int startIndex)
    {

    }

    public virtual void ApplyData()
    {
        
    }
}

/// <summary>
/// This is an hologram that contains transform data
/// </summary>
public class TransformHologram : Hologram //example of a hologram
{
    private Transform transform;
    public TransformHologram(HologramTransmitter source) : base(source)
    {
        dataBuffer = new byte[sizeof(int) + sizeof(float)*9];
        transform = source.GetComponent<Transform>();
    }

    public override ushort GetData(byte[] outputArray, int startIndex)
    {
        int writeIndex = startIndex + sizeof(int);

        writeIndex += Converter.Write(transform.position.x,outputArray, writeIndex);
        writeIndex += Converter.Write(transform.position.y, outputArray, writeIndex);
        writeIndex += Converter.Write(transform.position.z, outputArray, writeIndex);

        writeIndex += Converter.Write(transform.rotation.x, outputArray, writeIndex);
        writeIndex += Converter.Write(transform.rotation.y, outputArray, writeIndex);
        writeIndex += Converter.Write(transform.rotation.z, outputArray, writeIndex);

        writeIndex += Converter.Write(transform.localScale.x, outputArray, writeIndex);
        writeIndex += Converter.Write(transform.localScale.y, outputArray, writeIndex);
        writeIndex += Converter.Write(transform.localScale.z, outputArray, writeIndex);
        return base.GetData(outputArray, startIndex);
    }

    
}
