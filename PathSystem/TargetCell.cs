using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCell : MonoBehaviour
{
    private SpriteRenderer _sprite;

    public GameObject SelectedFrame;

    public Vector2 _position;

    public ChrController _targetChr;

    /*
     * A list of all the positions (within AStar cells) that can attack this spot
     * */
    public List<Vector2> _attackPositions = new List<Vector2>();

    public int _attackPosIndex = 0;

    public Color _color1;
    public Color _color2;

    void Awake()
    {
        _color1 = new Color(Color.red.r, Color.red.g, Color.red.b, 0.2f);
        _color2 = new Color(Color.red.r, Color.red.g, Color.red.b, 0.8f);
        _sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_sprite != null)
            _sprite.color = Color.Lerp(_color1, _color2, Mathf.PingPong(Time.time, 1));
    }

    public int ChangeAttackPosIndex()
    {
        if (_attackPositions.Count > 1)
        {
            _attackPosIndex = (_attackPosIndex + 1) % _attackPositions.Count;
        }
        else
        {
            _attackPosIndex = 0;
        }
        GetAttackRoute();
        return _attackPosIndex;
    }

    public void GetAttackRoute()
    {
        if (_attackPositions.Count > _attackPosIndex && _targetChr != null)
        {
            AStarPathfinding.Instance.GeneratePath(_attackPositions[_attackPosIndex]);
            ActivateFrame();
        }
        //    Vector2 ret = new Vector2(-1, -1);
        //if (_attackPositions.Count > _attackPosIndex)
        //    ret = _attackPositions[_attackPosIndex];
        //return ret;
    }

    public void GetRandomAttackRoute()
    {
        if (_attackPositions.Count > 0 && _targetChr != null)
        {
            AStarPathfinding.Instance.GeneratePath(_attackPositions[Random.Range(0, _attackPositions.Count)]);
            ActivateFrame();
        }
        //    Vector2 ret = new Vector2(-1, -1);
        //if (_attackPositions.Count > _attackPosIndex)
        //    ret = _attackPositions[_attackPosIndex];
        //return ret;
    }

    public void SetTargetCell(Vector2 pos, Vector2 attPos, ChrController chr)
    {
        _targetChr = chr;
        _position = pos;
        AddAttackPosition(attPos);
    }

    public void AddAttackPosition(Vector2 pos)
    {
        Vector2 temp = _attackPositions.Find(c => (c.x == pos.x) && (c.y == pos.y));
        //Debug.Log(temp);
        if (temp.x == 0 && temp.y == 0)
            _attackPositions.Add(pos);
    }

    public void ActivateFrame()
    {
        SelectedFrame.SetActive(true);
    }

    public void DeactivateFrame()
    {
        SelectedFrame.SetActive(false);
    }
}
