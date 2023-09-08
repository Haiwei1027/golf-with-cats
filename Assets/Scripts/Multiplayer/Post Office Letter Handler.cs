using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostOfficeLetterHandler : LetterHandler
{
    /*letterHandlers = new Dictionary<byte, LetterHandler>()
        {
            {(byte) LetterType.INTRODUCE, HandleIntroduce},
            { (byte)LetterType.CREATETOWN, HandleCreateTown},
            { (byte)LetterType.JOINTOWN,HandleJoinTown },
            { (byte)LetterType.LEAVETOWN, HandleLeaveTown },
            { (byte)LetterType.HOLOGRAMCREATE, HandleHologramCreate},
            { (byte)LetterType.HOLOGRAMUPDATE, HandleHologramUpdate},
            { (byte)LetterType.HOLOGRAMDESTROY, HandleHologramDestroy },
            { (byte)LetterType.STARTGAME, HandleStartGame }
        };*/

    public override void Handle(ResidentRecord postboxOwner, Letter letter)
    {
        LetterType type = letter.ReadType();
        switch (type)
        {
            default:
                Debug.LogError($"Unknown type {type}");
                return;
        }
    }

    public override void Close()
    {
        
    }

    public void HandleIntroduce(ResidentRecord sender, Letter letter)
    {
        string username = letter.ReadString();
        sender.Username = username;
        Debug.Log($"{username} Connected");
        letter.Release();
    }

    public void HandleCreateTown(ResidentRecord sender, Letter letter)
    {
        PostOffice.MakeTown(sender);
    }

    public void HandleJoinTown(ResidentRecord sender, Letter letter)
    {
        Town town = PostOffice.GetTown(letter.ReadInt());
        town.Join(sender);
    }

    public void HandleLeaveTown(ResidentRecord sender, Letter letter)
    {
        PostOffice.GetTown(sender.Town.Id).Leave(sender);
    }

    public void HandleHologramCreate(ResidentRecord sender, Letter letter)
    {
        PostOffice.GetTown(sender.Town.Id).hologramDatabase.Add(sender, letter);
    }

    public void HandleHologramUpdate(ResidentRecord sender, Letter letter)
    {
        PostOffice.GetTown(sender.Town.Id).hologramDatabase.Update(sender, letter);
    }

    public void HandleHologramDestroy(ResidentRecord sender, Letter letter)
    {
        PostOffice.GetTown(sender.Town.Id).hologramDatabase.Remove(sender, letter);
    }

    public void HandleStartGame(ResidentRecord sender, Letter letter)
    {
        PostOffice.GetTown(sender.Town.Id).Start(sender, letter);
    }
}
