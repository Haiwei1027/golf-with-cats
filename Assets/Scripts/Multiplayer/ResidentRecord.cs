using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public ResidentRecord(Postbox postbox, int id)
    {
        this.postbox = postbox;
        this.postbox.Owner = this;
        Id = id;
    }

    public ResidentRecord(Postbox postbox)
    {
        this.postbox = postbox;
        this.postbox.Owner = this;
    }

    public ResidentRecord(int id, string username)
    {
        this.id = id;
        this.username = username;
    }
}
