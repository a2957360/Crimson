using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>, ISerializationCallbackReceiver
{
    #region Variables

    #region Components
    //BoardManager _boardScript;
    LoadBoardFromXML _boardXMLScript;
    public SlotMachineInputManager SlotMachine;
    #endregion

    #region Editor Variables
    public int _mapWidth = 10;
    public int _mapHeight = 10;
    public int _borderWidth = 2;
    public float _mapCenterX = 4.5f;
    public float _mapCenterY = 4.5f;

    public Text _roundText;
    public Text _roundStatusText;
    public float _roundStartDuration = 2.0f;
    public float _enemyActionAfterDelay = 1.0f;

    // Momentum
    [Range(0, 100)]
    public int _momentum = 0;
    public Text _momentumText;
    public GameObject _momentumPointer;
    public GameObject _momentumHero;
    public GameObject _momentumEnemy;
    public GameObject _momentumHeroBG;
    public GameObject _momentumEnemyBG;
    [HideInInspector]
    public bool _momentumReached = false;

    #endregion

    #region Game Variables
    public enum GameStates : int
    {
        Title,
        Prepare,
        InProgress,
        PostBattle,
        BetweenMission,
    };
    public GameStates _gameState = GameStates.InProgress;

    public int _level = 0;
    //[HideInInspector]
    public int _round = 0;
    private float _gameOverTimer = 0;

    //[HideInInspector]
    public ChrController _curUnit;
    //[HideInInspector]
    public ChrController _curEnemy;
    //[HideInInspector]
    public ChrController _curPlayerEnemy;
    [HideInInspector]
    public bool _playerPhase = false;
    [HideInInspector]
    public bool _enemyPhase = false;

    //XML
    public Game loadxml = new Game();
    public string xmlfile;
    #endregion

    #region Collections
    public List<ChrController> _heros = new List<ChrController>();
    public List<ChrController> _enemies = new List<ChrController>();
    public Dictionary<Vector2, Cell> _cells = new Dictionary<Vector2, Cell>();
    public List<PickupController> _crystals = new List<PickupController>();
    public List<Vector2> _crystalsToSpawn = new List<Vector2>();
    #endregion

    #region Debug
    public bool _debugMode = true;
    public GameObject _imageDarken;
    #endregion

    #region Crystal Control
    //public bool _crystalSpawnFull = false;
    //public bool _crystalSpawnLimited = false;
    public GameObject _Healing_Crystal;
    //public GameObject Healing_Crystal_M;
    //public GameObject Healing_Crystal_L;
    #endregion

    #region Misc
    // ISerializationCallbackReceiver
    public List<Vector2> _keys = new List<Vector2>();
    public List<Cell> _values = new List<Cell>();
    private Transform _crystalHolder;
    #endregion

    #endregion

    #region Core GameFlow

    void Awake()
    {
        //_boardScript = GetComponent<BoardManager>();
        _boardXMLScript = GetComponent<LoadBoardFromXML>();
        InitGame();
    }

    void Start()
    {
        MissionStart();
    }

    void Update()
    {
    }

    #endregion

    #region Functions

    void InitGame()
    {
        //_level = 5;
        //_numEnemies = Mathf.Clamp((int)(_level * 0.5f + 2), 0, 7);
        //_numHeroes = Mathf.Clamp((int)(_level * 0.5f + 1), 0, 5);
        //if (_boardScript != null)
        //{

        //    _boardScript.columns = _mapWidth;
        //    _boardScript.rows = _mapHeight;
        //    _boardScript.SetupScene(_level);
        //}

        if (_boardXMLScript != null)
        {
            loadxml = loadxml.LoadXML(Application.dataPath + "/" + xmlfile + ".xml");
            if (loadxml != null)
            {
                _mapWidth = loadxml.levels[0].width;
                _mapHeight = loadxml.levels[0].height;
                _boardXMLScript.columns = _mapWidth;
                _boardXMLScript.rows = _mapHeight;
                _boardXMLScript.SetupScene();
                _crystalHolder = new GameObject("Crystals").transform;
            }
        }
    }

    // type 1: hero 2: enemy
    public void FindUnit(int x, int y, int type)
    {
        if (MissionManager.Instance._missionOver || MissionManager.Instance._missionWin)
        {
            return;
        }
        ChrController unit = null;
        if (type == 1 && _playerPhase)
        {
            unit = _heros.Find(s => ((int)s._position.x == x) && ((int)s._position.y == y) && (s._turn < 200) && (!s._isDead));
            //MenuController.Instance.ShowSelectionList = true;
        }
        else if (type == 2 && _enemyPhase)
            unit = _enemies.Find(s => ((int)s._position.x == x) && ((int)s._position.y == y) && (s._turn < 200) && (!s._isDead));

        if (unit != null)
        {
            //if (unit._isDead)
            //{
            //    return;
            //}
            DeselectCurUnit();
            _curUnit = unit;
            _curUnit.UnitSelected();
            if (_playerPhase)
                PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.PathSelection;
            //else if (_enemyPhase)
            //    AIDirector.Instance._pathSelectionMode = true;
        }
    }

    //to check enemy movement and moved hero's movement
    public void FindUnitMovement(int x, int y)
    {
        if (MissionManager.Instance._missionOver || MissionManager.Instance._missionWin)
        {
            return;
        }
        ChrController unit = null;
        unit = _heros.Find(s => ((int)s._position.x == x) && ((int)s._position.y == y) && (s._turn > 200));
        if (unit == null)
        {
            unit = _enemies.Find(s => ((int)s._position.x == x) && ((int)s._position.y == y) && (s._turn < 200));
        }

        if (unit != null)
        {
            if (unit._isDead)
            {
                return;
            }
            DeselectCurUnit();
            _curUnit = unit;
            _curUnit.UnitCheckMovement();
        }
    }

    public void DeselectCurUnit()
    {
        if (_curUnit != null)
        {
            _curUnit.UnitDeSelected();
        }
        _curUnit = null;
        _curEnemy = null;
        _curPlayerEnemy = null;
        if (_playerPhase)
            PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.ChrSelection;
        //else if (_enemyPhase)
        //    AIDirector.Instance._pathSelectionMode = false;
    }

    public void SoftDeselectCurUnit()
    {
        if (_curUnit != null)
        {
            bool ret = _curUnit.SoftDeselectUnit();
            if (ret)
            {
                DeselectCurUnit();
            }
        }
    }

    public void MoveCurrentUnit()
    {
        if (_curUnit != null)
        {
            // we do not need final check
            //AStarPathfinding.Instance.GeneratePath(targetPos);
            //if (_curUnit.tag == "Enemy")
            //{
            //    AStarPathfinding.Instance.GeneratePath(targetPos, false);
            //}
            if (AStarPathfinding.Instance._path.Count > 0)
            {
                if (_playerPhase)
                    PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.None;
                //else if (_enemyPhase)
                //    AIDirector.Instance._watchMode = true;
                _curUnit.MoveToPosition(AStarPathfinding.Instance.GivePath());
                //AStarPathfinding.Instance._templePath.Clear();
            }
            else
            {
                _curUnit.TurnFinished();
            }
        }
    }

    public void IncreaseMomentum(int amount)
    {
        _momentum = Mathf.Clamp(_momentum + amount, 0, 100);

        //if (_momentum >= 100 && !_playerPhase)
        //    _momentum = 99;

        //if (_momentum >= 100)
        //    _momentumReached = true;

        //UpdateMomentumText();
        Invoke("UpdateMomentumText", 2f);
    }

    //public void MomentumMiniGame()
    //{
    //    //SlotMachine.gameObject.SetActive(true);
    //    //SlotMachine.restart();
    //    //PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.SlotMachine;
    //    PlayerTurnTrulyFinished();
    //}

    #endregion

    #region Mission Flow

    // type 1: hero 2: enemy
    public void UnitTurnFinished(int type)
    {
        //Debug.Log(_curUnit + " " + _curEnemy);           
        if (_curUnit != null && type == 1 && _curEnemy != null)
        {
            PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.None;
            CombatManager.Instance.BattleSetup(_curUnit, _curEnemy, type == 1 ? CombatManager.BattleTypes.Offensive : CombatManager.BattleTypes.Defensive);
        }
        else if (_curUnit != null && type == 2 && _curPlayerEnemy != null)
        {
            PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.None;
            //CombatManager.Instance.BattleSetup(_curPlayerEnemy, _curUnit, type == 1 ? CombatManager.BattleTypes.Offensive : CombatManager.BattleTypes.Defensive);
            CameraManager.Instance.FollowTargetByVector(_curPlayerEnemy._position);
            Invoke("enemyBattledeplay", 0.5f);
        }
        else
        {
            if (_curUnit != null)
                MissionManager.Instance.CheckMissionEvent((int)_curUnit._position.x, (int)_curUnit._position.y, type);
            else
                FinishUnitTurn(type);
        }
    }

    void enemyBattledeplay()
    {
        CombatManager.Instance.BattleSetup(_curPlayerEnemy, _curUnit, CombatManager.BattleTypes.Defensive);
    }

    public void FinishUnitTurn(int type)
    {
        //SpillBlood((int)_curUnit._position.x, (int)_curUnit._position.y, UnityEngine.Random.Range(1, 6), false);
        ChrController temp = _curUnit;

        DeselectCurUnit();
        //PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.ChrSelection;
        //else if (type == 2)
        //    AIDirector.Instance._watchMode = false;

        MissionManager.Instance.CheckMissionComplete();

        if (!MissionManager.Instance._missionOver && !MissionManager.Instance._missionWin)
        {
            if (type == 1)
            {
                if (_heros.Count > 0)
                {
                    PlayerTurnTrulyFinished();
                    //if (_momentumReached && temp != null && !temp._isDead)
                    //{
                    //    temp.NewTurnStarted();
                    //    _momentum = 0;
                    //    _momentumReached = false;
                    //    UpdateMomentumText();
                    //    Invoke("MomentumMiniGame", 0.1f);
                    //}
                    //else
                    //{
                    //    PlayerTurnTrulyFinished();
                    //}
                }
            }
            else if (type == 2)
            {
                //EnemyActions();
                Invoke("EnemyActions", _enemyActionAfterDelay);
            }
        }
    }

    public void PlayerTurnTrulyFinished()
    {
        SortList(_heros);
        UpdateRoundText();
        if (_heros[0]._turn >= 200)
        {
            //EventManager.Instance.FindCalendarEvent(_level, _round, 1);
            SetGamePhase(false, true);
            EnemyPhaseStart();
            UpdateRoundText();
        }
    }

    public void FinishAllCharacterturn()
    {
        if (_playerPhase)
        {
            SortHeroes();
            for (int i = 0; i < _heros.Count; i++)
            {
                if (_heros[0]._turn < 200)
                {
                    _heros[0].TurnFinished();
                }
            }
            SoundManager.Instance.PlayMusic(true, false, 0);
        }
    }

    public void MissionStart()
    {
        _round = 0;
        SetGamePhase(false, false);
        UpdateMomentumText();
        //Invoke("NewRound", 0.2f);
        if (MissionManager.Instance._storyExplain == null)
        {
            EventManager.Instance.FindGameStateEvent(_level, 0);
        }
        else
        {
            EventManager.Instance.PlaySpecialEvent(MissionManager.Instance._storyExplain);
            MissionManager.Instance._storyExplain = null;
        }
    }

    public void NewRound()
    {
        WeatherManager.Instance.OpenWeather();
        _round += 1;
        SetGamePhase(false, false);
        if (_round == 1)
        {
            CenterOnUnit(true);
            SoundManager.Instance.PlayMusic(true, true, _level + 2);
        }

        MissionManager.Instance.CheckMissionComplete();

        if (!MissionManager.Instance._missionOver && !MissionManager.Instance._missionWin)
        {
            MissionManager.Instance.SpawnUnits();

            //if (_roundStatusText != null)
            //{
            //    _roundStatusText.text = "R" + _round + " Player Phase";
            //    _roundStatusText.gameObject.SetActive(true);
            //}

            //Invoke("NewRoundStart", _roundStartDuration);

            //if (_round == 1)
            //{
            //    NewRoundStart();
            //}
            //else
            //{

            //}
        }
    }

    public void NewRoundAfterSpawn()
    {
        CenterOnUnit(true);

        if (_roundStatusText != null)
        {
            _roundStatusText.color = new Color(1.0f, 0.847f, 0, 1.0f);
            _roundStatusText.text = "Round " + _round + ": Player Phase";
            _roundStatusText.gameObject.SetActive(true);
            if (_imageDarken != null)
                _imageDarken.SetActive(true);
            SoundManager.Instance.PlayerTurnSound();
        }

        Invoke("NewRoundStart", _roundStartDuration);
    }

    private void NewRoundStart()
    {
        _roundStatusText.gameObject.SetActive(false);
        if (_imageDarken != null)
            _imageDarken.SetActive(false);
        SetGamePhase(true, false);
        //_crystalSpawnFull = true;

        DeselectCurUnit();

        PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.ChrSelection;

        foreach (ChrController chr in _heros)
        {
            chr.NewTurnStarted();
        }

        foreach (ChrController chr in _enemies)
        {
            chr.NewTurnStarted();
        }

        UpdateRoundText();
        //free camera
        CameraManager.Instance.UnfollowTarget();

        //if (_round == 1)
        //    EventManager.Instance.FindGameStateEvent(_level, 0);
        //else
        EventManager.Instance.FindCalendarEvent(_level, _round);

        //if (_round > 1)
        //    GenerateCrystals();
    }

    private void EnemyPhaseStart()
    {
        if (_roundStatusText != null)
        {
            _roundStatusText.color = Color.cyan;
            //_roundStatusText.color = Color.blue;
            _roundStatusText.text = "Round " + _round + ": Enemy Phase";
            _roundStatusText.gameObject.SetActive(true);
            if (_imageDarken != null)
                _imageDarken.SetActive(true);
            SoundManager.Instance.PlayEnemyTurnSound();
        }
        //_crystalSpawnFull = true;
        CenterOnUnit(false);

        //GenerateCrystals();

        HintManager.Instance.StopHintPlay(false);

        Invoke("EnemyActions", _roundStartDuration);
    }

    private void EnemyActions()
    {
        _roundStatusText.gameObject.SetActive(false);
        if (_imageDarken != null)
            _imageDarken.SetActive(false);
        if (_enemies.Count > 0)
        {
            SortList(_enemies);

            if (_enemies[0]._turn >= 200)
            {
                NewRound();
            }
            else
            {
                _curUnit = _enemies[0];
                _curUnit.UnitSelected();
                AIDirector.Instance.DecideAction(_curUnit);
            }
        }
        else
        {

        }
    }

    #endregion

    #region Helpers

    public void CenterOnUnit(bool player)
    {
        ChrController chr = null;
        if (player)
            chr = _heros.Find(h => (h._isDead == false) && h._name == "Agnes");
        //else
        //    chr = _enemies.Find(h => h._isDead == false);
        if (chr != null)
            CameraManager.Instance.FollowTargetByVector(chr._position);
    }

    // type 1: hero 2: enemy
    public ChrController CheckUnit(int x, int y, int type)
    {
        ChrController unit = null;
        if (type == 1)
            unit = _heros.Find(s => ((int)s._position.x == x) && ((int)s._position.y == y) && (s._isDead == false));
        else if (type == 2)
            unit = _enemies.Find(s => ((int)s._position.x == x) && ((int)s._position.y == y) && (s._isDead == false));

        //if (unit != null)
        //{
        //    if (unit._isDead)
        //    {
        //        return null;
        //    }
        //}
        return unit;
    }

    public void SetGamePhase(bool playerPhase, bool enemyPhase)
    {
        _playerPhase = playerPhase;
        _enemyPhase = enemyPhase;
        PlayerInputManager.Instance._playerPhase = playerPhase;
        AIDirector.Instance._enemyPhase = enemyPhase;
    }

    private void SortHeroes()
    {
        SortList(_heros);
    }

    private void SortEnemies()
    {
        SortList(_enemies);
    }

    private void SortList(List<ChrController> unitList)
    {
        unitList.Sort((unit1, unit2) => unit1._turn.CompareTo(unit2._turn));
    }

    private void UpdateRoundText()
    {
        if (_roundText != null)
        {
            if (_playerPhase)
            {
                int totalPlayer = _heros.FindAll(h => !h._isDead).Count;
                int unfinished = _heros.FindAll(h => !h._isDead && h._turn < 200).Count;
                _roundText.text = "Round " + _round + " | Player (" + unfinished + "/" + totalPlayer + ")";
            }
            else
            {
                int totalEnemies = _enemies.FindAll(h => !h._isDead).Count;
                _roundText.text = "Round " + _round + " | Enemy (" + totalEnemies + ")";
            }
        }
    }

    private void UpdateMomentumText()
    {
        if (_momentumText != null)
            _momentumText.text = "Momentum: " + _momentum;
        if (_momentumPointer != null)
        {
            //300 is half length of the bar 3.5 is the mid point of the bar
            _momentumPointer.transform.localPosition = new Vector3((((float)_momentum / 50) * 300f) - 300f, _momentumPointer.transform.localPosition.y, 0);
            _momentumHero.transform.localScale = new Vector3((float)_momentum / 100f, _momentumHero.transform.localScale.y, _momentumHero.transform.localScale.z);
            _momentumHeroBG.transform.localScale = new Vector3((float)_momentum / 100f, (float)_momentum / 100f, _momentumHero.transform.localScale.z);
            _momentumEnemy.transform.localScale = new Vector3((-(100f - (float)_momentum) / 100f), _momentumEnemy.transform.localScale.y, _momentumEnemy.transform.localScale.z);
            _momentumEnemyBG.transform.localScale = new Vector3((100f - (float)_momentum) / 100f, (100f - (float)_momentum) / 100f, _momentumEnemy.transform.localScale.z);
        }
    }
    #endregion

    #region Functions: Cell Manipluation 

    // type 1: for heroZoD, 2: for enemyZoD
    public void SetZoD(int x, int y, int type)
    {
        ChangeZoD(x + 1, y, type);
        ChangeZoD(x - 1, y, type);
        ChangeZoD(x, y + 1, type);
        ChangeZoD(x, y - 1, type);
    }

    // type 1: for heroZoD, 2: for enemyZoD
    private void ChangeZoD(int x, int y, int type)
    {
        Cell newCell = GetCell(x, y);
        if (newCell != null)
        {
            if (type == 1)
                newCell._ZoD += 1;
            else if (type == 2)
                newCell._ZoD -= 1;
        }
    }

    // type 0: obstacle, type 1: for heroZoD, 2: for enemyZoD
    public void ChangeCellCost(int x, int y, int type, int cost)
    {
        Cell newCell = GetCell(x, y);
        if (newCell != null)
        {
            newCell._cost += cost;
            if (type != 0)
            {
                SetZoD(x, y, type);
            }
            if (type == 1 && cost == 10)
            {
                newCell.isPlayer = true;
            }
            else
            {
                newCell.isPlayer = false;
            }
            if (type == 2 && cost == 10)
            {
                newCell.isEnemy = true;
            }
            else
            {
                newCell.isEnemy = false;
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        Cell newCell = null;
        _cells.TryGetValue(new Vector2(x, y), out newCell);
        return newCell;
    }

    public bool GetCell_IsWalkable(int x, int y)
    {
        bool ret = false;
        Cell newCell = GetCell(x, y);
        if (newCell != null && newCell._cost <= 11)
        {
            ret = true;
        }
        return ret;
    }

    public void SpillBlood(int x, int y, int amount, bool splash)
    {
        //Cell newCell = GetCell(x, y);
        //if (newCell != null && newCell._cost <= 11)
        //{
        //    if (_debugMode)
        //        amount += 50;
        //    newCell.CrystalScoreInc(amount);
        //    if (splash)
        //    {
        //        SpillBlood(x, y - 1, (int)UnityEngine.Random.Range(amount * 0.2f, amount * 0.5f), false);
        //        SpillBlood(x, y + 1, (int)UnityEngine.Random.Range(amount * 0.2f, amount * 0.5f), false);
        //        SpillBlood(x + 1, y, (int)UnityEngine.Random.Range(amount * 0.2f, amount * 0.5f), false);
        //        SpillBlood(x - 1, y, (int)UnityEngine.Random.Range(amount * 0.2f, amount * 0.5f), false);
        //    }
        //    CheckIfSpawnCrystal(x, y);
        //}
    }

    public void CheckIfSpawnCrystal(int x, int y)
    {
        //Cell newCell = GetCell(x, y);
        //if (newCell != null && newCell._cost <= 11 && newCell.CrystalScoreGet() > 60)
        //{
        //    if (newCell.CrystalScoreGet() > 180)
        //    {
        //        _crystalsToSpawn.Add(new Vector2(x, y));
        //        newCell._crystal_spawn = Cell.Crystal_Spawns.Large;
        //        newCell.CrystalScoreReset();
        //    }
        //    else if (newCell.CrystalScoreGet() > 120)
        //    {
        //        if (UnityEngine.Random.Range(0, 10) > 6)
        //        {
        //            _crystalsToSpawn.Add(new Vector2(x, y));
        //            newCell._crystal_spawn = Cell.Crystal_Spawns.Med;
        //            newCell.CrystalScoreReset();
        //        }
        //    }
        //    else
        //    {
        //        if (UnityEngine.Random.Range(0, 10) > 7)
        //        {
        //            _crystalsToSpawn.Add(new Vector2(x, y));
        //            newCell._crystal_spawn = Cell.Crystal_Spawns.Small;
        //            newCell.CrystalScoreReset();
        //        }
        //    }
        //}
    }

    public void GenerateCrystals()
    {
        //for (int i = _crystalsToSpawn.Count - 1; i >= 0; i--)
        //{
        //    Cell newCell = GetCell((int)_crystalsToSpawn[i].x, (int)_crystalsToSpawn[i].y);
        //    if (newCell._cost < 10 && newCell._crystal_spawn != Cell.Crystal_Spawns.None && UnityEngine.Random.Range(0, 10) > 6)
        //    {
        //        GameObject instance = Instantiate(_Healing_Crystal, _crystalsToSpawn[i], Quaternion.identity);
        //        instance.transform.SetParent(_crystalHolder);
        //        PickupController crystalCtrl = instance.GetComponent<PickupController>();
        //        _crystals.Add(crystalCtrl);
        //        switch (newCell._crystal_spawn)
        //        {
        //            case Cell.Crystal_Spawns.Small:
        //                {
        //                    crystalCtrl.SetCrystalSize(1, _crystalsToSpawn[i]);
        //                }
        //                break;
        //            case Cell.Crystal_Spawns.Med:
        //                {
        //                    crystalCtrl.SetCrystalSize(2, _crystalsToSpawn[i]);
        //                }
        //                break;
        //            case Cell.Crystal_Spawns.Large:
        //                {
        //                    crystalCtrl.SetCrystalSize(3, _crystalsToSpawn[i]);
        //                }
        //                break;
        //        }
        //        newCell._crystal_spawn = Cell.Crystal_Spawns.None;
        //        _crystalsToSpawn.RemoveAt(i);
        //    }
        //}
    }

    //public void SpawnCrystal(bool fullMode)
    //{
    //    if (_round >= 3 && Healing_Crystal_S != null && Healing_Crystal_M != null && Healing_Crystal_L != null)
    //    {
    //        if (fullMode || UnityEngine.Random.Range(0, 10) > 7)
    //        {
    //            //List<Cell> cellList = _cells.
    //        }
    //    }
    //}

    public void ClearShadowBoard()
    {
        if (_heros.Count > 0)
        {
            foreach (ChrController item in _heros)
                Destroy(item.gameObject);
        }

        if (_enemies.Count > 0)
        {
            foreach (ChrController item in _enemies)
                Destroy(item.gameObject);
        }
        _cells.Clear();
        _heros.Clear();
        _enemies.Clear();
    }

    #endregion

    #region ISerializationCallbackReceiver
    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        foreach (var kvp in _cells)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        _cells = new Dictionary<Vector2, Cell>();

        for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
            _cells.Add(_keys[i], _values[i]);
    }
    #endregion
}

