using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionManager : Singleton<MissionManager>
{
    #region variables
    public Text _roundStatusText;
    public GameObject _screenDarken;

    public List<UnitSpawner> _spawners = new List<UnitSpawner>();
    public List<LocationTrigger> _locationTriggers = new List<LocationTrigger>();

    //[HideInInspector]
    public int _numEnemies = 0;
    //[HideInInspector]
    public int _numHeroes = 0;
    [HideInInspector]
    public bool _missionOver = false;
    [HideInInspector]
    public bool _missionWin = false;

    public int _totalPhase = 2;
    public int _curPhase = 0;

    public int _maxRound = 20;

    // mission has 2 wins conditions instead of one
    public bool _twoWinConditions = false;

    public enum WinConditions
    {
        Reach_Turn_Number,
        Kill_All_Enemies,
        Kill_Certain_Enemy,
        Reach_Certain_Location
    }
    public WinConditions _winCondition1 = WinConditions.Kill_All_Enemies;
    public WinConditions _winCondition2 = WinConditions.Reach_Certain_Location;

    // for WinCondition: Reach_Turn_Number
    public int _goldTurnNum = 10;

    public List<string> _heroVIPs = new List<string>();
    // for WinCondition: Kill Certain Enemy
    public List<string> _enemyVIPs = new List<string>();

    [HideInInspector]
    public bool _goalReached = false;
    [HideInInspector]
    public bool _enemyEscaped = false;
    // for WinCondition: Reach_Certain_Location
    public Vector2 _goalLocation = new Vector2(-1, -1);
    public Vector2 _enemyEscapeLocation = new Vector2(-1, -1);
    public List<Vector2> _enemyPatrolDest = new List<Vector2>();

    [Header("Events")]
    public List<Event> _winEvents = new List<Event>();
    public List<Event> _lossEvents = new List<Event>();
    public int _winEventIndex = 0;
    public int _lossEventIndex = 0;
    public Event _spawnEvent = null;
    public Event _triggerEvent = null;
    public string _nextLvName = "Welcome";
    public Event _storyExplain = null;
    public Event _missionEndCutscene = null;
    [Space(20)]

    [Header("Ecounter")]
    public List<string> _ecounterActors1 = new List<string>();
    public List<string> _ecounterActors2 = new List<string>();
    public List<Event> _ecounterEvents = new List<Event>();

    [Header("Unit Withdraw")]
    public List<string> _unitDefeated = new List<string>();
    public List<Event> _unitDefeatedEvent = new List<Event>();
    #endregion

    public void CheckMissionComplete()
    {
        if (!_missionOver && !_missionWin)
        {
            CheckWinCondition(_winCondition1);
            if (!_missionWin && _twoWinConditions)
                CheckWinCondition(_winCondition2);
        }

        if (!_missionOver && !_missionWin)
        {
            CheckLossConditions();
        }
    }

    void CheckLossConditions()
    {
        if (_numHeroes <= 0)
        {
            _missionOver = true;
            MissionOverProcessing(0);
        }
        if (!_missionOver && GameManager.Instance._round > _maxRound)
        {
            _missionOver = true;
            MissionOverProcessing(2);
        }
        if (!_missionOver)
        {
            List<ChrController> bosses = GameManager.Instance._enemies.FindAll(e => (e._unitType == ChrController.UnitTypes.Enemy_Boss) && !e._isDead);
            foreach (ChrController boss in bosses)
            {
                if (CalcDistance(boss._position, _enemyEscapeLocation) < 1)
                {
                    _enemyEscaped = true;
                }
            }
            _missionOver = _enemyEscaped;
            if (_missionOver)
                MissionOverProcessing(3);
        }
        if (!_missionOver && _heroVIPs.Count > 0)
        {
            foreach (string hero in _heroVIPs)
            {
                ChrController chr = GameManager.Instance._heros.Find(h => h._name == hero);
                if (chr != null)
                {
                    if (chr._isDead)
                    {
                        _missionOver = true;
                        if (chr._unitType == ChrController.UnitTypes.Hero_Main)
                        {
                            MissionOverProcessing(1);
                        }
                        else
                        {
                            MissionOverProcessing(0);
                        }
                    }
                }
            }
        }
    }

    void CheckWinCondition(WinConditions condition)
    {
        switch (condition)
        {
            case WinConditions.Reach_Turn_Number:
                {
                    _missionWin = GameManager.Instance._round >= _goldTurnNum;
                    if (_missionWin)
                        MissionWinProcessing(2);
                }
                break;
            case WinConditions.Kill_All_Enemies:
                {
                    _missionWin = _numEnemies <= 0;
                    if (_missionWin)
                        MissionWinProcessing(1);
                }
                break;
            case WinConditions.Kill_Certain_Enemy:
                {
                    if (_enemyVIPs.Count > 0)
                    {
                        foreach (string vip in _enemyVIPs)
                        {
                            if (!_missionWin)
                            {
                                ChrController chr = GameManager.Instance._enemies.Find(h => h._name == vip);
                                if (chr != null)
                                {
                                    if (chr._isDead)
                                    {
                                        _missionWin = true;
                                        MissionWinProcessing(0);
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case WinConditions.Reach_Certain_Location:
                {
                    ChrController chr = GameManager.Instance._heros.Find(e => (e._position == _goalLocation) && !e._isDead);
                    if (chr != null)
                        _goalReached = true;
                    _missionWin = _goalReached;
                    if (_missionWin)
                        MissionWinProcessing(3);
                }
                break;
        }
    }

    LocationTrigger FindLocationTrigger(int x, int y, bool playerSide)
    {
        LocationTrigger ret = null;

        ret = _locationTriggers.Find(t => (t._locationX == x) && (t._locationY == y) && (t._playerSide == playerSide));

        return ret;
    }

    #region Mission/Game State Changes

    public void CheckMissionEvent(int x, int y, int type)
    {
        if (_triggerEvent != null)
        {
            EventManager.Instance.PlaySpecialEvent(_triggerEvent);
            _triggerEvent = null;
        }
        else
        {
            CheckMeetingEvent(x, y, type);
        }
    }

    public void CheckTriggerEvent(int x, int y, int type)
    {
        if (_triggerEvent == null)
        {
            LocationTrigger temp = FindLocationTrigger(x, y, type == 1 ? true : false);
            if (temp != null)
            {
                temp.TriggerCheck();
            }
        }
    }

    public void CheckMeetingEvent(int x, int y, int type)
    {
        if ((_ecounterEvents.Count) > 0 && (_ecounterEvents.Count == _ecounterActors1.Count) && (_ecounterEvents.Count == _ecounterActors2.Count))
        {
            bool found = false;
            for (int i = 0; !found && i < _ecounterEvents.Count; i++)
            {
                ChrController actor1 = GameManager.Instance._heros.Find(h => h._name == _ecounterActors1[i] && !h._isDead);
                if (actor1 == null)
                    actor1 = GameManager.Instance._enemies.Find(h => h._name == _ecounterActors1[i] && !h._isDead);

                if (actor1 != null)
                {
                    ChrController actor2 = GameManager.Instance._heros.Find(h => h._name == _ecounterActors2[i] && !h._isDead);
                    if (actor2 == null)
                        actor2 = GameManager.Instance._enemies.Find(h => h._name == _ecounterActors2[i] && !h._isDead);

                    if (actor2 != null)
                    {
                        if (CalcDistance(actor1._position, actor2._position) <= 1)
                        {
                            EventManager.Instance.PlaySpecialEvent(_ecounterEvents[i]);
                            found = true;
                            _ecounterActors1.RemoveAt(i);
                            _ecounterActors2.RemoveAt(i);
                            _ecounterEvents.RemoveAt(i);
                        }
                        else if (CalcDistance(actor1._position, actor2._position) == 2 && Random.Range(0, 10) > 6)
                        {
                            EventManager.Instance.PlaySpecialEvent(_ecounterEvents[i]);
                            found = true;
                            _ecounterActors1.RemoveAt(i);
                            _ecounterActors2.RemoveAt(i);
                            _ecounterEvents.RemoveAt(i);
                        }
                    }
                }
            }
            if (!found)
            {
                CheckDeathEvent(x, y, type);
            }
        }
        else
        {
            CheckDeathEvent(x, y, type);
        }
    }

    public void CheckDeathEvent(int x, int y, int type)
    {
        if (_unitDefeated.Count > 0 && _unitDefeated.Count == _unitDefeatedEvent.Count)
        {
            bool found = false;
            for (int i = 0; !found && i < _unitDefeated.Count; i++)
            {
                ChrController actor1 = GameManager.Instance._heros.Find(h => h._name == _unitDefeated[i] && h._isDead);
                if (actor1 == null)
                    actor1 = GameManager.Instance._enemies.Find(h => h._name == _unitDefeated[i] && h._isDead);
                if (actor1 != null)
                {
                    EventManager.Instance.PlaySpecialEvent(_unitDefeatedEvent[i]);
                    found = true;
                    _unitDefeated.RemoveAt(i);
                    _unitDefeatedEvent.RemoveAt(i);
                }
            }
            if (!found)
            {
                GameManager.Instance.FinishUnitTurn(type);
            }
        }
        else
        {
            GameManager.Instance.FinishUnitTurn(type);
        }
    }

    public void MissionOverProcessing(int eventIndex)
    {
        HintManager.Instance.StopHintPlay(true);
        EventManager.Instance.PlaySpecialEvent(_lossEvents[eventIndex]);
        //Invoke("MissionEnd", 5.0f);
        //if (_roundStatusText != null)
        //{
        //    _roundStatusText.gameObject.SetActive(true);
        //    _roundStatusText.text = "Defeat!";
        //}
    }

    public void MissionWinProcessing(int eventIndex)
    {
        HintManager.Instance.StopHintPlay(true);
        EventManager.Instance.PlaySpecialEvent(_winEvents[eventIndex]);
        //Invoke("MissionEnd", 5.0f);
        //if (_roundStatusText != null)
        //{
        //    _roundStatusText.gameObject.SetActive(true);
        //    _roundStatusText.text = "Victory!";
        //}
    }

    public void MissionOverEnd()
    {
        _missionOver = true;
        PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.None;
        if (_roundStatusText != null)
        {
            if (_screenDarken != null)
            {
                _screenDarken.SetActive(true);
            }
            _roundStatusText.gameObject.SetActive(true);
            _roundStatusText.color = Color.red;
            _roundStatusText.text = "Defeat!";
        }
        SoundManager.Instance.PlayMissionOverSound();
        Invoke("MissionEnd", 3.5f);
    }

    public void MissionWinEnd()
    {
        Invoke("MissionWindEnd_Delay", 0.1f);
    }

    public void MissionWindEnd_Delay()
    {
        if (_missionEndCutscene == null)
        {
            MissionWinEnd_AfterEndCutscene();
        }
        else
        {
            EventManager.Instance.PlaySpecialEvent(_missionEndCutscene);
        }
    }

    public void MissionWinEnd_AfterEndCutscene()
    {
        _missionWin = true;
        PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.None;
        if (_roundStatusText != null)
        {
            if (_screenDarken != null)
            {
                _screenDarken.SetActive(true);
            }
            _roundStatusText.gameObject.SetActive(true);
            _roundStatusText.color = Color.green;
            _roundStatusText.text = "Victory!";
        }
        SoundManager.Instance.PlayMissionWinSound();
        Invoke("MissionEnd", 3.5f);
    }

    public void MissionEnd()
    {
        if (_missionWin)
            SceneManager.LoadScene(_nextLvName);
        else if (_missionOver)
            SceneManager.LoadScene("Welcome");
    }

    public void HeroDied()
    {
        --_numHeroes;
    }

    public void EnemyDied()
    {
        --_numEnemies;
    }

    public void SpawnUnits()
    {
        if (_spawners.Count > 0)
        {
            foreach (UnitSpawner spawner in _spawners)
            {
                spawner.SpawnUnit(GameManager.Instance._round);
            }
            Invoke("SpawnDialogueCheck", 0.1f);
        }
        else
        {
            GameManager.Instance.NewRoundAfterSpawn();
        }
    }

    public void SpawnDialogueCheck()
    {
        if (_spawnEvent != null)
        {
            EventManager.Instance.PlaySpecialEvent(_spawnEvent);
            _spawnEvent = null;
        }
        else
        {
            GameManager.Instance.NewRoundAfterSpawn();
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
