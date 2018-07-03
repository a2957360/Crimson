using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathStatusLayer : MonoBehaviour
{
    public int _X;
    public int _Y;
    public bool _isSet = false;
    SpriteRenderer sRend;

    private void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        InvokeRepeating("SetPathStatus", 2, 2);
    }

    void SetPathStatus()
    {
        if (_isSet && sRend != null)
        {
            Cell newCell;
            if (GameManager.Instance._cells.TryGetValue(new Vector2(_X, _Y), out newCell))
            {
                if (newCell._cost >= 10)
                {
                    sRend.color = new Color(255, 0, 0, 0.2f);
                }
                else
                {
                    if (newCell._ZoD > 0)
                        sRend.color = new Color(1.0f, 0.1f, 0.5f, 0.2f);
                    else if (newCell._ZoD < 0)
                        sRend.color = new Color(Color.grey.r, Color.grey.r, Color.grey.r, 0.2f);
                    else
                        sRend.color = new Color(0, 255, 0, 0.2f);
                }
            }

        }
    }

}
