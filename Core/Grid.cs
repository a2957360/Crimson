using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Cell _cell;

    public Grid(int x, int y, int cost)
    {
        _cell._X = x;
        _cell._Y = y;
        _cell._cost = cost;
    }
}
