using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueAStarNode
{
    public int _G;
    public int _H;
    public Vector2 _location;
    public TrueAStarNode _parent;

    public TrueAStarNode(TrueAStarNode parent, Vector2 location, Vector2 target)
    {
        _parent = parent;
        _location = location;
        if (_parent != null)
            _G = parent._G + 1;
        else
            _G = 0;

        // simple Manhattan distance method
        _H = (int)Vector3.Magnitude(target - location);
    }

    public int F()
    {
        return _G + _H;
    }

    public void ChangeParent(TrueAStarNode newParent)
    {
        _parent = newParent;
        if (_parent != null)
            _G = _parent._G + 1;
        else
            _G = 0;
    }
}
