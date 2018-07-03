using UnityEngine;
using System.Collections;

public class Layer
{
    private int _id;
    public int id
    {
        get { return _id; }
    }

    public string name { get; set; }
    public bool isShow { get; set; }

    public Layer(int newId, string _name)
    {
        _id = newId;
        name = _name;
        isShow = true;
    }
}
