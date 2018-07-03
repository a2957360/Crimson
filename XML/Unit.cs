using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Position
{

    public int _type { get; set; }

    public Unit() : base()
    {
        _type = 0;
    }

    public Unit(int x, int y, int type) : base(x, y)
    {
        _type = type;
    }
}
