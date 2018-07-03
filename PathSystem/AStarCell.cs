using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarCell : MonoBehaviour
{
    private SpriteRenderer _sprite;
    // derived from Cell's cost and ZoD, no need to separate G and H
    public int _F;
    public Vector2 _parent;
    public Vector2 _position;
    public Color _color1;
    public Color _color2;
    public GameObject FootPrint;

    // this cell is on the selected path to the target cell
    public bool _selectedPath;

    // unit can take action against another unit on this cell
    public bool _actionable;

    //public bool _targetable;

    public bool _isPlayer;

    public bool _isEnemy;

    public GameObject SelectedFrame;

    void Awake()
    {
        _color1 = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.3f);
        _color2 = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.8f);
        _sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _isPlayer = false;
        _isEnemy = false;
        if (_sprite != null)
            _sprite.enabled = true;
    }

    void Update()
    {
        if (_sprite != null)
            _sprite.color = Color.Lerp(_color1, _color2, Mathf.PingPong(Time.time, 1));
    }

    public void SetAStarCell(Vector2 parent, int F, Vector2 pos, bool actionable, bool targetable)
    {
        _parent = parent;
        _F = F;
        _position = pos;
        _actionable = actionable;

        if (targetable)
        {
            _color1 = new Color(Color.red.r, Color.red.g, Color.red.b, 0.3f);
            _color2 = new Color(Color.red.r, Color.red.g, Color.red.b, 0.8f);
        }
    }

    public void SetAsSelectedPath(bool value)
    {
        if (_sprite != null)
        {
            if (value)
            {
                FootPrint.SetActive(true);
                _color1 = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.5f);
                _color2 = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 1.0f);
            }
            else
            {
                FootPrint.SetActive(false);
                _color1 = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.3f);
                _color2 = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.8f);
                //if (_targetable)
                //{
                //    _color1 = new Color(Color.red.r, Color.red.g, Color.red.b, 0.1f);
                //    _color2 = new Color(Color.red.r, Color.red.g, Color.red.b, 0.5f);
                //}
                //else
                //{
                //    _color1 = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.1f);
                //    _color2 = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.5f);
                //}

            }
        }
    }

    public void ActivateFrame()
    {
        SelectedFrame.SetActive(true);
    }

    public void DeactivateFrame()
    {
        SelectedFrame.SetActive(false);
    }

    public void FootprintDirction(string direction)
    {
        Vector3 ftscale = FootPrint.gameObject.transform.localScale;
        switch (direction)
        {
            case "up":
                FootPrint.gameObject.transform.localScale = new Vector3(ftscale.x, Mathf.Abs(ftscale.y), ftscale.z);
                FootPrint.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case "down":
                FootPrint.gameObject.transform.localScale = new Vector3(ftscale.x, -Mathf.Abs(ftscale.y), ftscale.z);
                FootPrint.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case "left":
                FootPrint.gameObject.transform.localScale = new Vector3(ftscale.x, Mathf.Abs(ftscale.y), ftscale.z);
                FootPrint.gameObject.transform.localRotation = Quaternion.Euler(0,0,90);
                break;
            case "right":
                FootPrint.gameObject.transform.localScale = new Vector3(ftscale.x, Mathf.Abs(ftscale.y), ftscale.z);
                FootPrint.gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90);
                break;
        }

    }
}
