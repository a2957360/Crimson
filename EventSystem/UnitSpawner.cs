using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : FieldEvent
{
    public List<GameObject> _spawnList = new List<GameObject>();

    public int _startRound = 1;

    public int _coolDown = 1;

    private int _coolDownCount = 0;

    public int _totalSpawn = 5;

    public int _numSpawn = 0;

    public int _mission = 1;

    // really becareful when using this, multiple spawn events can cause game crash.
    public bool _spawnDialogue = false;
    public Event _spawnEvent;

    public enum SpawnAIPatterns : int
    {
        Guard, // stay at spawn pt, attack enemies to come in range
        Random, // move around randomly
        Approach, // try to approach a certain target
        Patrol,
        Retreat,
        Kill_Main, // trying to kill the main hero: agnes
    };
    public SpawnAIPatterns _SpawnAIPattern = SpawnAIPatterns.Approach;

    void Start()
    {
        _numSpawn = 0;
        _coolDownCount = _coolDown;
    }

    /*
     * Phase
     * 0: PlayerPhase
     * 1: EnemyPhase
     * */
    public void SpawnUnit(int round)
    {
        if (_active && round >= _startRound && _numSpawn < _totalSpawn && _mission == GameManager.Instance._level)
        {
            if (_coolDownCount >= _coolDown)
            {
                Cell curCell = GameManager.Instance.GetCell(_locationX, _locationY);
                if (curCell != null && curCell._cost < 10)
                {
                    GameObject toInstantiate = _spawnList[Random.Range(0, _spawnList.Count)];
                    GameObject instance = Instantiate(toInstantiate, new Vector3(_locationX, _locationY, 0f), Quaternion.identity) as GameObject;
                    GameManager.Instance.ChangeCellCost(_locationX, _locationY, _playerSide ? 1 : 2, 10);
                    ChrController chr = instance.GetComponent<ChrController>();
                    if (chr != null)
                    {
                        if (_playerSide)
                        {
                            GameManager.Instance._heros.Add(chr);
                            MissionManager.Instance._numHeroes++;
                            GameObject heroHolder = GameObject.Find("Hero");
                            if (heroHolder != null)
                                instance.transform.SetParent(heroHolder.transform);
                        }
                        else
                        {
                            //chr._AIPattern = ChrController.AIPatterns.Approach;
                            chr.ChangeAIPattern((int)_SpawnAIPattern);
                            GameManager.Instance._enemies.Add(chr);
                            MissionManager.Instance._numEnemies++;
                            GameObject enemyHolder = GameObject.Find("Enemy");
                            if (enemyHolder != null)
                                instance.transform.SetParent(enemyHolder.transform);
                        }
                    }
                    _coolDownCount = 0;
                    _numSpawn++;
                    if (_spawnDialogue)
                    {
                        _spawnDialogue = false;
                        MissionManager.Instance._spawnEvent = _spawnEvent;
                    }
                }
            }
            else
            {
                _coolDownCount++;
            }
        }
    }
}
