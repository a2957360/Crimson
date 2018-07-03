using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldEvent : MonoBehaviour {

    public int _Id;  

    public bool _active = false;

    // if spawn hero unit or enemy unit
    public bool _playerSide = false;

    public int _locationX;
    public int _locationY;
}
