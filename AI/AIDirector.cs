using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : Singleton<AIDirector>
{
    #region Variables
    // these variables do nothing for now
    [HideInInspector]
    public bool _enemyPhase = false;
    [HideInInspector]
    //public bool _pathSelectionMode = false;
    //[HideInInspector]
    //public bool _watchMode = false;

    public float _delay = 1.0f;

    private ChrController _curUnit;

    TargetCell _target = null;
    //Vector2 _attPos;
    #endregion

    #region Gameflow

    #endregion

    #region Functions

    public void DecideAction(ChrController unit)
    {
        _curUnit = unit;
        if (unit._turn < 200)
        {
            _target = AStarPathfinding.Instance.FindRandomTarget();
            if (_curUnit._AIPattern == ChrController.AIPatterns.Guard && _target == null)
            {
                Invoke("DecideActionDelay", _delay * 0.5f);
            }
            else
            {
                Invoke("DecideActionDelay", _delay);
            }
        }
    }

    private void DecideActionDelay()
    {
        if (_curUnit.HP_Percent() < 0.5f && _curUnit._AIPattern != ChrController.AIPatterns.Retreat && Random.Range(0, 10) > 7)
        {
            GameManager.Instance._curPlayerEnemy = null;
            RandomWander();
        }
        else
        {
            //_target = AStarPathfinding.Instance.FindRandomTarget();
            switch (_curUnit._AIPattern)
            {
                case ChrController.AIPatterns.Random:
                    {
                        if (_target != null && Random.Range(0, 2) == 0)
                        {
                            GameManager.Instance._curPlayerEnemy = _target._targetChr;
                            _target.GetRandomAttackRoute();
                        }
                        else
                        {
                            GameManager.Instance._curPlayerEnemy = null;
                            RandomWander();
                            if (Random.Range(0, 10) > 6)
                            {
                                _curUnit._AIPattern = ChrController.AIPatterns.Guard;
                            }
                        }
                        //GameManager.Instance.MoveCurrentUnit();                     
                    }
                    break;
                case ChrController.AIPatterns.Guard:
                    {
                        if (_target != null)
                        {
                            GameManager.Instance._curPlayerEnemy = _target._targetChr;
                            _target.GetRandomAttackRoute();
                        }
                        else
                        {
                            GameManager.Instance._curPlayerEnemy = null;
                            AStarPathfinding.Instance.ClearPath();
                            if (_curUnit.HP_Percent() < 0.9f && Random.Range(0, 10) > 6)
                            {
                                _curUnit._AIPattern = ChrController.AIPatterns.Approach;
                            }
                        }
                    }
                    break;
                case ChrController.AIPatterns.Approach:
                    {
                        if (_target != null)
                        {
                            GameManager.Instance._curPlayerEnemy = _target._targetChr;
                            _target.GetRandomAttackRoute();
                        }
                        else
                        {
                            GameManager.Instance._curPlayerEnemy = null;
                            FindPathtoEnemy();
                        }
                    }
                    break;
                case ChrController.AIPatterns.Patrol:
                    {
                        if (_target != null)
                        {
                            GameManager.Instance._curPlayerEnemy = _target._targetChr;
                            _target.GetRandomAttackRoute();
                        }
                        else
                        {
                            GameManager.Instance._curPlayerEnemy = null;
                            PatrolToPoint();
                        }
                    }
                    break;
                case ChrController.AIPatterns.Retreat:
                    {
                        if (_target != null)
                        {
                            if (CalcDistance(_curUnit._position, _target._targetChr._position) <= 2)
                            {
                                if (Random.Range(0, 10) > 6)
                                {
                                    GameManager.Instance._curPlayerEnemy = _target._targetChr;
                                    _target.GetRandomAttackRoute();
                                }
                                else
                                {
                                    GameManager.Instance._curPlayerEnemy = null;
                                    RetreatToPoint();
                                }
                            }
                            else
                            {
                                if (Random.Range(0, 10) > 8)
                                {
                                    GameManager.Instance._curPlayerEnemy = _target._targetChr;
                                    _target.GetRandomAttackRoute();
                                }
                                else
                                {
                                    GameManager.Instance._curPlayerEnemy = null;
                                    RetreatToPoint();
                                }
                            }
                        }
                        else
                        {
                            GameManager.Instance._curPlayerEnemy = null;
                            RetreatToPoint();
                        }
                    }
                    break;
            } // end of switch
        }
        GameManager.Instance.MoveCurrentUnit();
    }

    void RetreatToPoint()
    {
        if (_curUnit._AIPattern != ChrController.AIPatterns.Retreat)
        {
            DecideActionDelay();
            return;
        }
        Vector2 Path = new Vector2();
        if (AStarPathfinding.Instance.GenerateNextStepOnTrueAStarPath(_curUnit._position, MissionManager.Instance._enemyEscapeLocation, 70, out Path))
        {
            AStarPathfinding.Instance.GeneratePath(Path);
        }
        else
        {
            RandomWander();
        }
    }

    void PatrolToPoint()
    {
        if (_curUnit._position == _curUnit._patrolPt)
        {
            if (MissionManager.Instance._enemyPatrolDest.Count <= 1)
            {
                _curUnit.ChangeAIPattern(2);
            }
            else
            {
                bool ret = false;
                for (int i = 0; i < MissionManager.Instance._enemyPatrolDest.Count && !ret; i++)
                {
                    if (_curUnit._position != MissionManager.Instance._enemyPatrolDest[i])
                    {
                        _curUnit._patrolPt = MissionManager.Instance._enemyPatrolDest[i];
                        ret = true;
                    }
                }
                if (!ret)
                    _curUnit.ChangeAIPattern(2);
            }
        }
        if (_curUnit._AIPattern != ChrController.AIPatterns.Patrol)
        {
            DecideActionDelay();
            return;
        }
        Vector2 Path = new Vector2();
        if (AStarPathfinding.Instance.GenerateNextStepOnTrueAStarPath(_curUnit._position, _curUnit._patrolPt, 60, out Path))
        {
            AStarPathfinding.Instance.GeneratePath(Path);
        }
        else
        {
            RandomWander();
        }
    }

    void FindPathtoEnemy()
    {
        float distance = GameManager.Instance._mapHeight > GameManager.Instance._mapWidth ? GameManager.Instance._mapHeight : GameManager.Instance._mapWidth * 2;
        Vector2 NearestEnemyPosition = FindNearestEnemy();
        Vector2 Path = new Vector2();
        if (GameManager.Instance._heros.Find(s => s._position == NearestEnemyPosition && s._isDead == false) != null)
        {
            if (AStarPathfinding.Instance.GenerateNextStepOnTrueAStarPath(_curUnit._position, NearestEnemyPosition, 60, out Path))
            {
                AStarPathfinding.Instance.GeneratePath(Path);
            }
            else
            {
                RandomWander();
            }
            //for (int i = 0; i < AStarPathfinding.Instance._AStarCells.Count; i++)
            //{
            //    if (distance > Vector2.Distance(AStarPathfinding.Instance._AStarCells[i]._position, NearestEnemyPosition))
            //    {
            //        distance = Vector2.Distance(AStarPathfinding.Instance._AStarCells[i]._position, NearestEnemyPosition);
            //        Path = AStarPathfinding.Instance._AStarCells[i]._position;
            //    }
            //}
        }
        //AStarPathfinding.Instance.GeneratePath(Path);
        //GameManager.Instance.MoveCurrentUnit(Path);
    }

    Vector2 FindNearestEnemy()
    {
        float distance = GameManager.Instance._mapHeight > GameManager.Instance._mapWidth ? GameManager.Instance._mapHeight : GameManager.Instance._mapWidth * 2;
        Vector2 NearestEnemyPosition = new Vector2();
        for (int i = 0; i < GameManager.Instance._heros.Count; i++)
        {
            if (!GameManager.Instance._heros[i]._isDead)
            {
                if (distance > Vector2.Distance(_curUnit._position, GameManager.Instance._heros[i]._position))
                {
                    distance = Vector2.Distance(_curUnit._position, GameManager.Instance._heros[i]._position);
                    NearestEnemyPosition = GameManager.Instance._heros[i]._position;
                }
            }
        }
        return NearestEnemyPosition;
    }

    //private void CheckForNearbyEnemy(int x, int y)
    //{
    //    ChrController chr = GameManager.Instance.CheckUnit(x + GameManager.Instance._curUnit._AttackRange, y, 1);
    //    if (chr == null)
    //        chr = GameManager.Instance.CheckUnit(x - GameManager.Instance._curUnit._AttackRange, y, 1);
    //    if (chr == null)
    //        chr = GameManager.Instance.CheckUnit(x, y + GameManager.Instance._curUnit._AttackRange, 1);
    //    if (chr == null)
    //        chr = GameManager.Instance.CheckUnit(x, y - GameManager.Instance._curUnit._AttackRange, 1);

    //    if (GameManager.Instance._curUnit._AttackRange > 1)
    //    {
    //        for (int i = 1; i < GameManager.Instance._curUnit._AttackRange; i++)
    //        {
    //            if (chr == null)
    //                chr = GameManager.Instance.CheckUnit(x + i, y - (GameManager.Instance._curUnit._AttackRange - i), 1);
    //            if (chr == null)
    //                chr = GameManager.Instance.CheckUnit(x - i, y - (GameManager.Instance._curUnit._AttackRange - i), 1);
    //            if (chr == null)
    //                chr = GameManager.Instance.CheckUnit(x + i, y + (GameManager.Instance._curUnit._AttackRange - i), 1);
    //            if (chr == null)
    //                chr = GameManager.Instance.CheckUnit(x - i, y + (GameManager.Instance._curUnit._AttackRange - i), 1);
    //        }
    //    }

    //    GameManager.Instance._curPlayerEnemy = chr;
    //}

    //private bool AttackNearbyEnemy()
    //{
    //    CheckForNearbyEnemy((int)_curUnit._position.x, (int)_curUnit._position.y);
    //    bool ret = GameManager.Instance._curPlayerEnemy != null;
    //    if (ret)
    //        _curUnit.TurnFinished();
    //    return ret;
    //}

    //private bool CheckIfCanMove()
    //{
    //    bool ret = true;
    //    int count = AStarPathfinding.Instance._AStarCells.Count;

    //    if (count <= 0) // no where to move, nothing to attack, something not right
    //    {
    //        ret = false;
    //        _curUnit.TurnFinished();
    //    }
    //    return ret;
    //}

    //public void GenerateRandomAttPosForTarget()
    //{
    //    if (_target != null)
    //    {
    //        _attPos = _target._attackPositions[Random.Range(0, _target._attackPositions.Count)];
    //    }
    //}

    public void RandomWander()
    {
        List<AStarCell> availableAstarCell = AStarPathfinding.Instance._AStarCells.FindAll(s => !s._isEnemy);
        int count = availableAstarCell.Count;
        if (count > 0)
        {
            AStarPathfinding.Instance.GeneratePath(availableAstarCell[Random.Range(0, count)]._position);
        }
        else
        {
            AStarPathfinding.Instance.ClearPath();
        }
    }

    private int CalcDistance(Vector2 pos1, Vector2 pos2)
    {
        int distance = 0;
        distance = (int)(Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y));
        return distance;
    }
    #endregion
}
