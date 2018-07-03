using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatFriend
{
    public ChrController _chr = null;
    public string _name = "";
    public Sprite _image = null;

    public CombatFriend()
    {
        _chr = null;
        _name = "";
        _image = null;
    }

    public CombatFriend(ChrController chr, string newName, Sprite newImage)
    {
        _chr = chr;
        _name = newName;
        _image = newImage;
    }
}
