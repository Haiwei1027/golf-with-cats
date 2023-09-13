using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostOfficeLetterHandler : LetterHandler
{

    public override void Handle(ResidentRecord postboxOwner, Letter letter)
    {
        LetterType type = letter.ReadType();
        switch (type)
        {
            case LetterType.INTRODUCE:
                string username = letter.ReadString();
                postboxOwner.Username = username;
                Debug.Log($"{username} Connected");
                break;
            case LetterType.CREATETOWN:
                PostOffice.MakeTown(postboxOwner);
                break;
            case LetterType.JOINTOWN:
                Town town = PostOffice.GetTown(letter.ReadInt());
                town.Join(postboxOwner);
                break;
            default:
                Debug.LogError($"Unknown type {type}");
                break;
        }
        letter.Release();
    }

    public override void Close(Postbox postbox)
    {
        PostOffice.KickResident(postbox.Owner);
    }
}
