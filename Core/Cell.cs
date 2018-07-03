using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public int _X;
    public int _Y;

    /*
     * if equal or greater than 10, than sqaure not passable
     * */
    public int _cost = 0;

    // Zone of Disruption, this value would be positive if hero nearby
    // negative if Enemy nearby
    public int _ZoD = 0;

    //public float _evasionBonus = 0;
    //public float _accuracyPenalty = 0;

    //bool _highSpot = false;
    //bool _lowSpot = false;

    public enum Terrians : int
    {
        Walkable, // draw nothing on editor
        NonAccess,
        Forest, // agility increase
        Fort, // defense increase
        AttackingSpot, // attack increase
        Danger, // all stats decrease
    };
    public Terrians _terrian = Terrians.Walkable;

    public bool isPlayer = false;
    public bool isEnemy = false;

    // weight of each cell pertaining to player
    public int _playerWeight = 0;
    // weight of each cell pertaining to enemy
    public int _enemyWeight = 0;

    // spill blood on this cell location increase _crimsonScore
    private int _crimsonScore = 0;
    public enum Crystal_Spawns : int
    {
        None,
        Small,
        Med,
        Large,
    };
    public Crystal_Spawns _crystal_spawn = Crystal_Spawns.None;

    public Cell(int x, int y, int cost, int pWeight, int eWeight)
    {
        _X = x;
        _Y = y;
        _cost = cost;
        _playerWeight = pWeight;
        _enemyWeight = eWeight;
    }

    public void CrystalScoreInc(int value)
    {
        if (value > 0)
        {
            switch (_crystal_spawn)
            {
                case Crystal_Spawns.None:
                    {
                        _crimsonScore += value;
                    }
                    break;
                case Crystal_Spawns.Small:
                    {
                        if (Random.Range(0, 10) > 7)
                            _crystal_spawn = Crystal_Spawns.Med;
                    }
                    break;
                case Crystal_Spawns.Med:
                    {
                        if (Random.Range(0, 10) > 7)
                            _crystal_spawn = Crystal_Spawns.Large;
                    }
                    break;
                case Crystal_Spawns.Large:
                    {
                        _crimsonScore = 0;
                    }
                    break;
            }
        }
    }

    public void CrystalScoreReset()
    {
        _crimsonScore = 0;
    }

    public int CrystalScoreGet()
    {
        return _crimsonScore;
    }
}
