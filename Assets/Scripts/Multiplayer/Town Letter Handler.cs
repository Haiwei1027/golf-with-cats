using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownLetterHandler : LetterHandler
{
    private Town town;

    public TownLetterHandler(Town town)
    {
        this.town = town;
    }

    public override void Close(Postbox postbox)
    {
        town.Leave(postbox.Owner);
    }

    public override void Handle(ResidentRecord postboxOwner, Letter letter)
    {
        LetterType type = letter.ReadType();
        switch (type)
        {
            case LetterType.LEAVETOWN:
                PostOffice.GetTown(postboxOwner.Town.Id).Leave(postboxOwner);
                break;
            case LetterType.HOLOGRAMCREATE:
                HandleHologramCreate(postboxOwner, letter);
                break;
            case LetterType.HOLOGRAMUPDATE:
                HandleHologramUpdate(postboxOwner, letter);
                break;
            case LetterType.HOLOGRAMDESTROY:
                HandleHologramDestroy(postboxOwner, letter);
                break;
            case LetterType.STARTGAME:
                HandleStartGame(postboxOwner, letter);
                break;
            default:
                Debug.LogError($"Unknown type {type}");
                break;
        }
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
