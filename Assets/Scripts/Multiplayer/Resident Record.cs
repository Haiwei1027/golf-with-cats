using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains fields to store resident data independent of its logic
/// </summary>
public class ResidentRecord
{
    private int id;
    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    private Postbox postbox;
    public Postbox Postbox
    {
        get { return postbox;}
        set { postbox = value; }
    }
    private string username;
    public string Username
    {
        get { return username; }
        set { username = value; }
    }
    private TownRecord town;
    public TownRecord Town
    {
        get { return town; }
        set { town = value; }
    }
    private int colourId;
    public int ColourId
    {
        get { return colourId; }
        set { colourId = value; }
    }

    /// <summary>
    /// Created by server to store serverside playerdata
    /// </summary>
    /// <param name="postbox"></param>
    /// <param name="id"></param>
    public ResidentRecord(Postbox postbox, int id)
    {
        this.postbox = postbox;
        this.postbox.Owner = this;
        Id = id;
    }

    /// <summary>
    /// Create record instance to store local player data
    /// </summary>
    /// <param name="postbox"></param>
    public ResidentRecord(Postbox postbox)
    {
        this.postbox = postbox;
        this.postbox.Owner = this;
    }

    /// <summary>
    /// Create record instance to store remote player data
    /// </summary>
    /// <param name="id"></param>
    /// <param name="username"></param>
    public ResidentRecord(int id, string username, int colourId)
    {
        this.id = id;
        this.username = username;
        this.colourId = colourId;
    }
}
