using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

[XmlType("Position")]
public class Position
{

    public int _x { get; set; } // position in map grid (0, 0) top left corner
    public int _y { get; set; }

    public Position()
    {
        _x = 0;
        _y = 0;
    }

    public Position(int x, int y)
    {
        _x = x;
        _y = y;
    }

    //public bool Equals(Position other)
    //{
    //    return this._x == other._x &&
    //           this._y == other._y;
    //}
}
