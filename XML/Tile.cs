using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

//[XmlType("Tile")]
public class Tile : Position
{

    /*
     * 0 - Impassable
     * 1 - Default
     * 2 - 
     * 3 - 
     * 4 - 
     * 5 -
     * 6 -
     * 7 -
     * */
    public int _terrain { get; set; }

    /*
     * 0
     * 1 - item type 1
     * 2 - item type 2
     * 3 - item type 3
     * */
    public int _item { get; set; }

    //public Tile() : base()
    //{
    //    _brick = 0;
    //    _item = 0;
    //}

    public Tile() : base()
    {
        _terrain = 0;
        _item = 0;
    }

    public Tile(int x, int y, int terrain, int item) : base(x, y)
    {
        _terrain = terrain;
        _item = item;
    }
}
