using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStarPathfinding : Singleton<AStarPathfinding>
{
    #region Variables

    #region Editor
    public GameObject _AStarTile;
    public GameObject _TargetTile;
    public GameObject _PathDirectionTile;

    public GameObject _TargetIcon_EnemyMelee;
    public GameObject _TargetIcon_EnemyPistol;
    public GameObject _TargetIcon_EnemyRange;
    public GameObject _TargetIcon_EnemyLongRange;
    public GameObject _TargetIcon_EnemySpell;
    public GameObject _TargetIcon_EnemyBow;

    public GameObject _TargetIcon_HeroMelee;
    public GameObject _TargetIcon_HeroRange;
    public GameObject _TargetIcon_HeroSpell;
    public GameObject _TargetIcon_HeroBow;
    public GameObject _TargetIcon_HeroPistol;
    public GameObject _TargetIcon_HeroLongRange;
    #endregion

    #region Collections
    public List<AStarCell> _AStarCells = new List<AStarCell>();
    //public List<AStarCell> _AStarActionCells = new List<AStarCell>();
    public List<TargetCell> _targetCells = new List<TargetCell>();
    public List<AStarCell> _path = new List<AStarCell>();
    public List<GameObject> _targetIcons = new List<GameObject>();
    public List<GameObject> _pathDirectionTiles = new List<GameObject>();
    // for attackrange
    //public List<AStarCell> _templePath = new List<AStarCell>();
    private Transform _boardHolder;
    public bool _showAStarPath = false;
    #endregion

    #endregion

    #region Gameflow

    void Start()
    {
        _boardHolder = new GameObject("PathCells").transform;
    }

    #endregion

    #region Functions

    // start value is the movementRange of the unit
    public void GenerateAStarCells(ChrController.AttackTypes attackType, Vector2 startPos, int startValue, bool heroSide)
    {
        if (_AStarTile != null)
        {
            switch (attackType)
            {
                case ChrController.AttackTypes.Melee:
                    {
                        CheckForActionableMelee(startPos, heroSide == true ? 1 : 2, true);
                    }
                    break;
                case ChrController.AttackTypes.Spell:
                    {
                        CheckForActionableSpell(startPos, heroSide == true ? 1 : 2, true);
                    }
                    break;
                case ChrController.AttackTypes.Pistol:
                    {
                        CheckForActionablePistol(startPos, heroSide == true ? 1 : 2, true);
                    }
                    break;
                case ChrController.AttackTypes.Range:
                    {
                        CheckForActionableRange(startPos, heroSide == true ? 1 : 2, true);
                    }
                    break;
                case ChrController.AttackTypes.LongRange:
                    {
                        CheckForActionableLongRange(startPos, heroSide == true ? 1 : 2, true);
                    }
                    break;
                case ChrController.AttackTypes.Bow:
                    {
                        CheckForActionableBow(startPos, heroSide == true ? 1 : 2, true);
                    }
                    break;
            }
            //second parameter is attack range
            //CheckForActionable(startPos, GameManager.Instance._curUnit._AttackRange, 0, heroSide == true ? 1 : 2);
            //CheckForActionable(startPos, GameManager.Instance._curUnit._AttackRange, 1, heroSide == true ? 1 : 2);
            //CheckForActionable(startPos, GameManager.Instance._curUnit._AttackRange, 2, heroSide == true ? 1 : 2);
            //CheckForActionable(startPos, GameManager.Instance._curUnit._AttackRange, 3, heroSide == true ? 1 : 2);
            GenerateSingleAStarCell(attackType, new Vector2(startPos.x + 1, startPos.y), startValue, true, startPos, heroSide, 3);
            GenerateSingleAStarCell(attackType, new Vector2(startPos.x - 1, startPos.y), startValue, true, startPos, heroSide, 2);
            GenerateSingleAStarCell(attackType, new Vector2(startPos.x, startPos.y + 1), startValue, true, startPos, heroSide, 0);
            GenerateSingleAStarCell(attackType, new Vector2(startPos.x, startPos.y - 1), startValue, true, startPos, heroSide, 1);

            //Invoke("GenerateTargetIconCurrent", 0.05f);
            //GenerateTargetIcon(startPos);
        }
    }

    public void ClearPathCells()
    {
        if (_AStarCells.Count > 0)
        {
            foreach (AStarCell item in _AStarCells)
                Destroy(item.gameObject);

        }

        if (_targetCells.Count > 0)
        {
            //Attackable enemy
            foreach (TargetCell item in _targetCells)
                Destroy(item.gameObject);
        }
        _AStarCells.Clear();
        _path.Clear();
        //Attackable enemy
        _targetCells.Clear();
        //_templePath.Clear();
        ClearTargetIcons();
        ClearAStarDirectionPath();
    }

    public void ClearTargetIcons()
    {
        if (_targetIcons.Count > 0)
        {
            //Attackable enemy
            foreach (GameObject item in _targetIcons)
                Destroy(item);

            _targetIcons.Clear();
        }
    }

    public void ClearAStarDirectionPath()
    {
        if (_pathDirectionTiles.Count > 0)
        {
            foreach (GameObject item in _pathDirectionTiles)
                Destroy(item);

            _pathDirectionTiles.Clear();
        }
    }

    public void GeneratePath(Vector2 target)
    {
        AStarCell starCell = null;
        Vector2 curTarget = target;
        //ClearFrame();
        ////setframe
        //if (_AStarActionCells.Find(c => (c._position.x == target.x) && (c._position.y == target.y)) != null)
        //{
        //    _AStarActionCells.Find(c => (c._position.x == target.x) && (c._position.y == target.y)).SelectedFrame.SetActive(true);
        //}
        //if (_AStarCells.Find(c => (c._position.x == target.x) && (c._position.y == target.y)) != null)
        //{
        //    _AStarCells.Find(c => (c._position.x == target.x) && (c._position.y == target.y)).SelectedFrame.SetActive(true);
        //}

        //if (_AStarActionCells.Find(c => (c._position.x == target.x) && (c._position.y == target.y)) == null)
        //{
        //    //attackrange
        //    if (_path.Count != 0 && _path[_path.Count - 1]._actionable)
        //    {
        //        _templePath.Clear();
        //        _templePath.AddRange(_path);
        //    }
        //    ClearPath();
        //    GameManager.Instance._curEnemy = null;
        //}
        //else
        //{
        //    ChrController chr = GameManager.Instance.CheckUnit((int)target.x, (int)target.y, isHero == true ? 2 : 1);
        //    GameManager.Instance._curEnemy = chr;
        //    //attackrange
        //    if (_path.Count == 0 && _templePath.Count != 0)
        //    {
        //        Debug.Log("Pathtemple");
        //        _path.AddRange(_templePath);
        //        for (int i = 0; i < _path.Count; i++)
        //        {
        //            _path[i].SetAsSelectedPath(true);
        //        }
        //        //_path.Reverse();
        //    }
        //}

        ClearPath();
        starCell = _AStarCells.Find(c => (c._position.x == curTarget.x) && (c._position.y == curTarget.y));
        if (starCell != null && (starCell._isPlayer || starCell._isEnemy))
        {
            return;
        }

        do // this won't be called if cursor is on a actionable/targetable grid
        {
            starCell = _AStarCells.Find(c => (c._position.x == curTarget.x) && (c._position.y == curTarget.y));
            if (starCell != null)
            {
                starCell.SetAsSelectedPath(true);
                _path.Add(starCell);
                curTarget = starCell._parent;
            }
        } while (starCell != null);

        if (_path.Count > 0 && _AStarCells.Find(c => (c._position.x == target.x) && (c._position.y == target.y)) != null)
            _path.Reverse();

        if (_path.Count > 0)
        {
            float herodirection = _path[_path.Count - 1]._position.x - GameManager.Instance._curUnit._position.x;
            if (herodirection > 0)
            {
                //face right
                GameManager.Instance._curUnit.Flip(true);
            }
            else if (herodirection == 0)
            {
                //do not change direction
            }
            else
            {
                //face left
                GameManager.Instance._curUnit.Flip(false);
            }

            for (int i = 0; i < _path.Count; i++)
            {
                Vector2 ftdirection = new Vector2();
                if (i == 0)
                {
                    ftdirection = _path[i]._position - GameManager.Instance._curUnit._position;
                }
                else
                {
                    ftdirection = _path[i]._position - _path[i - 1]._position;
                }

                //right
                if (ftdirection.x > 0)
                {
                    _path[i].FootprintDirction("right");
                }
                //left
                else if (ftdirection.x < 0)
                {
                    _path[i].FootprintDirction("left");
                }
                //up
                else if (ftdirection.y > 0)
                {
                    _path[i].FootprintDirction("up");
                }
                //down
                else if (ftdirection.y < 0)
                {
                    _path[i].FootprintDirction("down");
                }
            }
        }

        //if (GameManager.Instance._curEnemy != null && _path.Count != 0)
        //{
        //    if (!isPositionNearby(target, _path[_path.Count - 1]._position))
        //    {
        //        ClearPath();
        //    }
        //}
    }

    //bool isPositionNearby(Vector2 center, Vector2 checkposition)
    //{
    //    if (GameManager.Instance._curUnit != null && GameManager.Instance._curEnemy != null)
    //    {
    //        float distance = Mathf.Abs((center - checkposition).x) + Mathf.Abs((center - checkposition).y);
    //        if (distance == GameManager.Instance._curUnit._AttackRange)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    public void ClearPath()
    {
        foreach (AStarCell cell in _path)
        {
            cell.SetAsSelectedPath(false);
        }

        _path.Clear();
    }

    //void ClearFrame()
    //{
    //    foreach (AStarCell cell in _path)
    //    {
    //        cell.SelectedFrame.SetActive(false);
    //    }
    //    foreach (AStarCell cell in _AStarActionCells)
    //    {
    //        cell.SelectedFrame.SetActive(false);
    //    }
    //}

    public List<Vector2> GivePath()
    {
        List<Vector2> result = new List<Vector2>();

        if (_path.Count > 0)
        {
            foreach (AStarCell cell in _path)
                result.Add(cell._position);
        }

        return result;
    }

    #endregion

    #region Helpers

    /* if boolean start is true, then cell is added regardless of value comparison
        this way the cells adjacent to a unit will always be added, unless they are obstacles
        Dir: 0: Up, 1: Down, 2: Left, 3: Right
        */
    private void GenerateSingleAStarCell(ChrController.AttackTypes attackType, Vector2 pos, int value, bool start, Vector2 parent, bool heroSide, int dir)
    {
        if (CheckBoundary((int)pos.x, (int)pos.y))
            return;

        bool isNewCell = true;
        bool isEnd = true;
        int F = value;
        Cell newCell = GameManager.Instance.GetCell((int)pos.x, (int)pos.y);

        if (newCell != null && (newCell._cost < 10 || (newCell.isPlayer && GameManager.Instance._playerPhase) || newCell.isEnemy) && (newCell._X != GameManager.Instance._curUnit._position.x || newCell._Y != GameManager.Instance._curUnit._position.y))
        {
            // calculate the F cost
            if (newCell.isPlayer && GameManager.Instance._playerPhase)
            {
                F -= (newCell._cost - 10);
            }
            else if (newCell.isEnemy)
            {
                F -= (newCell._cost - 10);
            }
            else
            {
                F -= newCell._cost;
            }
            if (GameManager.Instance._heros.Find(s => s == GameManager.Instance._curUnit) != null && newCell.isEnemy)
            {
                return;
            }
            if (GameManager.Instance._enemies.Find(s => s == GameManager.Instance._curUnit) != null && newCell.isPlayer)
            {
                return;
            }

            //if (heroSide && newCell._ZoD < 0)
            //    F += newCell._ZoD;
            //else if (!heroSide && newCell._ZoD > 0)
            //    F -= newCell._ZoD;

            if (start || F >= 0)
            {
                // changes
                if (heroSide && newCell._ZoD < 0)
                    F += newCell._ZoD;
                else if (!heroSide && newCell._ZoD > 0)
                    F -= newCell._ZoD;
                // check if this path is better than a previously established one
                AStarCell starCell = _AStarCells.Find(c => (c._position.x == pos.x) && (c._position.y == pos.y));
                if (starCell != null)
                {
                    isNewCell = false;
                    if (F > starCell._F)
                    {
                        starCell._F = F;
                        starCell._parent = parent;
                        isEnd = false;
                    }
                }
                if (isNewCell)
                {

                    GameObject instance = Instantiate(_AStarTile, pos, Quaternion.identity);
                    instance.transform.SetParent(_boardHolder);
                    if (newCell.isPlayer && GameManager.Instance._playerPhase)
                    {
                        instance.SetActive(false);
                    }
                    if (newCell.isEnemy)
                    {
                        instance.SetActive(false);
                    }
                    AStarCell curCell = instance.GetComponent<AStarCell>();
                    if (curCell != null)
                    {
                        bool _actionable = false;
                        switch (attackType)
                        {
                            case ChrController.AttackTypes.Melee:
                                {
                                    _actionable = CheckForActionableMelee(pos, heroSide == true ? 1 : 2, false);
                                }
                                break;
                            case ChrController.AttackTypes.Spell:
                                {
                                    _actionable = CheckForActionableSpell(pos, heroSide == true ? 1 : 2, false);
                                }
                                break;
                            case ChrController.AttackTypes.Pistol:
                                {
                                    _actionable = CheckForActionablePistol(pos, heroSide == true ? 1 : 2, false);
                                }
                                break;
                            case ChrController.AttackTypes.Range:
                                {
                                    _actionable = CheckForActionableRange(pos, heroSide == true ? 1 : 2, false);
                                }
                                break;
                            case ChrController.AttackTypes.LongRange:
                                {
                                    _actionable = CheckForActionableLongRange(pos, heroSide == true ? 1 : 2, false);
                                }
                                break;
                            case ChrController.AttackTypes.Bow:
                                {
                                    _actionable = CheckForActionableBow(pos, heroSide == true ? 1 : 2, false);
                                }
                                break;
                        }
                        //original
                        //bool _actionable = CheckForActionable(pos, GameManager.Instance._curUnit._AttackRange, dir, heroSide == true ? 1 : 2);
                        //get enemy position
                        //CheckForActionable(pos, dir, heroSide == true ? 1 : 2);
                        //bool _actionable = false;
                        if (newCell.isPlayer && GameManager.Instance._playerPhase)
                        {
                            curCell._isPlayer = true;
                        }
                        if (newCell.isEnemy)
                        {
                            curCell._isEnemy = true;
                        }
                        curCell.SetAStarCell(parent, F, pos, _actionable, false);
                        _AStarCells.Add(curCell);
                        isEnd = false;
                    }
                }

                if (!isEnd)
                {
                    switch (dir)
                    {
                        case 0: // Up
                            {
                                GenerateSingleAStarCell(attackType, new Vector2(pos.x + 1, pos.y), F, false, pos, heroSide, 3);
                                GenerateSingleAStarCell(attackType, new Vector2(pos.x - 1, pos.y), F, false, pos, heroSide, 2);
                                GenerateSingleAStarCell(attackType, new Vector2(pos.x, pos.y + 1), F, false, pos, heroSide, 0);
                            }
                            break;
                        case 1: // Down
                            {
                                GenerateSingleAStarCell(attackType, new Vector2(pos.x + 1, pos.y), F, false, pos, heroSide, 3);
                                GenerateSingleAStarCell(attackType, new Vector2(pos.x - 1, pos.y), F, false, pos, heroSide, 2);
                                GenerateSingleAStarCell(attackType, new Vector2(pos.x, pos.y - 1), F, false, pos, heroSide, 1);
                            }
                            break;
                        case 2: // Left
                            {
                                GenerateSingleAStarCell(attackType, new Vector2(pos.x - 1, pos.y), F, false, pos, heroSide, 2);
                                GenerateSingleAStarCell(attackType, new Vector2(pos.x, pos.y + 1), F, false, pos, heroSide, 0);
                                GenerateSingleAStarCell(attackType, new Vector2(pos.x, pos.y - 1), F, false, pos, heroSide, 1);
                            }
                            break;
                        case 3: // Right
                            {
                                GenerateSingleAStarCell(attackType, new Vector2(pos.x + 1, pos.y), F, false, pos, heroSide, 3);
                                GenerateSingleAStarCell(attackType, new Vector2(pos.x, pos.y + 1), F, false, pos, heroSide, 0);
                                GenerateSingleAStarCell(attackType, new Vector2(pos.x, pos.y - 1), F, false, pos, heroSide, 1);
                            }
                            break;
                    } // end of switch
                }
            } // if (start || F >= 0)
        }
    }

    private bool CheckForActionableMelee(Vector2 pos, int type, bool start)
    {
        bool ret = false;
        ret = CheckForSingleActionable(new Vector2(pos.x + 1, pos.y), pos, type, start);
        ret = CheckForSingleActionable(new Vector2(pos.x - 1, pos.y), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + 1), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - 1), pos, type, start) || ret;
        return ret;
    }

    private bool CheckForActionableSpell(Vector2 pos, int type, bool start)
    {
        bool ret = false;
        ret = CheckForSingleActionable(new Vector2(pos.x + 1, pos.y), pos, type, start);
        ret = CheckForSingleActionable(new Vector2(pos.x - 1, pos.y), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + 1), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - 1), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x + 1, pos.y + 1), pos, type, start);
        ret = CheckForSingleActionable(new Vector2(pos.x - 1, pos.y - 1), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x + 1, pos.y - 1), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x - 1, pos.y + 1), pos, type, start) || ret;
        return ret;
    }

    private bool CheckForActionableRange(Vector2 pos, int type, bool start)
    {
        bool ret = false;
        ret = CheckForSingleActionable(new Vector2(pos.x + 3, pos.y), pos, type, start);
        ret = CheckForSingleActionable(new Vector2(pos.x - 3, pos.y), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + 3), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - 3), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x + 2, pos.y), pos, type, start);
        ret = CheckForSingleActionable(new Vector2(pos.x - 2, pos.y), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + 2), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - 2), pos, type, start) || ret;
        return ret;
    }

    private bool CheckForActionablePistol(Vector2 pos, int type, bool start)
    {
        bool ret = false;
        ret = CheckForSingleActionable(new Vector2(pos.x + 1, pos.y), pos, type, start);
        ret = CheckForSingleActionable(new Vector2(pos.x - 1, pos.y), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + 1), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - 1), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x + 2, pos.y), pos, type, start);
        ret = CheckForSingleActionable(new Vector2(pos.x - 2, pos.y), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + 2), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - 2), pos, type, start) || ret;
        return ret;
    }

    private bool CheckForActionableLongRange(Vector2 pos, int type, bool start)
    {
        bool ret = false;
        ret = CheckForSingleActionable(new Vector2(pos.x + 3, pos.y), pos, type, start);
        ret = CheckForSingleActionable(new Vector2(pos.x - 3, pos.y), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + 3), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - 3), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x + 4, pos.y), pos, type, start);
        ret = CheckForSingleActionable(new Vector2(pos.x - 4, pos.y), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + 4), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - 4), pos, type, start) || ret;
        return ret;
    }

    private bool CheckForActionableBow(Vector2 pos, int type, bool start)
    {
        bool ret = false;
        ret = CheckForSingleActionable(new Vector2(pos.x + 1, pos.y + 1), pos, type, start);
        ret = CheckForSingleActionable(new Vector2(pos.x - 1, pos.y - 1), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x - 1, pos.y + 1), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x + 1, pos.y - 1), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x + 2, pos.y), pos, type, start);
        ret = CheckForSingleActionable(new Vector2(pos.x - 2, pos.y), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + 2), pos, type, start) || ret;
        ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - 2), pos, type, start) || ret;
        return ret;
    }

    // type: 1: hero 2: enemy
    // Dir: 0: Up, 1: Down, 2: Left, 3: Right
    //private bool CheckForActionable(Vector2 pos, int range, int dir, int type)
    //{
    //    bool ret = false;
    //    switch (dir)
    //    {
    //        case 0: // Up
    //            {
    //                ret = CheckForSingleActionable(new Vector2(pos.x + range, pos.y), pos, type);
    //                ret = CheckForSingleActionable(new Vector2(pos.x - range, pos.y), pos, type) || ret;
    //                ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + range), pos, type) || ret;
    //                if (range > 1)
    //                {
    //                    for (int i = 1; i < range; i++)
    //                    {
    //                        ret = CheckForSingleActionable(new Vector2(pos.x - i, pos.y + (range - i)), pos, type) || ret;
    //                        ret = CheckForSingleActionable(new Vector2(pos.x + i, pos.y + (range - i)), pos, type) || ret;
    //                    }
    //                }
    //                //if (!ret)
    //                //    ret = CheckForSingleActionable(new Vector2(pos.x - range, pos.y), type);
    //                //if (!ret)
    //                //    ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + range), type);
    //            }
    //            break;
    //        case 1: // Down
    //            {
    //                ret = CheckForSingleActionable(new Vector2(pos.x + range, pos.y), pos, type);
    //                ret = CheckForSingleActionable(new Vector2(pos.x - range, pos.y), pos, type) || ret;
    //                ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - range), pos, type) || ret;
    //                if (range > 1)
    //                {
    //                    for (int i = 1; i < range; i++)
    //                    {
    //                        ret = CheckForSingleActionable(new Vector2(pos.x - i, pos.y - (range - i)), pos, type) || ret;
    //                        ret = CheckForSingleActionable(new Vector2(pos.x + i, pos.y - (range - i)), pos, type) || ret;
    //                    }
    //                }
    //                //if (!ret)
    //                //    ret = CheckForSingleActionable(new Vector2(pos.x - range, pos.y), type);
    //                //if (!ret)
    //                //    ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - range), type);
    //            }
    //            break;
    //        case 2: // Left
    //            {
    //                ret = CheckForSingleActionable(new Vector2(pos.x - range, pos.y), pos, type);
    //                ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + range), pos, type) || ret;
    //                ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - range), pos, type) || ret;
    //                if (range > 1)
    //                {
    //                    for (int i = 1; i < range; i++)
    //                    {
    //                        ret = CheckForSingleActionable(new Vector2(pos.x - (range - 1), pos.y + i), pos, type) || ret;
    //                        ret = CheckForSingleActionable(new Vector2(pos.x - (range - 1), pos.y - i), pos, type) || ret;
    //                    }
    //                }
    //                //if (!ret)
    //                //    ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + range), type);
    //                //if (!ret)
    //                //    ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - range), type);
    //            }
    //            break;
    //        case 3: // Right
    //            {
    //                ret = CheckForSingleActionable(new Vector2(pos.x + range, pos.y), pos, type);
    //                ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + range), pos, type) || ret;
    //                ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - range), pos, type) || ret;
    //                if (range > 1)
    //                {
    //                    for (int i = 1; i < range; i++)
    //                    {
    //                        ret = CheckForSingleActionable(new Vector2(pos.x + (range - 1), pos.y + i), pos, type) || ret;
    //                        ret = CheckForSingleActionable(new Vector2(pos.x + (range - 1), pos.y - i), pos, type) || ret;
    //                    }
    //                }
    //                //if (!ret)
    //                //    ret = CheckForSingleActionable(new Vector2(pos.x, pos.y + range), type);
    //                //if (!ret)
    //                //    ret = CheckForSingleActionable(new Vector2(pos.x, pos.y - range), type);
    //            }
    //            break;
    //    } // end of switch
    //    return ret;
    //}

    // type: 1: hero 2: enemy
    private bool CheckForSingleActionable(Vector2 pos, Vector2 parent, int type, bool startPos)
    {
        bool ret = false;
        ChrController chr = GameManager.Instance.CheckUnit((int)pos.x, (int)pos.y, type == 1 ? 2 : 1);

        if (chr != null)
        {
            ret = true;

            //add attackable enemy
            TargetCell target = _targetCells.Find(c => (c._position.x == pos.x) && (c._position.y == pos.y));
            if (startPos || (GameManager.Instance.CheckUnit((int)parent.x, (int)parent.y, 1) == null && GameManager.Instance.CheckUnit((int)parent.x, (int)parent.y, 2) == null))
            {
                if (target == null)
                {
                    GameObject instance = Instantiate(_TargetTile, pos, Quaternion.identity);
                    instance.transform.SetParent(_boardHolder);
                    //bool _actionable = true;
                    TargetCell curCell = instance.GetComponent<TargetCell>();
                    curCell.SetTargetCell(pos, parent, chr);
                    _targetCells.Add(curCell);
                }
                else
                {
                    target.AddAttackPosition(parent);
                }
            }


        }

        return ret;
    }

    private bool CheckBoundary(int x, int y)
    {
        bool ret = false;

        if (x < 0 || x >= GameManager.Instance._mapWidth)
            ret = true;

        if (y < 0 || y >= GameManager.Instance._mapHeight)
            ret = true;
        return ret;
    }

    public TargetCell FindRandomTarget()
    {
        TargetCell target = null;

        if (_targetCells.Count > 0)
            target = _targetCells[UnityEngine.Random.Range(0, _targetCells.Count)];

        return target;
    }

    public void GenerateTargetIconCurrent()
    {
        if (GameManager.Instance._curUnit == null)
            return;
        GenerateTargetIcon(GameManager.Instance._curUnit._position);
    }

    public void GenerateTargetIcon(Vector3 pos)
    {
        ClearTargetIcons();
        if (GameManager.Instance._curUnit == null)
            return;

        switch (GameManager.Instance._curUnit._attackType)
        {
            case ChrController.AttackTypes.Melee:
                {
                    if (GameManager.Instance._curUnit.gameObject.tag == "Player")
                    {
                        GenerateTargetIconMelee(_TargetIcon_HeroMelee, pos);
                    }
                    else
                    {
                        GenerateTargetIconMelee(_TargetIcon_EnemyMelee, pos);
                    }
                }
                break;
            case ChrController.AttackTypes.Spell:
                {
                    if (GameManager.Instance._curUnit.gameObject.tag == "Player")
                    {
                        GenerateTargetIconSpell(_TargetIcon_HeroSpell, pos);
                    }
                    else
                    {
                        GenerateTargetIconSpell(_TargetIcon_EnemySpell, pos);
                    }
                }
                break;
            case ChrController.AttackTypes.Pistol:
                {
                    if (GameManager.Instance._curUnit.gameObject.tag == "Player")
                    {
                        GenerateTargetIconPistol(_TargetIcon_HeroPistol, pos);
                    }
                    else
                    {
                        GenerateTargetIconPistol(_TargetIcon_EnemyPistol, pos);
                    }
                }
                break;
            case ChrController.AttackTypes.Range:
                {
                    if (GameManager.Instance._curUnit.gameObject.tag == "Player")
                    {
                        GenerateTargetIconRange(_TargetIcon_HeroRange, pos);
                    }
                    else
                    {
                        GenerateTargetIconRange(_TargetIcon_EnemyRange, pos);
                    }
                }
                break;
            case ChrController.AttackTypes.LongRange:
                {
                    if (GameManager.Instance._curUnit.gameObject.tag == "Player")
                    {
                        GenerateTargetIconLongRange(_TargetIcon_HeroLongRange, pos);
                    }
                    else
                    {
                        GenerateTargetIconLongRange(_TargetIcon_EnemyLongRange, pos);
                    }
                }
                break;
            case ChrController.AttackTypes.Bow:
                {
                    if (GameManager.Instance._curUnit.gameObject.tag == "Player")
                    {
                        GenerateTargetIconBow(_TargetIcon_HeroBow, pos);
                    }
                    else
                    {
                        GenerateTargetIconBow(_TargetIcon_EnemyBow, pos);
                    }
                }
                break;
        }
    }

    private void GenerateSingleTargetIcon(GameObject targetIcon, Vector2 pos)
    {
        if (CheckBoundary((int)pos.x, (int)pos.y))
            return;

        //if (FindATargetableCell(pos))
        //{
        GameObject instance = Instantiate(targetIcon, pos, Quaternion.identity);

        if (!GameManager.Instance.GetCell_IsWalkable((int)pos.x, (int)pos.y))
        {
            SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
            sr.color = new Color(0, 0, 0, 0.5f);
        }

        instance.transform.SetParent(_boardHolder);
        _targetIcons.Add(instance);
        //}
    }

    private void GenerateTargetIconMelee(GameObject targetIcon, Vector2 pos)
    {
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x + 1, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x - 1, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y + 1));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y - 1));
    }

    private void GenerateTargetIconSpell(GameObject targetIcon, Vector2 pos)
    {
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x + 1, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x - 1, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y + 1));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y - 1));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x + 1, pos.y + 1));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x + 1, pos.y - 1));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x - 1, pos.y + 1));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x - 1, pos.y - 1));
    }

    private void GenerateTargetIconRange(GameObject targetIcon, Vector2 pos)
    {
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x + 2, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x - 2, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y + 2));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y - 2));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x + 3, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x - 3, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y + 3));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y - 3));
    }

    private void GenerateTargetIconPistol(GameObject targetIcon, Vector2 pos)
    {
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x + 1, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x - 1, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y + 1));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y - 1));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x + 2, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x - 2, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y + 2));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y - 2));
    }

    private void GenerateTargetIconLongRange(GameObject targetIcon, Vector2 pos)
    {
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x + 4, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x - 4, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y + 4));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y - 4));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x + 3, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x - 3, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y + 3));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y - 3));
    }

    private void GenerateTargetIconBow(GameObject targetIcon, Vector2 pos)
    {
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x + 1, pos.y + 1));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x - 1, pos.y - 1));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x - 1, pos.y + 1));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x + 1, pos.y - 1));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x + 2, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x - 2, pos.y));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y + 2));
        GenerateSingleTargetIcon(targetIcon, new Vector2(pos.x, pos.y - 2));
    }

    public AStarCell FindAStarCell(Vector2 pos, bool isPlayer, bool isEnemy)
    {
        AStarCell ret = _AStarCells.Find(c => (c._position == pos) && c._isPlayer == isPlayer && c._isEnemy == isEnemy);
        return ret;
    }

    public TargetCell FindTargetCell(Vector2 target)
    {
        TargetCell ret = _targetCells.Find(c => (c._position == target));
        return ret;
    }

    public bool FindATargetableCell(Vector2 pos)
    {
        bool ret = false;

        AStarCell temp = FindAStarCell(pos, false, false);
        if (temp != null)
        {
            ret = true;
        }
        else
        {
            TargetCell target = FindTargetCell(pos);
            if (target != null)
                ret = true;
        }
        return ret;
    }

    public List<ChrController> CheckActionableByPosition(Vector2 curposition, ChrController.AttackTypes attackType)
    {
        List<ChrController> AttackableTargets = new List<ChrController>();
        switch (attackType)
        {
            case ChrController.AttackTypes.Melee:
                {
                    CheckForSingleActionableByPositon(new Vector2(curposition.x + 1, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x - 1, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y + 1), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y - 1), AttackableTargets);
                }
                break;
            case ChrController.AttackTypes.Spell:
                {
                    CheckForSingleActionableByPositon(new Vector2(curposition.x + 1, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x - 1, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y + 1), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y - 1), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x + 1, curposition.y + 1), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x - 1, curposition.y - 1), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x + 1, curposition.y - 1), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x - 1, curposition.y + 1), AttackableTargets);
                }
                break;
            case ChrController.AttackTypes.Pistol:
                {
                    CheckForSingleActionableByPositon(new Vector2(curposition.x + 1, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x - 1, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y + 1), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y - 1), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x + 2, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x - 2, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y + 2), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y - 2), AttackableTargets);
                }
                break;
            case ChrController.AttackTypes.Range:
                {
                    CheckForSingleActionableByPositon(new Vector2(curposition.x + 3, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x - 3, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y + 3), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y - 3), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x + 2, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x - 2, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y + 2), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y - 2), AttackableTargets);
                }
                break;
            case ChrController.AttackTypes.LongRange:
                {
                    CheckForSingleActionableByPositon(new Vector2(curposition.x + 3, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x - 3, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y + 3), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y - 3), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x + 4, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x - 4, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y + 4), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y - 4), AttackableTargets);
                }
                break;
            case ChrController.AttackTypes.Bow:
                {
                    CheckForSingleActionableByPositon(new Vector2(curposition.x + 1, curposition.y + 1), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x - 1, curposition.y - 1), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x - 1, curposition.y + 1), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x + 1, curposition.y - 1), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x + 2, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x - 2, curposition.y), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y + 2), AttackableTargets);
                    CheckForSingleActionableByPositon(new Vector2(curposition.x, curposition.y - 2), AttackableTargets);
                }
                break;
        }

        return AttackableTargets;
    }

    private void CheckForSingleActionableByPositon(Vector2 pos, List<ChrController> tragetlist)
    {
        ChrController chr = GameManager.Instance.CheckUnit((int)pos.x, (int)pos.y, 2);

        if (chr != null)
        {
            tragetlist.Add(chr);
        }
    }

    public bool FindAttackableHeros(ChrController.AttackTypes attackType, Vector2 startPos)
    {
        Vector2 enemyposition = new Vector2();
        if (GameManager.Instance._playerPhase)
        {
            enemyposition = GameManager.Instance._curEnemy._position;
        }
        else
        {
            enemyposition = GameManager.Instance._curUnit._position;
        }
        switch (attackType)
        {
            case ChrController.AttackTypes.Melee:
                {
                    if (enemyposition.x == startPos.x)
                    {
                        if (Mathf.Abs(enemyposition.y - startPos.y) == 1)
                        {
                            return true;
                        }
                    }
                    if (enemyposition.y == startPos.y)
                    {
                        if (Mathf.Abs(enemyposition.x - startPos.x) == 1)
                        {
                            return true;
                        }
                    }
                }
                break;
            case ChrController.AttackTypes.Spell:
                {
                    if (Mathf.Abs(enemyposition.y - startPos.y) + Mathf.Abs(enemyposition.x - startPos.x) == 1 || Mathf.Abs(enemyposition.y - startPos.y) + Mathf.Abs(enemyposition.x - startPos.x) == 2)
                    {
                        return true;
                    }
                }
                break;
            case ChrController.AttackTypes.Pistol:
                {
                    if (enemyposition.x == startPos.x)
                    {
                        if (Mathf.Abs(enemyposition.y - startPos.y) == 1 || Mathf.Abs(enemyposition.y - startPos.y) == 2)
                        {
                            return true;
                        }
                    }
                    if (enemyposition.y == startPos.y)
                    {
                        if (Mathf.Abs(enemyposition.x - startPos.x) == 1 || Mathf.Abs(enemyposition.x - startPos.x) == 2)
                        {
                            return true;
                        }
                    }
                }
                break;
            case ChrController.AttackTypes.Range:
                {
                    if (enemyposition.x == startPos.x)
                    {
                        if (Mathf.Abs(enemyposition.y - startPos.y) == 2 || Mathf.Abs(enemyposition.y - startPos.y) == 3)
                        {
                            return true;
                        }
                    }
                    if (enemyposition.y == startPos.y)
                    {
                        if (Mathf.Abs(enemyposition.x - startPos.x) == 2 || Mathf.Abs(enemyposition.x - startPos.x) == 3)
                        {
                            return true;
                        }
                    }
                }
                break;
            case ChrController.AttackTypes.LongRange:
                {
                    if (enemyposition.x == startPos.x)
                    {
                        if (Mathf.Abs(enemyposition.y - startPos.y) == 3 || Mathf.Abs(enemyposition.y - startPos.y) == 4)
                        {
                            return true;
                        }
                    }
                    if (enemyposition.y == startPos.y)
                    {
                        if (Mathf.Abs(enemyposition.x - startPos.x) == 3 || Mathf.Abs(enemyposition.x - startPos.x) == 4)
                        {
                            return true;
                        }
                    }
                }
                break;
            case ChrController.AttackTypes.Bow:
                {
                    if (Mathf.Abs(enemyposition.y - startPos.y) + Mathf.Abs(enemyposition.x - startPos.x) == 2)
                    {
                        return true;
                    }
                }
                break;
        }

        return false;
    }
    #endregion

    #region True AStar

    // sort the nodes in openlist by their F score
    private void SortList(List<TrueAStarNode> openList)
    {
        openList.Sort((node1, node2) => node1.F().CompareTo(node2.F()));
    }

    private void ProcessAdjacents(TrueAStarNode current, List<TrueAStarNode> openList, List<TrueAStarNode> closeList, Vector2 target)
    {
        List<Vector2> adjacents = new List<Vector2>();
        if (GameManager.Instance.GetCell_IsWalkable((int)current._location.x - 1, (int)current._location.y))
            adjacents.Add(new Vector2(current._location.x - 1, current._location.y));

        if (GameManager.Instance.GetCell_IsWalkable((int)current._location.x + 1, (int)current._location.y))
            adjacents.Add(new Vector2(current._location.x + 1, current._location.y));

        if (GameManager.Instance.GetCell_IsWalkable((int)current._location.x, (int)current._location.y - 1))
            adjacents.Add(new Vector2(current._location.x, current._location.y - 1));

        if (GameManager.Instance.GetCell_IsWalkable((int)current._location.x, (int)current._location.y + 1))
            adjacents.Add(new Vector2(current._location.x, current._location.y + 1));

        foreach (Vector2 item in adjacents)
        {
            int closedListContainIndex = closeList.FindIndex(closedItem => closedItem._location.x == item.x && closedItem._location.y == item.y);
            if (closedListContainIndex == -1)
            {
                int openListContainIndex = openList.FindIndex(openedItem => openedItem._location.x == item.x && openedItem._location.y == item.y);
                if (openListContainIndex == -1)
                {
                    openList.Add(new TrueAStarNode(current, item, target));
                }
                else
                {
                    // optimization for open node
                    TrueAStarNode tempStar = new TrueAStarNode(current, item, target);
                    if (openList[openListContainIndex].F() > tempStar.F())
                    {
                        openList[openListContainIndex].ChangeParent(current);
                    }
                }
            }
            else
            {
                // optimization for close node
                TrueAStarNode tempStar = new TrueAStarNode(current, item, target);
                if (closeList[closedListContainIndex].F() > tempStar.F())
                {
                    closeList.RemoveAt(closedListContainIndex);
                    openList.Add(new TrueAStarNode(current, item, target));
                }
            }
        }
    }

    public void GenerateTrueAStarPath(Vector2 position, Vector2 target, List<Vector2> path, int limit)
    {
        List<TrueAStarNode> openList = new List<TrueAStarNode>();
        List<TrueAStarNode> closeList = new List<TrueAStarNode>();
        List<TrueAStarNode> pathList = new List<TrueAStarNode>();

        int count = 0;
        TrueAStarNode current = new TrueAStarNode(null, position, target);
        openList.Add(current);
        do
        {
            ++count;
            SortList(openList);

            current = openList[0];
            closeList.Add(current);
            openList.Remove(current);

            if (current._location == target)
            {
                break;
            }

            ProcessAdjacents(current, openList, closeList, target);
        } while (openList.Count > 0 && count <= limit);

        // add the reverse path from the closedList
        bool reachedFirst = false;
        if (closeList.Count > 0)
        {
            pathList.Add(closeList[closeList.Count - 1]);
        }
        else
        {
            reachedFirst = true;
        }
        do
        {
            if (pathList[pathList.Count - 1]._parent != null)
            {
                pathList.Add(pathList[pathList.Count - 1]._parent);
            }
            else
            {
                reachedFirst = true;
            }
        } while (!reachedFirst);

        pathList.Reverse();

        foreach (TrueAStarNode pathItem in pathList)
        {
            path.Add(pathItem._location);
        }

        if (_showAStarPath && path.Count > 0)
        {
            foreach (Vector2 item in path)
            {
                GameObject instance = Instantiate(_PathDirectionTile, item, Quaternion.identity);
                instance.transform.SetParent(_boardHolder);
                _pathDirectionTiles.Add(instance);
            }
        }
    }

    public bool GenerateNextStepOnTrueAStarPath(Vector2 position, Vector2 target, int limit, out Vector2 movePos)
    {
        bool ret = false;
        movePos = position;
        List<Vector2> curPath = new List<Vector2>();
        GenerateTrueAStarPath(position, target, curPath, limit);
        if (curPath.Count > 0)
        {
            int i = curPath.Count - 1;
            do
            {
                if (FindAStarCell(curPath[i], false, false) != null)
                {
                    ret = true;
                    movePos = curPath[i];
                }
                --i;
            } while (!ret && i >= 0);
        }
        return ret;
    }
    #endregion
}
