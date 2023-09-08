using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LetterHandler 
{
    public abstract void Handle(ResidentRecord postboxOwner, Letter letter);

    public abstract void Close();
}
