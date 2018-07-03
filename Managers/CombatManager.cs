using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : Singleton<CombatManager>
{
    #region Variables
    public enum BattleTypes : int
    {
        Offensive,
        Defensive,
        Duel,
    };
    public BattleTypes _battleType = BattleTypes.Offensive;
    public bool _isOffensiveBattle = false;
    public bool _isDefensiveBattle = false;

    //public GameObject _combatant1;
    //public GameObject _combatant2;

    public ChrController _playerCtrl;
    public ChrController _enemyCtrl;
    public ChrController _friendCtrl;
    public List<CombatFriend> _friends = new List<CombatFriend>();
    public int _curFriend = 0;
    public bool _friendAssist = false;
    public bool _isAssisting = false;

    // places for the combat body doubles
    public GameObject _LeftCharacter;
    public GameObject _RightCharacter;
    public GameObject _FriendCharacter;
    CombatAnimationController _leftAnimationController;
    CombatAnimationController _RightAnimationController;
    CombatAnimationController _friendAnimationController;

    //particle effects
    public ParticleController leftParticle;
    public ParticleController rightParticle;

    public enum BattleProgress : int
    {
        None,
        Setup,
        Decision,
        Animation,
        Results,
        CleanUp,
        FriendAssist,
    };
    public BattleProgress _progress = BattleProgress.None;

    [Header("UI")]
    public Color _healthDmgColor;
    public Color _healthRecoveryColor;
    public Color _neutralColor;
    public Color _triumphColor;
    public Color _deflatedColor;

    public Text _battleResultText;
    public Text _battleScoreText;
    public Text _timingSpeedText;
    public Text _enemyTimingSpeedText;

    public Text _decisionText;

    public Text _battleInfo2_player;
    public Text _battleInfo2_enemy;
    public Text _battleInfo1_player;
    public Text _battleInfo1_enemy;

    public Text _triadStatus;
    public Text _locationStatus;
    public Text _timingStatus;
    public CombatDecisionVisual _decisionCtrl;
    public string _battleResult = "";
    public GameObject _BGImage;
    public GameObject _BGDarken;
    public Text _friendText;

    public Image _playerBattleTellVisual;
    public Image _enemyBattleTellVisual1;
    public Image _enemyBattleTellVisual2;
    public Image _triadHelpImageHolder;
    public Sprite _tradHelpImage1;
    public Sprite _tradHelpImage2;

    public GameObject _BattleDecisionsVisual;
    public Text _battleTitle;
    public GameObject _battleInfo_player;
    public GameObject _battleInfo_enemy;
    private BattleResultController _BattleDecisionsVisualCtrl;
    public BattleMoveTextController _BattleMoveCtrl;
    [Space(20)]

    [Header("Decisons")]
    public int _decisionValue = -1;
    public int _timingDecisionValue = 0;
    public int _locationDecisionValue = 0;
    public string _playerDecisionName;
    //[HideInInspector]
    /*
     * 1 : Power or Guard
     * 2 : Technique or Dodge
     * 3 : Precision or Counter
     */
    public int _enemyMove = 0;

    //[HideInInspector]
    /*
    * 1 : High attack or guessing opponent high attack
    * 0 : Med attack or guessing opponent med attack
    * -1 : Low attack or guessing opponent low attack
    */
    public int _enemyLocation = 0;
    //[HideInInspector]
    public string _enemyMoveName;
    [Space(20)]

    [Header("Battle Flow")]
    public bool _easyMode = false;
    public bool _superEasyMode = false;
    public bool _isAuto = false;
    public bool _isInstantDeath = false;
    public bool _debug = false;
    public bool _enemyTiming = false;
    public float _decisionTime = 3.0f;
    public float _animationTime = 3.0f;
    public float _resultsTime = 3.0f;
    public float _battleStartDelay = 0.5f;
    [Space(20)]

    #region Combat stats

    /*
     * 0 - 0.5: bad outcome, 0.5 - 0.8: neutral: 0.8 > : good outcome 
     * */
    private float _battleScore = 0;

    /*
     * 1 : triad sucessful for player
     * 0 : triad tie for player
     * -1 : triad fail for player
     * */
    private int _triad = 0;

    // player's offensive location is different than enemy's defense
    // or player's defensive location is same as enemy's
    private bool _locationSuccess = false;

    // player successfully their action's speed in comparison to enemy's
    private bool _timingSuccess = false;

    private int _playerDmg = 0;
    private int _enemyDmg = 0;
    private float _attackerSpeedHigh = 0;
    private float _attackerSpeedLow = 0;
    private float _attackerSpeed;

    private float _defenderSpeedHigh = 0;
    private float _defenderSpeedLow = 0;
    private float _defenderSpeed;

    private bool _fatal = false;
    private bool _canCounter = false;
    private int _distance = 0;

    [Header("Combat")]
    // High dmg, critical hit wound enemy
    public float _powerAttModifier = 1.2f;
    // Med dmg, better momentum increase, critical hit slow enemy 
    public float _precisionAttModifier = 1.0f;
    // Low dmg, recover health, critical hit weaken enemy
    public float _technicalAttModifier = 0.8f;
    // for guard on precision
    public float _goodGuardModifier = 0.2f;
    // for other guards
    public float _weakGuardModifier = 0.7f;
    public float _normalGuardModifier = 0.5f;
    // for grazing attack
    public float _grazingModifier = 0.15f;
    // for normal counter
    public float _counterModifier = 0.7f;
    public float _criticalAttackModifier = 0.2f;

    public int _powerHighSpeed = 3;
    public int _powerLowSpeed = 5;
    public int _precisionHighSpeed = 2;
    public int _precisionLowSpeed = 3;
    public int _techniqueHighSpeed = 1;
    public int _techniqueLowSpeed = 4;

    public int _dodgeHighSpeed = 2;
    public int _dodgeLowSpeed = 4;
    public int _guardHighSpeed = 1;
    public int _guardLowSpeed = 3;
    public int _counterHighSpeed = 3;
    public int _counterLowSpeed = 5;

    public int _momentumOnDodge = 10;
    public int _momentumOnBlock = 10;
    public int _momentumOnCounter = 15;
    public int _momentumOnPower = 25;
    public int _momentumOnTechnique = 20;
    public int _momentumOnPrecision = 30;
    public int _momentumOnHurt = -15;
    public int _momentumOnBlocked = -5;
    public int _momentumOnMissed = -10;
    public int _momentumOnCountered = -15;
    [Space(20)]
    #endregion

    #region Tutorial

    public List<Event> _offensiveEvents = new List<Event>();
    public List<Event> _defensiveEvents = new List<Event>();

    public bool _isTutorial = false;
    private int _tutorialOffensiveIndex = 0;
    private int _tutorialDefensiveIndex = 0;

    public bool _isBattleSelectPhase = true;
    private bool _bufferOnPlayerSide = true;
    #endregion

    #endregion

    #region Gameflow

    private void Update()
    {
        //locate bg on camera position
        _BGImage.transform.position = CameraManager.Instance._camPos;
    }

    private void Start()
    {
        Invoke("setBGSize", 0.2f);
    }

    #endregion

    #region Game Logical Flow

    public void BattleSetup(ChrController chr1, ChrController chr2, BattleTypes battle)
    {
        CameraManager.Instance.curcamera.orthographicSize = 3;
        _playerCtrl = chr1;
        _enemyCtrl = chr2;
        _battleType = battle;
        _progress = BattleProgress.Setup;
        if (_playerCtrl != null && _enemyCtrl != null)
        {
            if (_easyMode)
            {
                GameManager.Instance.IncreaseMomentum(20);
                if (!PlayerInputManager.Instance._MapGrid && !PlayerInputManager.Instance._ExtraPath)
                {
                    _superEasyMode = true;
                }
                else
                {
                    _superEasyMode = false;
                }
            }
            else
            {
                _superEasyMode = false;
            }
            HintManager.Instance.StopHintPlay(false);
            _playerCtrl.AdjustStats();
            _enemyCtrl.AdjustStats();
            CombatBodySetup();
            EnemyDecisionMaking();
            DisplayTerrianInfo();
            _BattleDecisionsVisualCtrl = _BattleDecisionsVisual.GetComponent<BattleResultController>();

            _curFriend = 0;
            _isBattleSelectPhase = true;
            _friendAssist = false;
            _isAssisting = false;
            _friends.Add(new CombatFriend());
            FindFriends();
            //DisplayPlayerActionName();

            _decisionText.gameObject.SetActive(false);
            _decisionValue = -1;

            _isAuto = false;
            _isInstantDeath = false;

            //SetHitStatus("", "", "", "");

            if (_debug)
                _battleResultText.gameObject.SetActive(true);
            else
                _battleResultText.gameObject.SetActive(false);

            _battleResultText.text = "";
            _BGImage.SetActive(true);
            _battleInfo_player.SetActive(true);
            _battleInfo_enemy.SetActive(true);
            //camera control
            CameraManager.Instance._canFreeMove = false;
            _playerDmg = 0;
            _enemyDmg = 0;
            _battleResult = "";

            CalcDistance();

            if (_battleType == BattleTypes.Offensive)
            {
                _battleTitle.text = "Offense";
                _isOffensiveBattle = true;
            }
            else if (_battleType == BattleTypes.Defensive)
            {
                _battleTitle.text = "Defense";
                _isDefensiveBattle = true;
            }

            if (_isTutorial && _offensiveEvents.Count > 0 && _defensiveEvents.Count > 0)
            {
                BattleTutorial();
            }
            else
            {
                _progress = BattleProgress.Decision;
                BattleDecision();
            }
        }
        else
        {
            _progress = BattleProgress.None;
        }
    }

    public void BattleTutorial()
    {
        if (_battleType == BattleTypes.Offensive)
        {
            if (_tutorialOffensiveIndex < _offensiveEvents.Count)
            {
                EventManager.Instance.PlaySpecialEvent(_offensiveEvents[_tutorialOffensiveIndex]);
                _tutorialOffensiveIndex++;
            }
            else
            {
                if (_tutorialDefensiveIndex >= _defensiveEvents.Count)
                    _isTutorial = false;
                _progress = BattleProgress.Decision;
                BattleDecision();
            }
        }
        else if (_battleType == BattleTypes.Defensive)
        {
            if (_tutorialDefensiveIndex < _defensiveEvents.Count)
            {
                EventManager.Instance.PlaySpecialEvent(_defensiveEvents[_tutorialDefensiveIndex]);
                _tutorialDefensiveIndex++;
            }
            else
            {
                if (_tutorialOffensiveIndex >= _offensiveEvents.Count)
                    _isTutorial = false;
                _progress = BattleProgress.Decision;
                BattleDecision();
            }
        }
    }

    public void BattleDecision()
    {
        SoundManager.Instance.PlayBattleMusic(true, true);
        CalcAndDisplaySpeed();

        if (GameManager.Instance._momentum < 15)
        {
            Invoke("BattleAuto", 0.1f);
        }
        else if (GameManager.Instance._momentum > 80 && GameManager.Instance._enemyPhase)
        {
            Invoke("BattleDecisionDelayStart", 0.1f);
        }
        else
        {
            if (GameManager.Instance._momentum >= 60 && GameManager.Instance._playerPhase)
            {
                // show B Button: Instant Kill
                PlayerInputManager.Instance.BattleConfirmSelectionButtonB.SetActive(true);
                PlayerInputManager.Instance.BattleCSBGShort.SetActive(false);
                PlayerInputManager.Instance.BattleCSBGLong.SetActive(true);
            }
            else
            {
                PlayerInputManager.Instance.BattleConfirmSelectionButtonB.SetActive(false);
                PlayerInputManager.Instance.BattleCSBGShort.SetActive(true);
                PlayerInputManager.Instance.BattleCSBGLong.SetActive(false);
            }

            if (_battleType == BattleTypes.Offensive)
            {
                EventManager.Instance.InsertDialogue(true, _playerCtrl._portraitCombat, _playerCtrl._dialogue_favAttack, false);
                EventManager.Instance.InsertDialogue(false, _enemyCtrl._portraitCombat, _enemyCtrl._dialogue_favDefense, false);

                _playerBattleTellVisual.gameObject.SetActive(true);
                _playerBattleTellVisual.sprite = _playerCtrl._Fav_Offense_Icon;
                _enemyBattleTellVisual1.gameObject.SetActive(true);
                _enemyBattleTellVisual1.sprite = _enemyCtrl._Fav_Defense_Icon;
            }
            else if (_battleType == BattleTypes.Defensive)
            {
                EventManager.Instance.InsertDialogue(true, _playerCtrl._portraitCombat, _playerCtrl._dialogue_favDefense, false);
                EventManager.Instance.InsertDialogue(false, _enemyCtrl._portraitCombat, _enemyCtrl._dialogue_favAttack, false);

                _playerBattleTellVisual.gameObject.SetActive(true);
                _playerBattleTellVisual.sprite = _playerCtrl._Fav_Defense_Icon;
                _enemyBattleTellVisual1.gameObject.SetActive(true);
                _enemyBattleTellVisual1.sprite = _enemyCtrl._Fav_Offense_Icon;
            }
            Invoke("BattleSelectionPhase", 0.1f);
        }
    }

    public void BattleSelectionPhase()
    {
        PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.BattleTypeDecision;
        PlayerInputManager.Instance.BattleConfirmSelection.SetActive(true);
        if (_battleType == BattleTypes.Offensive)
        {
            if (_triadHelpImageHolder != null && _tradHelpImage1 != null)
                _triadHelpImageHolder.sprite = _tradHelpImage1;
        }
        else if (_battleType == BattleTypes.Defensive)
        {
            if (_triadHelpImageHolder != null && _tradHelpImage2 != null)
                _triadHelpImageHolder.sprite = _tradHelpImage2;
        }
    }

    // auto Battle
    public void BattleAuto()
    {
        _isBattleSelectPhase = false;
        _battleTitle.text += ": Auto";
        EventManager.Instance.StopEventPlay();
        PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.None;
        _isAuto = true;
        EnemyDecisionMaking();
        PlayerAutoDecisionMaking();
        Invoke("BattleAnimation", _battleStartDelay);
    }

    public void BattleInstantDeath()
    {
        _isBattleSelectPhase = false;
        PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.None;
        _isInstantDeath = true;
        EventManager.Instance.StopEventPlay();
        Invoke("BattleInstantDeathCalc", _battleStartDelay);
    }

    // this serve as now the strategy portion of battle
    public void BattleDecisionDelayStart()
    {
        _isBattleSelectPhase = false;
        _battleTitle.text += ": Manual";
        //_BattleDecisionsVisual.SetActive(true);

        GameManager.Instance.IncreaseMomentum(-15);
        if (_decisionCtrl != null)
            _decisionCtrl.StartDecisionVisual(_battleType == BattleTypes.Offensive);


        int enemy_int = _enemyCtrl.intelligence;
        if ((_easyMode || _enemyCtrl.HP_Percent() < 0.3f) && enemy_int > 1)
            enemy_int--;

        if (!_easyMode && enemy_int < 3 && Random.Range(0, 10) > 6)
            enemy_int++;

        switch (_battleType)
        {
            case BattleTypes.Offensive:
                {
                    if (_BattleMoveCtrl != null)
                    {
                        _BattleMoveCtrl.SetColorText("Precision", _decisionValue);
                    }

                    _BattleDecisionsVisualCtrl.ShowBattleResult(3, 7);
                    //_distanceTextEnemy.text = "Distance: " + _distance + "\nCounter: " + _canCounter;
                    EventManager.Instance.FindBattleEvent(_enemyMove + 3, enemy_int, _enemyLocation, _playerCtrl._portraitCombat, _enemyCtrl._portraitCombat);
                }
                break;
            case BattleTypes.Defensive:
                {
                    if (_BattleMoveCtrl != null)
                    {
                        _BattleMoveCtrl.SetColorText("Counter", _decisionValue);
                    }

                    _BattleDecisionsVisualCtrl.ShowBattleResult(6, 7);
                    //_distanceText.text = "Distance: " + _distance + "\nCounter: " + _canCounter;
                    EventManager.Instance.FindBattleEvent(_enemyMove, enemy_int, _enemyLocation, _playerCtrl._portraitCombat, _enemyCtrl._portraitCombat);
                }
                break;
        }
        PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.Battle;
        DisplayFriendText();

        //Invoke("BattleDecisionEnd", _decisionTime);
    }

    public void BattleDecisionEnd()
    {
        DestoryChildObj(_FriendCharacter);
        BattleAnimation();
    }

    public void BattleInstantDeathCalc()
    {
        _BGDarken.SetActive(true);
        _battleTitle.text += ": Blitz";
        _progress = BattleProgress.Animation;
        //hide particle
        leftParticle.disactiveBuff();
        rightParticle.disactiveBuff();

        float killChance = (float)GameManager.Instance._momentum * 0.01f - 0.15f + (_playerCtrl._Agility - _enemyCtrl._Agility) * 0.01f;
        if (_enemyCtrl._unitType == ChrController.UnitTypes.Enemy_Boss)
            killChance -= 0.2f;

        if (_easyMode)
            killChance += 0.2f;

        EventManager.Instance.InsertDialogue(true, _playerCtrl._portraitCombat, _playerCtrl._dialogue_instantKill, false);
        _leftAnimationController.StartAttackAnimation("att power");

        if (Random.Range(0.0f, 1.0f) < killChance)
        {
            _battleResultText.color = _healthRecoveryColor;
            _RightAnimationController.StartDefenseAnimation("hurt");
            _battleResult = "Fatal Attack Hit Enemy! Massive Damage!";
            _fatal = true;
            _enemyDmg = 199;
            SetHitStatus("Fatal Hit", "Hurt", "", "", 1, -1, 0, 0);
            GameManager.Instance.IncreaseMomentum(-30);

        }
        else
        {
            _battleResultText.color = _healthDmgColor;
            _RightAnimationController.StartDefenseAnimation("dodge");
            _battleResult = "Enemy Dodged Fatal Attack!";
            SetHitStatus("Miss", "Survive", "", "", -1, 1, 0, 0);
            GameManager.Instance.IncreaseMomentum(-50);
        }
        if (_decisionCtrl != null)
            _decisionCtrl.DisplayDecisionVisual(false, false, 1);

        Invoke("BattleAnimationDelayStart", 0.1f);
    }

    public void BattleAnimation()
    {
        EventManager.Instance.StopEventPlay();
        PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.None;
        _progress = BattleProgress.Animation;
        //hide particle
        leftParticle.disactiveBuff();
        rightParticle.disactiveBuff();

        _attackerSpeed = Random.Range(_attackerSpeedHigh, _attackerSpeedLow);
        _defenderSpeed = Random.Range(_defenderSpeedHigh, _defenderSpeedLow);

        switch (_timingDecisionValue)
        {
            case 0:
                {
                    if (Mathf.Abs(_attackerSpeed - _defenderSpeed) <= 1.0f)
                        _timingSuccess = true;
                    else
                        _timingSuccess = false;
                }
                break;
            case -1:
                {
                    if (_battleType == BattleTypes.Offensive)
                    {
                        if ((_attackerSpeed - _defenderSpeed) > 1.0f)
                            _timingSuccess = true;
                        else
                            _timingSuccess = false;
                    }
                    else
                    {
                        if ((_defenderSpeed - _attackerSpeed) > 1.0f)
                            _timingSuccess = true;
                        else
                            _timingSuccess = false;
                    }
                }
                break;
            case 1:
                {
                    if (_battleType == BattleTypes.Offensive)
                    {
                        if ((_attackerSpeed - _defenderSpeed) < 1)
                            _timingSuccess = true;
                        else
                            _timingSuccess = false;
                    }
                    else
                    {
                        if ((_defenderSpeed - _attackerSpeed) < 1)
                            _timingSuccess = true;
                        else
                            _timingSuccess = false;
                    }
                }
                break;
        }

        if (_battleType == BattleTypes.Offensive)
        {
            if (_locationDecisionValue != _enemyLocation)
                _locationSuccess = true;
            else
                _locationSuccess = false;
        }
        else
        {
            if (_locationDecisionValue != _enemyLocation)
                _locationSuccess = false;
            else
                _locationSuccess = true;
        }

        if (_decisionValue >= 1) // Power attack or Block
        {
            switch (_enemyMove)
            {
                case 1:
                    {
                        if (_battleType == BattleTypes.Offensive) // player: power attack, enemy: Block 
                        {
                            _triad = 0;
                            CombatCalc(2);
                        }
                        else // player: Block, enemy: power attack
                        {
                            _triad = 0;
                            CombatCalc(2);
                        }
                    }
                    break;
                case 2:
                    {
                        if (_battleType == BattleTypes.Offensive) // player: power attack, enemy: Dodge
                        {
                            _triad = -1;
                            CombatCalc(1);
                        }
                        else // player: Block, enemy: technique
                        {
                            _triad = -1;
                            CombatCalc(5);
                        }
                    }
                    break;
                case 3:
                    {
                        if (_battleType == BattleTypes.Offensive) // player: power attack, enemy: counter
                        {
                            _triad = 1;
                            CombatCalc(3);
                        }
                        else // player: Block, enemy: precision
                        {
                            _triad = 1;
                            CombatCalc(8);
                        }
                    }
                    break;
            }
        }
        else if (_decisionValue <= -1) // Precision Attack or Counter
        {
            switch (_enemyMove)
            {
                case 1:
                    {
                        if (_battleType == BattleTypes.Offensive) // player: Precision attack, enemy: Block
                        {
                            _triad = -1;
                            CombatCalc(8);
                        }
                        else // player: Counter, enemy: power attack
                        {
                            _triad = -1;
                            CombatCalc(3);
                        }
                    }
                    break;
                case 2:
                    {
                        if (_battleType == BattleTypes.Offensive) // player: Precision attack, enemy: Dodge
                        {
                            _triad = 1;
                            CombatCalc(7);
                        }
                        else // player: Counter, enemy: technical
                        {
                            _triad = 1;
                            CombatCalc(6);
                        }
                    }
                    break;
                case 3:
                    {
                        if (_battleType == BattleTypes.Offensive) // player: Precision attack, enemy: Counter
                        {
                            _triad = 0;
                            CombatCalc(9);
                        }
                        else // player: Counter, enemy: precision
                        {
                            _triad = 0;
                            CombatCalc(9);
                        }
                    }
                    break;
            }
        }
        else // Technical Attack or Dodge
        {
            switch (_enemyMove)
            {
                case 1:
                    {
                        if (_battleType == BattleTypes.Offensive) // player: Technical attack, enemy: block
                        {
                            _triad = 1;
                            CombatCalc(5);
                        }
                        else // player: Dodge, enemy: power attack
                        {
                            _triad = 1;
                            CombatCalc(1);
                        }
                    }
                    break;
                case 2:
                    {
                        if (_battleType == BattleTypes.Offensive) // player: Technical attack, enemy: dodge
                        {
                            _triad = 0;
                            CombatCalc(4);
                        }
                        else // player: Dodge, enemy: technical
                        {
                            _triad = 0;
                            CombatCalc(4);
                        }
                    }
                    break;
                case 3:
                    {
                        if (_battleType == BattleTypes.Offensive) // player: Technical attack, enemy: counter
                        {
                            _triad = -1;
                            CombatCalc(6);
                        }
                        else // player: Dodge, enemy: precision
                        {
                            _triad = -1;
                            CombatCalc(7);
                        }
                    }
                    break;
            }
        }

        if (_decisionCtrl != null)
            _decisionCtrl.DisplayDecisionVisual(false, false, 1);

        Invoke("BattleAnimationDelayStart", 0.1f);
    }

    public void BattleAnimationDelayStart()
    {
        PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.None;
        _BGDarken.SetActive(true);
        DisplayFinalDecision();
        //int curHP = 0;
        _playerCtrl.TakeDamage(_playerDmg, _fatal);
        _enemyCtrl.TakeDamage(_enemyDmg, _fatal);
        //if (_dmg > 0)
        //{
        //    curHP = _combatant2Ctrl.TakeDamage(_dmg, _fatal);
        //    if (curHP == 0)
        //        EventManager.Instance.FindBattleOutcomeEvent(3, 1, false, _combatant1Ctrl._portraitCombat, _combatant2Ctrl._portraitCombat);
        //    else if (curHP == 1)
        //        EventManager.Instance.FindBattleOutcomeEvent(4, 1, false, _combatant1Ctrl._portraitCombat, _combatant2Ctrl._portraitCombat);
        //    else
        //        EventManager.Instance.FindBattleOutcomeEvent(2, 1, false, _combatant1Ctrl._portraitCombat, _combatant2Ctrl._portraitCombat);
        //}
        //else if (_dmg < 0)
        //{
        //    curHP = _combatant1Ctrl.TakeDamage(-_dmg, _fatal);
        //    if (curHP == 0)
        //        EventManager.Instance.FindBattleOutcomeEvent(3, 1, true, _combatant1Ctrl._portraitCombat, _combatant2Ctrl._portraitCombat);
        //    else if (curHP == 1)
        //        EventManager.Instance.FindBattleOutcomeEvent(4, 1, true, _combatant1Ctrl._portraitCombat, _combatant2Ctrl._portraitCombat);
        //    else
        //        EventManager.Instance.FindBattleOutcomeEvent(2, 1, true, _combatant1Ctrl._portraitCombat, _combatant2Ctrl._portraitCombat);
        //}
        //else
        //{
        //    EventManager.Instance.FindBattleOutcomeEvent(1, 1, false, _combatant1Ctrl._portraitCombat, _combatant2Ctrl._portraitCombat);
        //}

        CameraManager.Instance.StarCombatAnimation(_RightCharacter, _animationTime);
        Invoke("BattleAnimationEnd", _animationTime);
    }

    private void BattleAnimationEnd()
    {
        _progress = BattleProgress.Results;
        BattleResults();
    }

    public void BattleResults()
    {
        _decisionText.gameObject.SetActive(true);
        //Debug.Log(_battleResult);
        _battleResultText.text = _battleResult;
        DisplayHPChange();
        Invoke("BattleResultsEnd", _resultsTime);
        //EventManager.Instance.DisplayBattleInfo(_battleResult, _resultsTime);
    }

    private void BattleResultsEnd()
    {
        _progress = BattleProgress.FriendAssist;
        BattleAssistCheck();
    }

    public void BattleAssistCheck()
    {
        if (!_friendAssist || _friendCtrl == null || _playerCtrl._HP <= 0 || _enemyCtrl._HP <= 0)
        {
            _progress = BattleProgress.CleanUp;
            BattleCleanUp();
        }
        else
        {
            _playerDmg = 0;
            _enemyDmg = 0;
            _battleType = BattleTypes.Offensive;
            _decisionValue = (int)_friendCtrl._FA_Move;
            //_decisionText.text = "FRIEND ASSIST";
            UICleanUp_FA();
            EventManager.Instance.DeleteDialogue(false, true);
            EventManager.Instance.InsertDialogue(true, _friendCtrl._portraitCombat, _friendCtrl._dialogue_FA_Move, false);
            EnemyDecisionMaking();
            _canCounter = false;
            _isAssisting = true;
            _friendAssist = false;
            FriendBodySetup();
            Invoke("BattleAssist", 1.5f);
        }
    }

    public void BattleAssist()
    {
        _battleTitle.text = "Friend Assist";
        FriendPlayerBodiesSwitch();
        //BattleAnimation();
        Invoke("BattleAnimation", 0.1f);
    }

    public void BattleCleanUp()
    {
        if (_playerDmg > 0)
        {
            _playerCtrl.CheckDeath();
        }
        if (_enemyDmg > 0)
        {
            _enemyCtrl.CheckDeath();
        }

        CameraManager.Instance.curcamera.orthographicSize = CameraManager.Instance.ZoomSize;

        //_playerCtrl = null;
        //_enemyCtrl = null;
        _curFriend = 0;
        _friendCtrl = null;
        _friends.Clear();

        DestoryChildObj(_LeftCharacter);
        DestoryChildObj(_RightCharacter);
        DestoryChildObj(_FriendCharacter);

        UICleanUp_FA();
        _BGDarken.SetActive(false);
        _battleTitle.text = "";

        SetHitStatus("", "", "", "", 0, 0, 0, 0);
        _isOffensiveBattle = false;
        _isDefensiveBattle = false;
        _friendText.text = "";
        //_distanceTextEnemy.text = "";
        //_distanceText.text = "";
        //_playerHPDmg.text = "";
        //_enemyHPDmg.text = "";
        _decisionText.text = "";
        //_battleResultText.text = "";
        _decisionText.gameObject.SetActive(false);
        _battleInfo_player.SetActive(false);
        _battleInfo_enemy.SetActive(false);
        _timingSpeedText.text = "";
        _enemyTimingSpeedText.text = "";
        //_triadStatus.text = "";
        //_locationStatus.text = "";
        //_timingStatus.text = "";
        //_battleScoreText.text = "";
        //_decisionBar.transform.localScale = barscale;
        _decisionValue = 0;
        _BGImage.SetActive(false);
        //SetHitStatus("", "", "", "");
        //camera control
        CameraManager.Instance._canFreeMove = true;
        _progress = BattleProgress.None;
        GameManager.Instance._curEnemy = null;
        GameManager.Instance._curPlayerEnemy = null;
        bool player = GameManager.Instance._curUnit.tag == "Player";
        EventManager.Instance.StopEventPlay();
        SoundManager.Instance.PlayMusic(true, false, 0);

        GameManager.Instance.UnitTurnFinished(player ? 1 : 2);
    }

    #endregion

    #region Helpers

    public void CombatBodySetup()
    {
        //combat Animation prepare
        GameObject leftchr = Instantiate(_playerCtrl._combatBody, Vector3.zero, Quaternion.identity);
        GameObject rightchr = Instantiate(_enemyCtrl._combatBody, Vector3.zero, Quaternion.identity);
        leftchr.SetActive(true);
        leftchr.transform.localScale = new Vector3(-leftchr.transform.localScale.x, leftchr.transform.localScale.y, leftchr.transform.localScale.z);
        rightchr.SetActive(true);

        //show buff
        leftParticle.Activebuff(_playerCtrl);
        rightParticle.Activebuff(_enemyCtrl);

        DestoryChildObj(_LeftCharacter);
        DestoryChildObj(_RightCharacter);
        DestoryChildObj(_FriendCharacter);

        leftchr.transform.SetParent(_LeftCharacter.transform);
        rightchr.transform.SetParent(_RightCharacter.transform);
        leftchr.transform.localPosition = Vector3.zero;
        rightchr.transform.localPosition = Vector3.zero;
        _leftAnimationController = leftchr.GetComponentInChildren<CombatAnimationController>();
        _RightAnimationController = rightchr.GetComponentInChildren<CombatAnimationController>();
    }

    public void FriendPlayerBodiesSwitch()
    {
        if (_friendCtrl != null && _playerCtrl != null)
        {
            GameObject playerChr = Instantiate(_playerCtrl._combatBody, Vector3.zero, Quaternion.identity);
            GameObject friendChr = Instantiate(_friendCtrl._combatBody, Vector3.zero, Quaternion.identity);
            playerChr.SetActive(true);
            playerChr.transform.localScale = new Vector3(-playerChr.transform.localScale.x, playerChr.transform.localScale.y, playerChr.transform.localScale.z);
            friendChr.SetActive(true);
            friendChr.transform.localScale = new Vector3(-friendChr.transform.localScale.x, friendChr.transform.localScale.y, friendChr.transform.localScale.z);

            DestoryChildObj(_LeftCharacter);
            DestoryChildObj(_FriendCharacter);

            playerChr.transform.SetParent(_FriendCharacter.transform);
            friendChr.transform.SetParent(_LeftCharacter.transform);
            playerChr.transform.localPosition = Vector3.zero;
            friendChr.transform.localPosition = Vector3.zero;
            _leftAnimationController = friendChr.GetComponentInChildren<CombatAnimationController>();
            _playerCtrl = _friendCtrl;
        }
    }

    public void FriendBodySetup()
    {
        if (_friendCtrl != null)
        {
            GameObject friendchr = Instantiate(_friendCtrl._combatBody, Vector3.zero, Quaternion.identity);
            friendchr.SetActive(true);
            friendchr.transform.localScale = new Vector3(-friendchr.transform.localScale.x, friendchr.transform.localScale.y, friendchr.transform.localScale.z);
            DestoryChildObj(_FriendCharacter);
            friendchr.transform.SetParent(_FriendCharacter.transform);
            friendchr.transform.localPosition = Vector3.zero;
            _friendAnimationController = friendchr.GetComponentInChildren<CombatAnimationController>();
        }
    }

    public void DestoryChildObj(GameObject cur)
    {
        for (int i = 0; i < cur.transform.childCount; i++)
        {
            Destroy(cur.transform.GetChild(i).gameObject);
        }
    }

    public void FindFriends()
    {
        if (_playerCtrl != null && GameManager.Instance._momentum >= 30)
        {
            //FindSingleFriend((int)_playerCtrl._position.x - 1, (int)_playerCtrl._position.y);
            //FindSingleFriend((int)_playerCtrl._position.x + 1, (int)_playerCtrl._position.y);
            //FindSingleFriend((int)_playerCtrl._position.x, (int)_playerCtrl._position.y - 1);
            //FindSingleFriend((int)_playerCtrl._position.x, (int)_playerCtrl._position.y + 1);
            List<ChrController> herolist = new List<ChrController>();
            if (GameManager.Instance._playerPhase)
            {
                herolist = GameManager.Instance._heros.FindAll(s => !s._isDead && s._position != GameManager.Instance._curUnit._position);
            }
            else
            {
                herolist = GameManager.Instance._heros.FindAll(s => !s._isDead && s._position != GameManager.Instance._curPlayerEnemy._position);
            }
            for (int i = 0; i < herolist.Count; i++)
            {
                if (AStarPathfinding.Instance.FindAttackableHeros(herolist[i]._attackType, herolist[i]._position))
                {
                    if (herolist[i] != null && _friends.Find(s => s._chr == herolist[i]) == null)
                    {
                        _friends.Add(new CombatFriend(herolist[i], herolist[i]._name, herolist[i]._portraitCombat));
                    }
                }
            }
        }
    }

    public void FindSingleFriend(int x, int y)
    {
        ChrController chr = GameManager.Instance.CheckUnit(x, y, 1);
        if (chr != null)
            _friends.Add(new CombatFriend(chr, chr._name, chr._portraitCombat));
    }

    public void ChangeLocationValue(int val)
    {
        SoundManager.Instance.PlaySelectionChangeSound();
        _locationDecisionValue = Mathf.Clamp(_locationDecisionValue + val, -1, 1);
        //if (_locationDecisionValue >= 1 && val > 0)
        //{
        //    _locationDecisionValue = -1;
        //}
        //else if (_locationDecisionValue <= -1 && val < 0)
        //{
        //    _locationDecisionValue = 1;
        //}
        //else
        //{
        //    _locationDecisionValue = _locationDecisionValue + val;
        //}
        if (_decisionCtrl != null)
            _decisionCtrl.DisplayLocation(_locationDecisionValue);
        DisplayPlayerActionName();
    }

    public void ChangeTimingValue(int val)
    {
        _timingDecisionValue = Mathf.Clamp(_timingDecisionValue + val, -1, 1);
        if (_decisionCtrl != null)
            _decisionCtrl.DisplayTiming(_timingDecisionValue);
    }

    public void ChangeDecisionValue(int val)
    {
        SoundManager.Instance.PlaySelectionChangeSound();
        //_decisionValue = Mathf.Clamp(_decisionValue + val, -1, 1);
        //_decisionValue = val;
        if (val < 0)
        {
            _decisionValue--;
            if (_decisionValue < -1)
            {
                _decisionValue = 1;
            }
        }
        else if (val > 0)
        {
            _decisionValue++;
            if (_decisionValue > 1)
            {
                _decisionValue = -1;
            }
        }

        if (_decisionCtrl != null)
            _decisionCtrl.DisplayDecisionVisual(true, _battleType == BattleTypes.Offensive, _decisionValue);

        if (_decisionValue >= 1)
        {
            _BattleDecisionsVisualCtrl.ShowBattleResult(_battleType == BattleTypes.Offensive ? 1 : 4, 7);
            if (_BattleMoveCtrl != null)
            {
                _BattleMoveCtrl.SetColorText(_battleType == BattleTypes.Offensive ? "Power" : "Guard", _decisionValue);
            }
        }
        else if (_decisionValue <= -1)
        {
            _BattleDecisionsVisualCtrl.ShowBattleResult(_battleType == BattleTypes.Offensive ? 3 : 6, 7);
            if (_BattleMoveCtrl != null)
            {
                _BattleMoveCtrl.SetColorText(_battleType == BattleTypes.Offensive ? "Precision" : "Counter", _decisionValue);
            }
        }
        else
        {
            _BattleDecisionsVisualCtrl.ShowBattleResult(_battleType == BattleTypes.Offensive ? 2 : 5, 7);
            if (_BattleMoveCtrl != null)
            {
                _BattleMoveCtrl.SetColorText(_battleType == BattleTypes.Offensive ? "Technique" : "Dodge", _decisionValue);
            }
        }

        CalcAndDisplaySpeed();
        DisplayPlayerActionName();
    }

    public void ChangeFriendIndex(bool forward)
    {
        if (_friends.Count > 1)
        {
            SoundManager.Instance.PlaySelectionChangeSound();
            if (forward)
            {
                ++_curFriend;
                if (_curFriend >= _friends.Count)
                    _curFriend = 0;
            }
            else
            {
                --_curFriend;
                if (_curFriend < 0)
                    _curFriend = _friends.Count - 1;
            }
            DisplayFriendText();
            if (_friends[_curFriend]._chr == null)
            {
                _friendAssist = false;
                if (_friendCtrl != null)
                {
                    _friendCtrl = null;
                    GameManager.Instance.IncreaseMomentum(10);
                    EventManager.Instance.DeleteDialogue(true, false);
                    DestoryChildObj(_FriendCharacter);
                }
            }
            else
            {
                if (_friendCtrl == null)
                {
                    GameManager.Instance.IncreaseMomentum(-10);
                }
                //_friendAssist = true;
                _friendCtrl = _friends[_curFriend]._chr;
                FriendBodySetup();
                if (_battleType == BattleTypes.Offensive)
                {
                    EventManager.instance.InsertDialogue(true, _friends[_curFriend]._image, _friendCtrl._dialogue_FA_offensive, false);
                    if (_friendCtrl._FA_Offensive_Condition == ChrController.FA_Offensive_Conditions.Random && Random.Range(0, 10) > 5)
                        _friendAssist = true;
                    else
                        _friendAssist = false;
                }
                else if (_battleType == BattleTypes.Defensive)
                {
                    EventManager.instance.InsertDialogue(true, _friends[_curFriend]._image, _friendCtrl._dialogue_FA_defensive, false);
                    if (_friendCtrl._FA_Defensive_Condition == ChrController.FA_Defensive_Conditions.Random && Random.Range(0, 10) > 5)
                        _friendAssist = true;
                    else
                        _friendAssist = false;
                }
            }
        }
    }

    #endregion

    #region AI

    public void PlayerAutoDecisionMaking()
    {
        if (_isAuto)
        {
            int dec = _battleType == BattleTypes.Offensive ? (int)_playerCtrl._Fav_Offensive : (int)_playerCtrl._Fav_Defense;
            switch (dec)
            {
                case 1: // Power or Guard                    
                case 0: // Technical or Dodge                
                case -1: // Precision or Counter
                    {
                        if (Random.Range(0, 10) < 8)
                            _decisionValue = dec;
                        else
                            _decisionValue = Random.Range(-1, 2);
                    }
                    break;
                case 2: // Random
                    {
                        _decisionValue = Random.Range(-1, 2);
                    }
                    break;
            }
            _locationDecisionValue = Random.Range(-1, 2);
            _timingDecisionValue = Random.Range(-1, 2);
        }
    }

    public void EnemyDecisionMaking()
    {
        if (!_isAuto)
        {
            _enemyMove = Random.Range(1, 4);
        }
        else
        {
            int dec = _battleType == BattleTypes.Offensive ? (int)_enemyCtrl._Fav_Defense : (int)_enemyCtrl._Fav_Offensive;
            switch (dec)
            {
                case 1: // Power or Guard
                    {
                        if (Random.Range(0, 10) < 8)
                            _enemyMove = 1;
                        else
                            _enemyMove = Random.Range(1, 4);
                    }
                    break;
                case 0: // Technical or Dodge   
                    {
                        if (Random.Range(0, 10) < 8)
                            _enemyMove = 2;
                        else
                            _enemyMove = Random.Range(1, 4);
                    }
                    break;
                case -1: // Precision or Counter
                    {
                        if (Random.Range(0, 10) < 8)
                            _enemyMove = 3;
                        else
                            _enemyMove = Random.Range(1, 4);
                    }
                    break;
                case 2: // Random
                    {
                        _enemyMove = Random.Range(1, 4);
                    }
                    break;
            }
        }
        _enemyLocation = Random.Range(-1, 2);
        switch (_enemyMove)
        {
            case 1:
                {
                    _enemyMoveName = _battleType == BattleTypes.Offensive ? "GUARD" : "POWER";
                }
                break;
            case 2:
                {
                    _enemyMoveName = _battleType == BattleTypes.Offensive ? "DODGE" : "TECHNIQUE";
                }
                break;
            case 3:
                {
                    _enemyMoveName = _battleType == BattleTypes.Offensive ? "COUNTER" : "PRECISION";
                }
                break;
        }
    }

    #endregion

    #region Calc

    public void CombatCalc(int type)
    {
        SingleBattleOutcomeDecider();
        int outcome = 0;
        if (_battleScore < 0.5f) // bad outcome
        {
            _battleScoreText.color = _deflatedColor;
            outcome = -1;
        }
        else if (_battleScore < 0.8f) // neutral outcome
        {
            _battleScoreText.color = _neutralColor;
            outcome = 0;
        }
        else // good outcome
        {
            _battleScoreText.color = _triumphColor;
            outcome = 1;
        }

        switch (type)
        {
            case 1: // Power on Dodge
                {
                    if (_battleType == BattleTypes.Offensive)
                    {
                        _fatal = false;
                        _leftAnimationController.StartAttackAnimation("att power");
                        _RightAnimationController.StartDefenseAnimation("dodge");
                        switch (outcome)
                        {
                            case -1: // bad outcome
                                {
                                    _bufferOnPlayerSide = false;
                                    Invoke("CalcBuffer", 2.0f);
                                    _playerDmg = 0;
                                    _enemyDmg = 0;
                                    GameManager.Instance.IncreaseMomentum(_momentumOnMissed);
                                    _battleResultText.color = _healthDmgColor;
                                    _battleResult = "Enemy Dodged Power Attack Completely!";
                                    //SetHitStatus("MISS", "PERFECT DODGE", "", "");
                                    SetHitStatus("Miss", "Full Dodge", "", "", -1, 1, 0, 0);
                                    EventManager.Instance.InsertDialogue(false, _enemyCtrl._portraitCombat, _enemyCtrl._dialogue_perfectDodge, false);
                                }
                                break;
                            case 0:
                            case 1: // good outcome
                                {
                                    CalcDmg(false, _grazingModifier);
                                    //GameManager.Instance.IncreaseMomentum(_momentumOnMissed);
                                    _battleResultText.color = _neutralColor;
                                    _battleResult = "Attack barely grazed Enemy!";
                                    //SetHitStatus("GRAZING HIT", "ALMOST DODGE", "", "");
                                    SetHitStatus("Weak Hit", "", "", "", 0, 0, 0, 0);
                                }
                                break;
                        }
                    }
                    else
                    {
                        _fatal = false;
                        _leftAnimationController.StartAttackAnimation("dodge");
                        _RightAnimationController.StartDefenseAnimation("att power");
                        if (_friendCtrl != null && !_isAssisting)
                        {
                            if (_friendCtrl._FA_Defensive_Condition == ChrController.FA_Defensive_Conditions.Dodge_Success)
                            {
                                _friendAssist = true;
                            }
                        }
                        switch (outcome)
                        {
                            case -1:
                            case 0:
                                {
                                    CalcDmg(false, _grazingModifier);
                                    GameManager.Instance.IncreaseMomentum((int)(_momentumOnDodge * 0.7f));
                                    _battleResultText.color = _healthRecoveryColor;
                                    _battleResult = "Enemy Attack barely connects!";
                                    //SetHitStatus("ALMOST DODGE", "GRAZING HIT", "", "");
                                    SetHitStatus("", "Weak Hit", "", "", 0, 0, 0, 0);
                                }
                                break;
                            case 1:
                                {
                                    if (_friendCtrl != null && !_isAssisting)
                                    {
                                        if (_friendCtrl._FA_Defensive_Condition == ChrController.FA_Defensive_Conditions.Perfect_Action)
                                        {
                                            _friendAssist = true;
                                        }
                                    }
                                    _bufferOnPlayerSide = true;
                                    Invoke("CalcBuffer", 2.0f);
                                    _playerDmg = 0;
                                    _enemyDmg = 0;
                                    GameManager.Instance.IncreaseMomentum(_momentumOnDodge);
                                    _battleResultText.color = _healthDmgColor;
                                    _battleResult = "Dodged Power Attack completely!";
                                    //SetHitStatus("PERFECT DODGE", "MISS", "", "");
                                    SetHitStatus("Full Dodge", "Miss", "", "", 1, -1, 0, 0);
                                    EventManager.Instance.InsertDialogue(true, _playerCtrl._portraitCombat, _playerCtrl._dialogue_perfectDodge, false);
                                }
                                break;
                        }
                    }
                }
                break;
            case 2: // Power on Guard
                {
                    if (_battleType == BattleTypes.Offensive)
                    {
                        _fatal = false;
                        switch (outcome)
                        {
                            case -1:
                                {
                                    _leftAnimationController.StartAttackAnimation("att power");
                                    _RightAnimationController.StartDefenseAnimation("guard");
                                    CalcDmg(false, _goodGuardModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnBlocked);
                                    _battleResultText.color = _neutralColor;
                                    _battleResult = "Attack barely penetrates Enemy's Guard!";
                                    //SetHitStatus("BLOCKED HIT", "STRONG GUARD", "", "");
                                    SetHitStatus("", "Guard", "", "", 0, 1, 0, 0);
                                }
                                break;
                            case 0:
                                {
                                    _leftAnimationController.StartAttackAnimation("att power");
                                    _RightAnimationController.StartDefenseAnimation("guard");
                                    CalcDmg(false, _normalGuardModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnPower / 2);
                                    _battleResultText.color = _neutralColor;
                                    _battleResult = "Attack blocked by Enemy's Guard!";
                                    //SetHitStatus("BLOCKED HIT", "GUARD", "", "");
                                    SetHitStatus("Hit", "Guard", "", "", 0, 1, 0, 0);
                                }
                                break;
                            case 1:
                                {
                                    if (_friendCtrl != null && !_isAssisting)
                                    {
                                        if (_friendCtrl._FA_Offensive_Condition == ChrController.FA_Offensive_Conditions.Power_Hit)
                                        {
                                            _friendAssist = true;
                                        }
                                    }
                                    _leftAnimationController.StartAttackAnimation("att power");
                                    _RightAnimationController.StartDefenseAnimation("hurt");
                                    _fatal = true;
                                    CalcDmg(false, _weakGuardModifier);
                                    GameManager.Instance.IncreaseMomentum((int)(_momentumOnPower * 0.8f));
                                    _battleResultText.color = _healthRecoveryColor;
                                    _battleResult = "Attack broke Enemy's Guard!";
                                    SetHitStatus("Hit", "Weak Guard", "", "", 1, -1, 0, 0);
                                }
                                break;
                        }
                    }
                    else
                    {
                        _fatal = false;
                        switch (outcome)
                        {
                            case -1:
                                {
                                    _fatal = true;
                                    _leftAnimationController.StartAttackAnimation("hurt");
                                    _RightAnimationController.StartDefenseAnimation("att power");
                                    CalcDmg(false, _weakGuardModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnHurt);
                                    //GameManager.Instance.IncreaseMomentum(_momentumOnPower / 2);
                                    _battleResultText.color = _healthDmgColor;
                                    _battleResult = "Guard broke by Enemy's Attack!";
                                    //SetHitStatus("WEAK GUARD", "BLOCKED HIT", "", "");
                                    SetHitStatus("Weak Guard", "Hit", "", "", -1, 1, 0, 0);
                                }
                                break;
                            case 0:
                                {
                                    if (_friendCtrl != null && !_isAssisting)
                                    {
                                        if (_friendCtrl._FA_Defensive_Condition == ChrController.FA_Defensive_Conditions.Block_Success)
                                        {
                                            _friendAssist = true;
                                        }
                                    }
                                    _leftAnimationController.StartAttackAnimation("guard");
                                    _RightAnimationController.StartDefenseAnimation("att power");
                                    CalcDmg(false, _normalGuardModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnBlock / 2);
                                    _battleResultText.color = _neutralColor;
                                    _battleResult = "Blocked Enemy's Power Attack!";
                                    //SetHitStatus("GUARD", "BLOCKED HIT", "", "");
                                    SetHitStatus("Guard", "Hit", "", "", 1, 0, 0, 0);
                                }
                                break;
                            case 1:
                                {
                                    if (_friendCtrl != null && !_isAssisting)
                                    {
                                        if (_friendCtrl._FA_Defensive_Condition == ChrController.FA_Defensive_Conditions.Block_Success)
                                        {
                                            _friendAssist = true;
                                        }
                                    }
                                    _leftAnimationController.StartAttackAnimation("guard");
                                    _RightAnimationController.StartDefenseAnimation("att power");
                                    CalcDmg(false, _goodGuardModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnBlock);
                                    _battleResultText.color = _healthRecoveryColor;
                                    _battleResult = "Enemy's Attack barely got through!";
                                    //SetHitStatus("STRONG GUARD", "BLOCKED HIT", "", "");
                                    SetHitStatus("Guard", "", "", "", 1, 0, 0, 0);
                                }
                                break;
                        }
                    }
                }
                break;
            case 3: // Power on Counter
                {
                    if (_battleType == BattleTypes.Offensive)
                    {
                        _fatal = true;
                        _leftAnimationController.StartAttackAnimation("att power");
                        _RightAnimationController.StartDefenseAnimation("hurt");
                        GameManager.Instance.IncreaseMomentum(_momentumOnPower);
                        switch (outcome)
                        {
                            case -1:
                            case 0:
                                {
                                    if (_friendCtrl != null && !_isAssisting)
                                    {
                                        if (_friendCtrl._FA_Offensive_Condition == ChrController.FA_Offensive_Conditions.Power_Hit)
                                        {
                                            _friendAssist = true;
                                        }
                                    }
                                    CalcDmg(false, _powerAttModifier);
                                    _battleResultText.color = _healthRecoveryColor;
                                    _battleResult = "Attack overwhelmed Enemy's Counter!";
                                    //SetHitStatus("CLEAN HIT", "COUNTER FAIL", "", "");
                                    SetHitStatus("Clean Hit", "", "", "", 1, 0, 0, 0);
                                }
                                break;
                            case 1:
                                {
                                    _bufferOnPlayerSide = true;
                                    Invoke("CalcBuffer", 2.0f);
                                    if (_friendCtrl != null && !_isAssisting)
                                    {
                                        if (_friendCtrl._FA_Offensive_Condition == ChrController.FA_Offensive_Conditions.Critical_Hit
                                            || _friendCtrl._FA_Offensive_Condition == ChrController.FA_Offensive_Conditions.Power_Hit)
                                        {
                                            _friendAssist = true;
                                        }
                                    }
                                    CalcDmg(false, _powerAttModifier + _criticalAttackModifier);
                                    _battleResultText.color = _healthRecoveryColor;
                                    _battleResult = "Attack completely overwhelmed Enemy's Counter!";
                                    //SetHitStatus("CRITICAL HIT", "COUNTER FAIL", "", "");
                                    SetHitStatus("Critical Hit", "", "", "", 1, -1, 0, 0);
                                    EventManager.Instance.InsertDialogue(true, _playerCtrl._portraitCombat, _playerCtrl._dialogue_critical, false);
                                }
                                break;
                        }
                    }
                    else
                    {
                        _fatal = true;
                        _leftAnimationController.StartAttackAnimation("hurt");
                        _RightAnimationController.StartDefenseAnimation("att power");
                        GameManager.Instance.IncreaseMomentum(_momentumOnHurt);
                        switch (outcome)
                        {
                            case -1:
                                {
                                    _bufferOnPlayerSide = false;
                                    Invoke("CalcBuffer", 2.0f);
                                    CalcDmg(false, _powerAttModifier + _criticalAttackModifier);
                                    _battleResultText.color = _healthDmgColor;
                                    _battleResult = "Counter completely failed against Enemy's Attack!";
                                    //SetHitStatus("COUNTER FAIL", "CRITICAL HIT", "", "");
                                    SetHitStatus("", "Critical Hit", "", "", -1, 1, 0, 0);
                                    EventManager.Instance.InsertDialogue(false, _enemyCtrl._portraitCombat, _enemyCtrl._dialogue_critical, false);
                                }
                                break;
                            case 0:
                            case 1:
                                {
                                    CalcDmg(false, _powerAttModifier);
                                    _battleResultText.color = _healthDmgColor;
                                    _battleResult = "Counter failed against Enemy's Attack!";
                                    //SetHitStatus("COUNTER FAIL", "CLEAN HIT", "", "");
                                    SetHitStatus("", "Clean Hit", "", "", 0, 1, 0, 0);
                                }
                                break;
                        }
                    }
                }
                break;
            case 4: // Technique on Dodge
                {
                    if (_battleType == BattleTypes.Offensive)
                    {
                        _leftAnimationController.StartAttackAnimation("att technique");
                        switch (outcome)
                        {
                            case -1:
                                {
                                    _fatal = false;
                                    _playerDmg = 0;
                                    _enemyDmg = 0;
                                    _RightAnimationController.StartDefenseAnimation("dodge");
                                    GameManager.Instance.IncreaseMomentum(_momentumOnMissed);
                                    _battleResultText.color = _healthDmgColor;
                                    _battleResult = "Enemy Dodged Technique Attack!";
                                    //SetHitStatus("MISS", "DODGE", "", "");
                                    SetHitStatus("", "Dodge", "", "", 0, 1, 0, 0);
                                }
                                break;
                            case 0:
                                {
                                    _fatal = false;
                                    _RightAnimationController.StartDefenseAnimation("dodge");
                                    CalcDmg(false, _grazingModifier);
                                    //GameManager.Instance.IncreaseMomentum(_momentumOnPower / 2);
                                    _battleResult = "Technique Attack barely grazed Enemy!";
                                    _battleResultText.color = _neutralColor;
                                    //SetHitStatus("GRAZING HIT", "ALMOST DODGE", "", "");
                                    SetHitStatus("Weak Hit", "", "", "", 0, 0, 0, 0);
                                }
                                break;
                            case 1:
                                {
                                    if (_friendCtrl != null && !_isAssisting)
                                    {
                                        if (_friendCtrl._FA_Offensive_Condition == ChrController.FA_Offensive_Conditions.Technical_Hit)
                                        {
                                            _friendAssist = true;
                                        }
                                    }
                                    _fatal = true;
                                    _RightAnimationController.StartDefenseAnimation("hurt");
                                    CalcDmg(false, _technicalAttModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnTechnique);
                                    _battleResult = "Technique Attack hit dodging Enemy!";
                                    _battleResultText.color = _healthRecoveryColor;
                                    //SetHitStatus("CLEAN HIT", "DODGE FAIL", "", "");
                                    SetHitStatus("Clean Hit", "", "", "", 1, 0, 0, 0);
                                }
                                break;
                        }
                    }
                    else
                    {
                        _RightAnimationController.StartAttackAnimation("att technique");
                        switch (outcome)
                        {
                            case -1:
                                {
                                    _fatal = true;
                                    CalcDmg(false, _technicalAttModifier);
                                    _leftAnimationController.StartDefenseAnimation("hurt");
                                    GameManager.Instance.IncreaseMomentum(_momentumOnHurt);
                                    _battleResult = "Failed to Dodge Enemy's Technique Attack!";
                                    _battleResultText.color = _healthDmgColor;
                                    //SetHitStatus("DODGE FAIL", "CLEAN HIT", "", "");
                                    SetHitStatus("", "Clean Hit", "", "", 0, 1, 0, 0);
                                }
                                break;
                            case 0:
                                {
                                    _fatal = false;
                                    _leftAnimationController.StartDefenseAnimation("dodge");
                                    CalcDmg(false, _grazingModifier);
                                    //GameManager.Instance.IncreaseMomentum(_momentumOnPower / 2);
                                    _battleResult = "Almost Dodged Enemy's Technique Attack!";
                                    _battleResultText.color = _neutralColor;
                                    //SetHitStatus("ALMOST DODGE", "GRAZING HIT", "", "");
                                    SetHitStatus("", "Weak Hit", "", "", 0, 0, 0, 0);
                                }
                                break;
                            case 1:
                                {
                                    if (_friendCtrl != null && !_isAssisting)
                                    {
                                        if (_friendCtrl._FA_Defensive_Condition == ChrController.FA_Defensive_Conditions.Dodge_Success)
                                        {
                                            _friendAssist = true;
                                        }
                                    }
                                    _fatal = false;
                                    _leftAnimationController.StartDefenseAnimation("dodge");
                                    _playerDmg = 0;
                                    _enemyDmg = 0;
                                    GameManager.Instance.IncreaseMomentum(_momentumOnDodge);
                                    _battleResult = "Dodged Enemy's Technique Attack!";
                                    _battleResultText.color = _healthRecoveryColor;
                                    //SetHitStatus("DODGE", "MISS", "", "");
                                    SetHitStatus("Dodge", "", "", "", 1, 0, 0, 0);
                                }
                                break;
                        }
                    }
                }
                break;
            case 5: // Technique on Guard
                {
                    if (_battleType == BattleTypes.Offensive)
                    {
                        _leftAnimationController.StartAttackAnimation("att technique");
                        if (_friendCtrl != null && !_isAssisting)
                        {
                            if (_friendCtrl._FA_Offensive_Condition == ChrController.FA_Offensive_Conditions.Technical_Hit)
                            {
                                _friendAssist = true;
                            }
                        }
                        switch (outcome)
                        {
                            case -1:
                                {
                                    _RightAnimationController.StartDefenseAnimation("hurt");
                                    _fatal = true;
                                    CalcDmg(false, _weakGuardModifier);
                                    //GameManager.Instance.IncreaseMomentum(_momentumOnPower / 2);
                                    _battleResult = "Technique Attack barely blocked by Enemy!";
                                    _battleResultText.color = _neutralColor;
                                    //SetHitStatus("BLOCKED HIT", "WEAK GUARD", "", "");
                                    SetHitStatus("Hit", "Weak Guard", "", "", 1, -1, 0, 0);
                                }
                                break;
                            case 0:
                                {
                                    _RightAnimationController.StartDefenseAnimation("hurt");
                                    _fatal = true;
                                    CalcDmg(false, _technicalAttModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnTechnique);
                                    _battleResultText.color = _healthRecoveryColor;
                                    _battleResult = "Technique Attack penetrated Enemy's Guard!";
                                    //SetHitStatus("CLEAN HIT", "GUARD FAIL", "", "");
                                    SetHitStatus("Clean Hit", "", "", "", 1, 0, 0, 0);
                                }
                                break;
                            case 1:
                                {
                                    if (_friendCtrl != null && !_isAssisting)
                                    {
                                        if (_friendCtrl._FA_Offensive_Condition == ChrController.FA_Offensive_Conditions.Critical_Hit)
                                        {
                                            _friendAssist = true;
                                        }
                                    }
                                    _bufferOnPlayerSide = true;
                                    Invoke("CalcBuffer", 2.0f);
                                    _RightAnimationController.StartDefenseAnimation("hurt");
                                    _fatal = true;
                                    CalcDmg(false, _technicalAttModifier + _criticalAttackModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnTechnique);
                                    _battleResultText.color = _healthRecoveryColor;
                                    _battleResult = "Technique Attack completely penetrated Enemy's Guard!";
                                    //SetHitStatus("CRITICAL HIT", "GUARD FAIL", "", "");
                                    SetHitStatus("Critical Hit", "", "", "", 1, -1, 0, 0);
                                    EventManager.Instance.InsertDialogue(true, _playerCtrl._portraitCombat, _playerCtrl._dialogue_critical, false);
                                    _playerDmg = -(int)(_enemyDmg * Random.Range(0.3f, 0.7f));
                                }
                                break;
                        }
                    }
                    else
                    {
                        _RightAnimationController.StartAttackAnimation("att technique");
                        switch (outcome)
                        {
                            case -1:
                                {
                                    _bufferOnPlayerSide = false;
                                    Invoke("CalcBuffer", 2.0f);
                                    _leftAnimationController.StartDefenseAnimation("hurt");
                                    _fatal = true;
                                    CalcDmg(false, _technicalAttModifier + _criticalAttackModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnHurt);
                                    _battleResultText.color = _healthDmgColor;
                                    _battleResult = "Guard completely broken by Enemy's Technique Attack!";
                                    //SetHitStatus("GUARD FAIL", "CRITICAL HIT", "", "");
                                    SetHitStatus("", "Critcal Hit", "", "", -1, 1, 0, 0);
                                    EventManager.Instance.InsertDialogue(false, _enemyCtrl._portraitCombat, _enemyCtrl._dialogue_critical, false);
                                    _enemyDmg = -(int)(_playerDmg * Random.Range(0.3f, 0.7f));
                                }
                                break;
                            case 0:
                                {
                                    _leftAnimationController.StartDefenseAnimation("hurt");
                                    _fatal = true;
                                    CalcDmg(false, _technicalAttModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnHurt);
                                    _battleResultText.color = _healthDmgColor;
                                    _battleResult = "Guard broken by Enemy's Technique Attack!!";
                                    //SetHitStatus("GUARD FAIL", "CLEAN HIT", "", "");
                                    SetHitStatus("", "Clean Hit", "", "", 0, 1, 0, 0);
                                }
                                break;
                            case 1:
                                {
                                    _leftAnimationController.StartDefenseAnimation("hurt");
                                    _fatal = true;
                                    CalcDmg(false, _weakGuardModifier);
                                    _battleResultText.color = _neutralColor;
                                    _battleResult = "Barely blocked Enemy's Technique Attack!";
                                    //SetHitStatus("WEAK GUARD", "BLOCKED HIT", "", "");
                                    SetHitStatus("Weak Guard", "Hit", "", "", -1, 1, 0, 0);
                                }
                                break;
                        }
                    }
                }
                break;
            case 6: // Technique on Counter
                {
                    if (_battleType == BattleTypes.Offensive)
                    {
                        if (_canCounter)
                        {
                            switch (outcome)
                            {
                                case -1:
                                    {
                                        _bufferOnPlayerSide = false;
                                        Invoke("CalcBuffer", 2.0f);
                                        _fatal = true;
                                        _leftAnimationController.StartAttackAnimation("hurt");
                                        _RightAnimationController.StartDefenseAnimation("att precision");
                                        CalcDmg(true, _counterModifier + _criticalAttackModifier);
                                        GameManager.Instance.IncreaseMomentum(_momentumOnHurt);
                                        _battleResult = "Overwhelmed by Enemy Counter Attack!";
                                        _battleResultText.color = _healthDmgColor;
                                        //SetHitStatus("ATTACK FAIL", "COUNTER", "", "CRITICAL HIT");
                                        SetHitStatus("", "Counter", "", "Critical Hit", 0, 1, -1, 1);
                                        EventManager.Instance.InsertDialogue(false, _enemyCtrl._portraitCombat, _enemyCtrl._dialogue_perfectCounter, false);
                                    }
                                    break;
                                case 0:
                                    {
                                        _fatal = true;
                                        _leftAnimationController.StartAttackAnimation("hurt");
                                        _RightAnimationController.StartDefenseAnimation("att precision");
                                        CalcDmg(true, _counterModifier);
                                        GameManager.Instance.IncreaseMomentum(_momentumOnHurt);
                                        _battleResult = "Hit by Enemy Counter Attack!";
                                        _battleResultText.color = _healthDmgColor;
                                        //SetHitStatus("ATTACK FAIL", "COUNTER", "", "CLEAN HIT");
                                        SetHitStatus("", "Counter", "", "Clean Hit", 0, 1, 0, 1);
                                    }
                                    break;
                                case 1:
                                    {
                                        _fatal = false;
                                        _leftAnimationController.StartAttackAnimation("dodge");
                                        _RightAnimationController.StartDefenseAnimation("att precision");
                                        CalcDmg(true, _grazingModifier);
                                        //GameManager.Instance.IncreaseMomentum(_momentumOnPower / 2);
                                        _battleResult = "Barely grazed by Enemy's Counter Attack!";
                                        _battleResultText.color = _healthRecoveryColor;
                                        //SetHitStatus("ATTACK FAIL", "COUNTER", "ALMOST DODGE", "GRAZED HIT");
                                        SetHitStatus("", "Counter", "", "Weak Hit", 0, 1, 0, 0);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            _fatal = false;
                            _playerDmg = 0;
                            _enemyDmg = 0;
                            _leftAnimationController.StartAttackAnimation("att technique");
                            _RightAnimationController.StartDefenseAnimation("dodge");
                            GameManager.Instance.IncreaseMomentum(_momentumOnMissed);
                            _battleResult = "Enemy cannot Counter, but Dodged Technique Attack!";
                            _battleResultText.color = _healthDmgColor;
                            //SetHitStatus("MISS", "DODGE", "", "");
                            SetHitStatus("", "Dodge", "", "", 0, 1, 0, 0);
                        }
                    }
                    else // battle type defensive
                    {
                        if (_friendCtrl != null && !_isAssisting)
                        {
                            if (_friendCtrl._FA_Defensive_Condition == ChrController.FA_Defensive_Conditions.Counter_Success)
                            {
                                _friendAssist = true;
                            }
                        }
                        if (_canCounter)
                        {
                            switch (outcome)
                            {
                                case -1:
                                    {
                                        _fatal = false;
                                        _RightAnimationController.StartAttackAnimation("dodge");
                                        _leftAnimationController.StartDefenseAnimation("att precision");
                                        CalcDmg(true, _grazingModifier);
                                        //GameManager.Instance.IncreaseMomentum(_momentumOnPower / 2);
                                        _battleResult = "Counter Attack barely grazed Enemy!";
                                        _battleResultText.color = _neutralColor;
                                        //SetHitStatus("COUNTER", "ATTACK FAIL", "GRAZED HIT", "ALMOST DODGE");
                                        SetHitStatus("Counter", "", "Weak Hit", "", 1, 0, 0, 0);
                                    }
                                    break;
                                case 0:
                                    {
                                        _fatal = true;
                                        _RightAnimationController.StartAttackAnimation("hurt");
                                        _leftAnimationController.StartDefenseAnimation("att precision");
                                        CalcDmg(true, _counterModifier);
                                        GameManager.Instance.IncreaseMomentum(_momentumOnCounter);
                                        _battleResult = "Counter Attack hit Enemy!";
                                        _battleResultText.color = _healthRecoveryColor;
                                        //SetHitStatus("COUNTER", "ATTACK FAIL", "CLEAN HIT", "");
                                        SetHitStatus("Counter", "", "Clean Hit", "", 1, 0, 1, 0);
                                    }
                                    break;
                                case 1:
                                    {
                                        _bufferOnPlayerSide = true;
                                        Invoke("CalcBuffer", 2.0f);
                                        if (_friendCtrl != null && !_isAssisting)
                                        {
                                            if (_friendCtrl._FA_Defensive_Condition == ChrController.FA_Defensive_Conditions.Perfect_Action)
                                            {
                                                _friendAssist = true;
                                            }
                                        }
                                        _fatal = true;
                                        _RightAnimationController.StartAttackAnimation("hurt");
                                        _leftAnimationController.StartDefenseAnimation("att precision");
                                        CalcDmg(true, _counterModifier + _criticalAttackModifier);
                                        GameManager.Instance.IncreaseMomentum(_momentumOnCounter);
                                        _battleResult = "Counter Attack overwhelmed Enemy!";
                                        _battleResultText.color = _healthRecoveryColor;
                                        //SetHitStatus("COUNTER", "ATTACK FAIL", "CRITICAL HIT", "");
                                        SetHitStatus("Counter", "", "Critical Hit", "", 1, 0, 1, -1);
                                        EventManager.Instance.InsertDialogue(true, _playerCtrl._portraitCombat, _playerCtrl._dialogue_perfectCounter, false);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            _fatal = false;
                            _playerDmg = 0;
                            _enemyDmg = 0;
                            _RightAnimationController.StartAttackAnimation("att technique");
                            _leftAnimationController.StartDefenseAnimation("dodge");
                            GameManager.Instance.IncreaseMomentum(_momentumOnDodge);
                            _battleResult = "Cannot Counter, but Dodged Technique Attack!";
                            _battleResultText.color = _healthRecoveryColor;
                            //SetHitStatus("DODGE", "MISS", "", "");
                            SetHitStatus("Dodge", "", "", "", 1, 0, 0, 0);
                        }
                    }
                }
                break;
            case 7: // Precision on Dodge
                {
                    if (_battleType == BattleTypes.Offensive)
                    {
                        _leftAnimationController.StartAttackAnimation("att precision");
                        if (_friendCtrl != null && !_isAssisting)
                        {
                            if (_friendCtrl._FA_Offensive_Condition == ChrController.FA_Offensive_Conditions.Precision_Hit)
                            {
                                _friendAssist = true;
                            }
                        }
                        switch (outcome)
                        {
                            case -1:
                            case 0:
                                {
                                    _RightAnimationController.StartDefenseAnimation("hurt");
                                    _fatal = true;
                                    CalcDmg(false, _precisionAttModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnPrecision);
                                    _battleResult = "Precision Attack hit dodging Enemy!";
                                    _battleResultText.color = _healthRecoveryColor;
                                    //SetHitStatus("CLEAN HIT", "DODGE FAIL", "", "");
                                    SetHitStatus("Clean Hit", "", "", "", 1, 0, 0, 0);
                                }
                                break;
                            case 1:
                                {
                                    _bufferOnPlayerSide = true;
                                    Invoke("CalcBuffer", 2.0f);
                                    if (_friendCtrl != null && !_isAssisting)
                                    {
                                        if (_friendCtrl._FA_Offensive_Condition == ChrController.FA_Offensive_Conditions.Critical_Hit)
                                        {
                                            _friendAssist = true;
                                        }
                                    }
                                    _RightAnimationController.StartDefenseAnimation("hurt");
                                    _fatal = true;
                                    CalcDmg(false, _precisionAttModifier + _criticalAttackModifier);
                                    GameManager.Instance.IncreaseMomentum((int)(_momentumOnPrecision * 1.1f));
                                    _battleResult = "Precision Attack overwhelmed dodging Enemy!";
                                    _battleResultText.color = _healthRecoveryColor;
                                    //SetHitStatus("CRITICAL HIT", "DODGE FAIL", "", "");
                                    SetHitStatus("Critical Hit", "", "", "", 1, 0, 0, 0);
                                    EventManager.Instance.InsertDialogue(true, _playerCtrl._portraitCombat, _playerCtrl._dialogue_critical, false);
                                }
                                break;
                        }
                    }
                    else
                    {
                        _RightAnimationController.StartAttackAnimation("att precision");
                        switch (outcome)
                        {
                            case -1:
                                {
                                    _bufferOnPlayerSide = false;
                                    Invoke("CalcBuffer", 2.0f);
                                    _leftAnimationController.StartDefenseAnimation("hurt");
                                    _fatal = true;
                                    CalcDmg(false, _precisionAttModifier + _criticalAttackModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnHurt);
                                    _battleResult = "Overwhelmed by Enemy's Precision Attack!";
                                    _battleResultText.color = _healthDmgColor;
                                    //SetHitStatus("DODGE FAIL", "CRITICAL HIT", "", "");
                                    SetHitStatus("", "Critical Hit", "", "", -1, 1, 0, 0);
                                    EventManager.Instance.InsertDialogue(false, _enemyCtrl._portraitCombat, _enemyCtrl._dialogue_critical, false);
                                }
                                break;
                            case 0:
                            case 1:
                                {
                                    _leftAnimationController.StartDefenseAnimation("hurt");
                                    _fatal = true;
                                    CalcDmg(false, _precisionAttModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnHurt);
                                    _battleResult = "Hit by Enemy's Precision Attack!";
                                    _battleResultText.color = _healthDmgColor;
                                    //SetHitStatus("DODGE FAIL", "CLEAN HIT", "", "");
                                    SetHitStatus("", "Clean Hit", "", "", 0, 1, 0, 0);
                                }
                                break;
                                //{
                                //    _leftAnimationController.StartDefenseAnimation("dodge");
                                //    _fatal = false;
                                //    CalcDmg(false, _grazingModifier);
                                //    //GameManager.Instance.IncreaseMomentum(_momentumOnPower / 2);
                                //    _battleResult = "Barely grazed by Enemy's Precision Attack!";
                                //    _battleResultText.color = _healthRecoveryColor;
                                //    //SetHitStatus("ALMOST DODGE", "GRAZED HIT", "", "");
                                //    SetHitStatus("", "GRAZED HIT", "", "");
                                //}
                                //break;
                        }
                    }
                }
                break;
            case 8: // Precision on Guard
                {
                    if (_battleType == BattleTypes.Offensive)
                    {
                        _fatal = false;
                        _leftAnimationController.StartAttackAnimation("att precision");
                        _RightAnimationController.StartDefenseAnimation("guard");
                        switch (outcome)
                        {
                            case -1:
                                {
                                    _bufferOnPlayerSide = false;
                                    Invoke("CalcBuffer", 2.0f);
                                    //CalcDmg(false, _goodGuardModifier);
                                    _enemyDmg = 1;
                                    GameManager.Instance.IncreaseMomentum(_momentumOnBlocked);
                                    _battleResult = "Attack barely penetrates Enemy's Guard!";
                                    _battleResultText.color = _healthDmgColor;
                                    //SetHitStatus("BLOCKED HIT", "PERFECT GUARD", "", "");
                                    SetHitStatus("", "Full Guard", "", "", 0, 1, 0, 0);
                                    EventManager.Instance.InsertDialogue(false, _enemyCtrl._portraitCombat, _enemyCtrl._dialogue_perfectGuard, false);
                                }
                                break;
                            case 0:
                            case 1:
                                {
                                    CalcDmg(false, _normalGuardModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnBlocked);
                                    _battleResultText.color = _neutralColor;
                                    _battleResult = "Attack blocked by Enemy's Guard!";
                                    //SetHitStatus("BLOCKED HIT", "GUARD", "", "");
                                    SetHitStatus("", "Guard", "", "", 0, 1, 0, 0);
                                }
                                break;
                        }
                    }
                    else
                    {
                        _fatal = false;
                        _leftAnimationController.StartAttackAnimation("guard");
                        _RightAnimationController.StartDefenseAnimation("att precision");
                        if (_friendCtrl != null && !_isAssisting)
                        {
                            if (_friendCtrl._FA_Defensive_Condition == ChrController.FA_Defensive_Conditions.Block_Success)
                            {
                                _friendAssist = true;
                            }
                        }
                        switch (outcome)
                        {
                            case -1:
                            case 0:
                                {
                                    CalcDmg(false, _normalGuardModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnBlock);
                                    _battleResult = "Blocked Enemy's Precision Attack!";
                                    _battleResultText.color = _neutralColor;
                                    //SetHitStatus("GUARD", "BLOCKED HIT", "", "");
                                    SetHitStatus("Guard", "", "", "", 1, 0, 0, 0);
                                }
                                break;
                            case 1:
                                {
                                    _bufferOnPlayerSide = true;
                                    Invoke("CalcBuffer", 2.0f);
                                    if (_friendCtrl != null && !_isAssisting)
                                    {
                                        if (_friendCtrl._FA_Defensive_Condition == ChrController.FA_Defensive_Conditions.Perfect_Action)
                                        {
                                            _friendAssist = true;
                                        }
                                    }
                                    //CalcDmg(false, _goodGuardModifier);
                                    _playerDmg = 1;
                                    GameManager.Instance.IncreaseMomentum(_momentumOnBlock);
                                    _battleResult = "Completely Blocked Enemy's Attack!";
                                    _battleResultText.color = _healthRecoveryColor;
                                    //SetHitStatus("PERFECT GUARD", "BLOCKED HIT", "", "");
                                    SetHitStatus("Full Guard", "", "", "", 1, 0, 0, 0);
                                    EventManager.Instance.InsertDialogue(true, _playerCtrl._portraitCombat, _playerCtrl._dialogue_perfectGuard, false);
                                }
                                break;
                        }
                    }
                }
                break;
            case 9: // Precision on Counter
                {
                    if (_battleType == BattleTypes.Offensive)
                    {
                        switch (outcome)
                        {
                            case -1:
                                {
                                    if (_canCounter)
                                    {
                                        _fatal = true;
                                        _leftAnimationController.StartAttackAnimation("hurt");
                                        _RightAnimationController.StartDefenseAnimation("att precision");
                                        CalcDmg(true, _counterModifier);
                                        GameManager.Instance.IncreaseMomentum(_momentumOnHurt);
                                        _battleResult = "Precision Attack Countered by Enemy!";
                                        _battleResultText.color = _healthDmgColor;
                                        //SetHitStatus("ATTACK FAIL", "COUNTER", "", "CLEAN HIT");
                                        SetHitStatus("", "Counter", "", "Clean Hit", 0, 1, 0, 1);
                                    }
                                    else
                                    {
                                        _leftAnimationController.StartAttackAnimation("att precision");
                                        _RightAnimationController.StartDefenseAnimation("guard");
                                        _fatal = false;
                                        CalcDmg(false, _goodGuardModifier);
                                        GameManager.Instance.IncreaseMomentum(_momentumOnBlocked);
                                        _battleResult = "Enemy cannot counter, but guarded against Attack!";
                                        _battleResultText.color = _healthDmgColor;
                                        //SetHitStatus("BLOCKED HIT", "STRONG GUARD", "", "");
                                        SetHitStatus("", "Guard", "", "", 0, 1, 0, 0);
                                    }
                                }
                                break;
                            case 0:
                            case 1:
                                {
                                    if (_friendCtrl != null && !_isAssisting)
                                    {
                                        if (_friendCtrl._FA_Offensive_Condition == ChrController.FA_Offensive_Conditions.Precision_Hit)
                                        {
                                            _friendAssist = true;
                                        }
                                    }
                                    _fatal = true;
                                    _leftAnimationController.StartAttackAnimation("att precision");
                                    _RightAnimationController.StartDefenseAnimation("hurt");
                                    CalcDmg(false, _precisionAttModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnPrecision);
                                    _battleResult = "Enemy's Counter failed, Precision Attack hit!";
                                    _battleResultText.color = _healthRecoveryColor;
                                    //SetHitStatus("CLEAN HIT", "COUNTER FAIL", "", "");
                                    SetHitStatus("Clean Hit", "", "", "", 1, 0, 0, 0);
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (outcome)
                        {
                            case -1:
                            case 0:
                                {
                                    _fatal = true;
                                    _RightAnimationController.StartAttackAnimation("att precision");
                                    _leftAnimationController.StartDefenseAnimation("hurt");
                                    CalcDmg(false, _precisionAttModifier);
                                    GameManager.Instance.IncreaseMomentum(_momentumOnHurt);
                                    _battleResult = "Counter Attack failed!";
                                    _battleResultText.color = _healthDmgColor;
                                    //SetHitStatus("COUNTER FAIL", "CLEAN HIT", "", "");
                                    SetHitStatus("", "Clean Hit", "", "", 0, 1, 0, 0);
                                }
                                break;
                            case 1:
                                {
                                    if (_friendCtrl != null && !_isAssisting)
                                    {
                                        if (_friendCtrl._FA_Defensive_Condition == ChrController.FA_Defensive_Conditions.Counter_Success)
                                        {
                                            _friendAssist = true;
                                        }
                                    }
                                    if (_canCounter)
                                    {
                                        _fatal = true;
                                        _RightAnimationController.StartAttackAnimation("hurt");
                                        _leftAnimationController.StartDefenseAnimation("att precision");
                                        CalcDmg(true, _counterModifier);
                                        GameManager.Instance.IncreaseMomentum(_momentumOnPrecision);
                                        _battleResult = "Countered Enemy's Precision Attack!";
                                        _battleResultText.color = _healthRecoveryColor;
                                        //SetHitStatus("COUNTER", "ATTACK FAIL", "CLEAN HIT", "");
                                        SetHitStatus("Counter", "", "Clean Hit", "", 1, 0, 1, 0);
                                    }
                                    else
                                    {
                                        _RightAnimationController.StartAttackAnimation("att precision");
                                        _leftAnimationController.StartDefenseAnimation("guard");
                                        _fatal = false;
                                        CalcDmg(false, _goodGuardModifier);
                                        GameManager.Instance.IncreaseMomentum(_momentumOnBlock);
                                        _battleResult = "Cannot counter, but guarded against Attack!";
                                        _battleResultText.color = _healthRecoveryColor;
                                        //SetHitStatus("STRONG GUARD", "BLOCKED HIT", "", "");
                                        SetHitStatus("Guard", "", "", "", 1, 0, 0, 0);
                                    }
                                }
                                break;
                        }
                    }
                }
                break;
        }
    }

    /*
     * 0 - 0.5: bad outcome, 0.5 - 0.8: neutral: 0.8 > : good outcome 
     * */
    private void SingleBattleOutcomeDecider()
    {
        switch (_triad)
        {
            case -1:
                _battleScore = 0.2f;
                break;
            case 0:
                _battleScore = 0.55f;
                break;
            case 1:
                _battleScore = 0.7f;
                break;
        }

        if (_locationSuccess)
            _battleScore += Random.Range(0.1f, 0.3f);
        else
            _battleScore += Random.Range(-0.3f, 0.0f);

        if (_timingSuccess)
            _battleScore += Random.Range(0.0f, 0.2f);
        else
            _battleScore += Random.Range(-0.15f, 0.0f);

        _battleScore += Random.Range(0.0f, ((float)_playerCtrl._Agility * 0.01f));
        _battleScore -= Random.Range(0.0f, ((float)_enemyCtrl._Agility * 0.01f));

        if (_enemyCtrl._HP <= 10)
            _battleScore += 0.2f;
    }

    private void CalcDmg(bool counter, float modifier)
    {
        float baseDmg = 0;

        float momentumModifer = (float)(GameManager.Instance._momentum - 50) * 0.02f;

        if (_battleType == BattleTypes.Offensive)
        {
            if (counter)
            {
                baseDmg = Mathf.Clamp((_enemyCtrl._Attack * (Random.Range(3.0f, 3.5f) - momentumModifer) - _playerCtrl._Defense * Random.Range(1.5f, 2.2f)), 1.0f, 999.0f);
                _playerDmg = (int)(baseDmg * modifier);
            }
            else
            {
                baseDmg = Mathf.Clamp((_playerCtrl._Attack * (Random.Range(3.0f, 3.5f) + momentumModifer) - _enemyCtrl._Defense * Random.Range(1.5f, 2.2f)), 1.0f, 999.0f);
                _enemyDmg = (int)(baseDmg * modifier);
            }
        }
        else
        {
            if (counter)
            {
                baseDmg = Mathf.Clamp((_playerCtrl._Attack * (Random.Range(3.0f, 3.5f) + momentumModifer) - _enemyCtrl._Defense * Random.Range(1.7f, 2.5f)), 1.0f, 999.0f);
                _enemyDmg = (int)(baseDmg * modifier);
            }
            else
            {
                baseDmg = Mathf.Clamp((_enemyCtrl._Attack * (Random.Range(3.0f, 3.5f) - momentumModifer) - _playerCtrl._Defense * Random.Range(1.7f, 2.5f)), 1.0f, 999.0f);
                _playerDmg = (int)(baseDmg * modifier);
            }
        }
        if (_easyMode)
        {
            if (_playerDmg > 50 && Random.Range(0, 10) > 6)
                _playerDmg = (int)(_playerDmg * Random.Range(0.7f, 0.85f));
            if (_enemyDmg > 1 && _enemyDmg < 50)
                _enemyDmg += Random.Range(5, 11);
        }
        else
        {
            if (_playerDmg > 50 && Random.Range(0, 10) > 6)
                _playerDmg = (int)(_playerDmg * Random.Range(0.7f, 0.85f));
            if (_enemyDmg > 50 && Random.Range(0, 10) > 6)
                _enemyDmg = (int)(_enemyDmg * Random.Range(0.7f, 0.85f));
            if (_playerDmg > 1 && _playerDmg < 50)
                _playerDmg += Random.Range(5, 11);
        }

        if (_superEasyMode && _enemyDmg > 1)
        {
            _fatal = true;
            _enemyDmg = 999;
        }

        if (_superEasyMode && _playerDmg > 10)
        {
            _playerDmg = 10;
        }
    }

    public void CalcAndDisplaySpeed()
    {
        int attackMove = 0;
        int defenderMove = 0;
        ChrController.AttackTypes attackerWeaponType;
        ChrController.AttackTypes defenderWeaponType;

        if (_battleType == BattleTypes.Offensive)
        {
            attackMove = _decisionValue + 2;
            defenderMove = 4 - _enemyMove;
            attackerWeaponType = _playerCtrl._attackType;
            defenderWeaponType = _enemyCtrl._attackType;
        }
        else
        {
            attackMove = 4 - _enemyMove;
            defenderMove = _decisionValue + 2;
            attackerWeaponType = _enemyCtrl._attackType;
            defenderWeaponType = _playerCtrl._attackType;
        }

        switch (attackerWeaponType)
        {
            case ChrController.AttackTypes.Melee:
                {
                    _attackerSpeedHigh = 1;
                    _attackerSpeedLow = 3;
                }
                break;
            case ChrController.AttackTypes.Spell:
                {
                    _attackerSpeedHigh = 3;
                    _attackerSpeedLow = 4;
                }
                break;
            case ChrController.AttackTypes.Pistol:
                {
                    _attackerSpeedHigh = 1;
                    _attackerSpeedLow = 2;
                }
                break;
            case ChrController.AttackTypes.Range:
                {
                    _attackerSpeedHigh = 3;
                    _attackerSpeedLow = 5;
                }
                break;
            case ChrController.AttackTypes.LongRange:
                {
                    _attackerSpeedHigh = 4;
                    _attackerSpeedLow = 5;
                }
                break;
            case ChrController.AttackTypes.Bow:
                {
                    _attackerSpeedHigh = 2;
                    _attackerSpeedLow = 5;
                }
                break;
        }

        switch (attackMove)
        {
            case 3: //Power
                {
                    _attackerSpeedHigh += _powerHighSpeed;
                    _attackerSpeedLow += _powerLowSpeed;
                }
                break;
            case 2: //Technique
                {
                    _attackerSpeedHigh += _techniqueHighSpeed;
                    _attackerSpeedLow += _techniqueLowSpeed;
                }
                break;
            case 1: // Precision
                {
                    _attackerSpeedHigh += _precisionHighSpeed;
                    _attackerSpeedLow += _precisionLowSpeed;
                }
                break;
        }

        switch (defenderMove)
        {
            case 3: //Guard
                {
                    _defenderSpeedHigh = _guardHighSpeed * 2;
                    _defenderSpeedLow = _guardLowSpeed * 2;
                }
                break;
            case 2: //Dodge
                {
                    _defenderSpeedHigh = _dodgeHighSpeed * 2;
                    _defenderSpeedLow = _dodgeLowSpeed * 2;
                }
                break;
            case 1: //Counter
                {

                    _defenderSpeedHigh = _counterHighSpeed;
                    _defenderSpeedLow = _counterLowSpeed;
                    switch (defenderWeaponType)
                    {
                        case ChrController.AttackTypes.Melee:
                            {
                                _defenderSpeedHigh += 1;
                                _defenderSpeedLow += 3;
                            }
                            break;
                        case ChrController.AttackTypes.Spell:
                            {
                                _defenderSpeedHigh += 3;
                                _defenderSpeedLow += 4;
                            }
                            break;
                        case ChrController.AttackTypes.Pistol:
                            {
                                _defenderSpeedHigh += 1;
                                _defenderSpeedLow += 2;
                            }
                            break;
                        case ChrController.AttackTypes.Range:
                            {
                                _defenderSpeedHigh += 3;
                                _defenderSpeedLow += 5;
                            }
                            break;
                        case ChrController.AttackTypes.LongRange:
                            {
                                _defenderSpeedHigh += 4;
                                _defenderSpeedLow += 5;
                            }
                            break;
                        case ChrController.AttackTypes.Bow:
                            {
                                _defenderSpeedHigh += 2;
                                _defenderSpeedLow += 5;
                            }
                            break;
                    }
                }
                break;
        }
        if (_battleType == BattleTypes.Offensive)
        {
            if (_enemyTiming)
            {
                _timingSpeedText.text = "Speed: " + _attackerSpeedHigh + "~" + _attackerSpeedLow;
                _enemyTimingSpeedText.text = "Speed: " + _defenderSpeedHigh + "~" + _defenderSpeedLow;
            }
        }
        else
        {

            if (_enemyTiming)
            {
                _timingSpeedText.text = "Speed: " + _defenderSpeedHigh + "~" + _defenderSpeedLow;
                _enemyTimingSpeedText.text = "Speed: " + _attackerSpeedHigh + "~" + _attackerSpeedLow;
            }
        }
    }

    private void CalcDistance()
    {
        _distance = (int)(Mathf.Abs(_playerCtrl._position.x - _enemyCtrl._position.x) + Mathf.Abs(_playerCtrl._position.y - _enemyCtrl._position.y));
        if (_battleType == BattleTypes.Offensive)
        {
            if (_distance == 1)
            {
                if (_enemyCtrl._canCounterNear)
                    _canCounter = true;
                else
                    _canCounter = false;
            }
            else if (_distance == 2)
            {
                if (_enemyCtrl._canCounterMid)
                    _canCounter = true;
                else
                    _canCounter = false;
            }
            else
            {
                if (_enemyCtrl._canCounterLong)
                    _canCounter = true;
                else
                    _canCounter = false;
            }
        }
        else if (_battleType == BattleTypes.Defensive)
        {
            if (_distance == 1)
            {
                if (_playerCtrl._canCounterNear)
                    _canCounter = true;
                else
                    _canCounter = false;
            }
            else if (_distance == 2)
            {
                if (_playerCtrl._canCounterMid)
                    _canCounter = true;
                else
                    _canCounter = false;
            }
            else
            {
                if (_playerCtrl._canCounterLong)
                    _canCounter = true;
                else
                    _canCounter = false;
            }
        }
        DisplayCounterInfo();
        //Invoke("DisplayCounterInfo", 0.1f);
    }

    public void CalcBuffer()
    {
        if (!_isAssisting)
        {
            int value = 0;
            //value = _locationSuccess ? 2 : 1;
            value = 2;
            if (_bufferOnPlayerSide)
            {
                if (_battleType == BattleTypes.Offensive)
                    value = -value;
                switch (_locationDecisionValue)
                {
                    case -1:
                        {
                            if (_battleType == BattleTypes.Offensive)
                            {
                                _enemyCtrl.BuffApply(0, 0, value);
                                _battleInfo1_enemy.color = _healthDmgColor;
                                _battleInfo1_enemy.text = "Speed " + value;
                            }
                            else if (_battleType == BattleTypes.Defensive)
                            {
                                _playerCtrl.BuffApply(0, 0, value);
                                _battleInfo1_player.color = _healthRecoveryColor;
                                _battleInfo1_player.text = "Speed +" + value;
                            }
                        }
                        break;
                    case 0:
                        {
                            if (_battleType == BattleTypes.Offensive)
                            {
                                _enemyCtrl.BuffApply(0, value, 0);
                                _battleInfo1_enemy.color = _healthDmgColor;
                                _battleInfo1_enemy.text = "Protect " + value;
                            }
                            else if (_battleType == BattleTypes.Defensive)
                            {
                                _playerCtrl.BuffApply(0, value, 0);
                                _battleInfo1_player.color = _healthRecoveryColor;
                                _battleInfo1_player.text = "Protect +" + value;
                            }
                        }
                        break;
                    case 1:
                        {
                            if (_battleType == BattleTypes.Offensive)
                            {
                                _enemyCtrl.BuffApply(value, 0, 0);
                                _battleInfo1_enemy.color = _healthDmgColor;
                                _battleInfo1_enemy.text = "Brave " + value;
                            }
                            else if (_battleType == BattleTypes.Defensive)
                            {
                                _playerCtrl.BuffApply(value, 0, 0);
                                _battleInfo1_player.color = _healthRecoveryColor;
                                _battleInfo1_player.text = "Brave +" + value;
                            }
                        }
                        break;
                }
            }
            else
            {
                if (_battleType == BattleTypes.Defensive)
                    value = -value;
                switch (_enemyLocation)
                {
                    case -1:
                        {
                            if (_battleType == BattleTypes.Offensive)
                            {
                                _enemyCtrl.BuffApply(0, 0, value);
                                _battleInfo1_enemy.color = _healthRecoveryColor;
                                _battleInfo1_enemy.text = "Speed +" + value;
                            }
                            else if (_battleType == BattleTypes.Defensive)
                            {
                                _playerCtrl.BuffApply(0, 0, value);
                                _battleInfo1_player.color = _healthDmgColor;
                                _battleInfo1_player.text = "Speed " + value;
                            }
                        }
                        break;
                    case 0:
                        {
                            if (_battleType == BattleTypes.Offensive)
                            {
                                _enemyCtrl.BuffApply(0, value, 0);
                                _battleInfo1_enemy.color = _healthRecoveryColor;
                                _battleInfo1_enemy.text = "Protect +" + value;
                            }
                            else if (_battleType == BattleTypes.Defensive)
                            {
                                _playerCtrl.BuffApply(0, value, 0);
                                _battleInfo1_player.color = _healthDmgColor;
                                _battleInfo1_player.text = "Protect " + value;
                            }
                        }
                        break;
                    case 1:
                        {
                            if (_battleType == BattleTypes.Offensive)
                            {
                                _enemyCtrl.BuffApply(value, 0, 0);
                                _battleInfo1_enemy.color = _healthRecoveryColor;
                                _battleInfo1_enemy.text = "Brave +" + value;
                            }
                            else if (_battleType == BattleTypes.Defensive)
                            {
                                _playerCtrl.BuffApply(value, 0, 0);
                                _battleInfo1_player.color = _healthDmgColor;
                                _battleInfo1_player.text = "Brave " + value;
                            }
                        }
                        break;
                }
            }
        }
    }

    #endregion

    #region UI

    public void DisplayFinalDecision()
    {
        if (!_isInstantDeath)
            _BattleDecisionsVisual.SetActive(true);
        _enemyBattleTellVisual1.gameObject.SetActive(false);
        _enemyBattleTellVisual2.gameObject.SetActive(false);
        _playerBattleTellVisual.gameObject.SetActive(false);
        _friendText.text = "";
        _friendText.gameObject.transform.parent.gameObject.SetActive(false);
        _timingSpeedText.text = "";
        _enemyTimingSpeedText.text = "";
        if (_decisionValue >= 1)
        {
            _playerDecisionName = _battleType == BattleTypes.Offensive ? "POWER" : "GUARD";
            if (_BattleDecisionsVisualCtrl != null)
                _BattleDecisionsVisualCtrl.ShowBattleResult(_battleType == BattleTypes.Offensive ? 1 : 4, _battleType == BattleTypes.Offensive ? _enemyMove + 3 : _enemyMove);
        }
        else if (_decisionValue <= -1)
        {
            _playerDecisionName = _battleType == BattleTypes.Offensive ? "PRECISION" : "COUNTER";
            if (_BattleDecisionsVisualCtrl != null)
                _BattleDecisionsVisualCtrl.ShowBattleResult(_battleType == BattleTypes.Offensive ? 3 : 6, _battleType == BattleTypes.Offensive ? _enemyMove + 3 : _enemyMove);
        }
        else
        {
            _playerDecisionName = _battleType == BattleTypes.Offensive ? "TECHNIQUE" : "DODGE";
            if (_BattleDecisionsVisualCtrl != null)
                _BattleDecisionsVisualCtrl.ShowBattleResult(_battleType == BattleTypes.Offensive ? 2 : 5, _battleType == BattleTypes.Offensive ? _enemyMove + 3 : _enemyMove);
        }

        //_playerDecisionName += " " + _decisionValue;
        //if (_battleType == BattleTypes.Offensive)
        //if (!_isAssisting && !_isInstantDeath)
        //    _decisionText.text = _playerDecisionName + " vs. " + _enemyMoveName;

        _decisionText.text = "";

        //else
        //  _decisionText.text = _enemyMoveName + " vs. " + _playerDecisionName;

        if (_debug)
        {
            if (_battleScoreText != null)
                _battleScoreText.text = _battleScore.ToString("0.00");

            if (_triadStatus != null)
            {
                switch (_triad)
                {
                    case -1:
                        {
                            _triadStatus.text = "TRIAD FAILED";
                            _triadStatus.color = _healthDmgColor;
                        }
                        break;
                    case 0:
                        {
                            _triadStatus.text = "TRIAD NEUTRAL";
                            //_battleScoreText.color = _neutralColor;
                            _triadStatus.color = _neutralColor;
                        }
                        break;
                    case 1:
                        {
                            _triadStatus.text = "TRIAD SUCCESS";
                            //_battleScoreText.color = _healthRecoveryColor;
                            _triadStatus.color = _healthRecoveryColor;
                        }
                        break;
                }
            }

            if (_locationStatus != null)
            {
                if (_locationSuccess)
                {
                    _locationStatus.text = "TARGETING SUCCESS";
                    _locationStatus.color = _healthRecoveryColor;
                }
                else
                {
                    _locationStatus.text = "TARGETING FAILED";
                    _locationStatus.color = _healthDmgColor;
                }
            }
            if (_timingStatus != null)
            {
                if (_timingSuccess)
                {
                    if (_battleType == BattleTypes.Offensive)
                        _timingStatus.text = "TIMING SUCCESS " + _attackerSpeed.ToString("0.00") + "/" + _defenderSpeed.ToString("0.00");
                    else if (_battleType == BattleTypes.Defensive)
                        _timingStatus.text = "TIMING SUCCESS " + _defenderSpeed.ToString("0.00") + "/" + _attackerSpeed.ToString("0.00");
                    _timingStatus.color = _healthRecoveryColor;
                }
                else
                {
                    if (_battleType == BattleTypes.Offensive)
                        _timingStatus.text = "TIMING FAILED " + _attackerSpeed.ToString("0.00") + "/" + _defenderSpeed.ToString("0.00");
                    else if (_battleType == BattleTypes.Defensive)
                        _timingStatus.text = "TIMING FAILED " + _defenderSpeed.ToString("0.00") + "/" + _attackerSpeed.ToString("0.00");
                    _timingStatus.color = _healthDmgColor;
                }
            }
        }
    }

    public void DisplayPlayerActionName()
    {
        switch (_decisionValue)
        {
            case 1: // Power or Guard
                {
                    switch (_locationDecisionValue)
                    {
                        case 1:
                            {
                                if (_battleType == BattleTypes.Offensive)
                                    _decisionText.text = "Action:\nHIGH POWER ATTACK";
                                else if (_battleType == BattleTypes.Defensive)
                                    _decisionText.text = "Action:\nHIGH GUARD";
                            }
                            break;
                        case -1:
                            {
                                if (_battleType == BattleTypes.Offensive)
                                    _decisionText.text = "Action:\nLOW POWER ATTACK";
                                else if (_battleType == BattleTypes.Defensive)
                                    _decisionText.text = "Action:\nLOW GUARD";
                            }
                            break;
                        case 0:
                            {
                                if (_battleType == BattleTypes.Offensive)
                                    _decisionText.text = "Action:\nMID POWER ATTACK";
                                else if (_battleType == BattleTypes.Defensive)
                                    _decisionText.text = "Action:\nMID GUARD";
                            }
                            break;
                    }
                }
                break;
            case -1: // Precision or Counter
                {
                    switch (_locationDecisionValue)
                    {
                        case 1:
                            {
                                if (_battleType == BattleTypes.Offensive)
                                    _decisionText.text = "Action:\nHIGH PRECISION ATTACK";
                                else if (_battleType == BattleTypes.Defensive)
                                    _decisionText.text = "Action:\nCOUNTER HIGH ATTACK";
                            }
                            break;
                        case -1:
                            {
                                if (_battleType == BattleTypes.Offensive)
                                    _decisionText.text = "Action:\nLOW PRECISION ATTACK";
                                else if (_battleType == BattleTypes.Defensive)
                                    _decisionText.text = "Action:\nCOUNTER LOW ATTACK";
                            }
                            break;
                        case 0:
                            {
                                if (_battleType == BattleTypes.Offensive)
                                    _decisionText.text = "Action:\nMID PRECISION ATTACK";
                                else if (_battleType == BattleTypes.Defensive)
                                    _decisionText.text = "Action:\nCOUNTER MID ATTACK";
                            }
                            break;
                    }
                }
                break;
            case 0: // Technique or Dodge
                {
                    switch (_locationDecisionValue)
                    {
                        case 1:
                            {
                                if (_battleType == BattleTypes.Offensive)
                                    _decisionText.text = "Action:\nHIGH TECHNIQUE ATTACK";
                                else if (_battleType == BattleTypes.Defensive)
                                    _decisionText.text = "Action:\nDODGE HIGH ATTACK";
                            }
                            break;
                        case -1:
                            {
                                if (_battleType == BattleTypes.Offensive)
                                    _decisionText.text = "Action:\nLOW TECHNIQUE ATTACK";
                                else if (_battleType == BattleTypes.Defensive)
                                    _decisionText.text = "Action:\nDODGE LOW ATTACK";
                            }
                            break;
                        case 0:
                            {
                                if (_battleType == BattleTypes.Offensive)
                                    _decisionText.text = "Action:\nMID TECHNIQUE ATTACK";
                                else if (_battleType == BattleTypes.Defensive)
                                    _decisionText.text = "Action:\nDODGE MID ATTACK";
                            }
                            break;
                    }
                }
                break;
        }
    }

    public void setBGSize()
    {
        var sr = _BGImage.GetComponent<SpriteRenderer>();
        //barscale = _decisionBar.transform.localScale;
        if (sr != null)
        {
            var width = sr.sprite.bounds.size.x;
            var height = sr.sprite.bounds.size.y;
            var _cameraHeight = 3 * 2;
            var _cameraWidth = _cameraHeight * ((float)Screen.width / (float)Screen.height);
            _BGImage.transform.localScale = new Vector3(_cameraWidth / width, (_cameraHeight * 1.2f) / height);
        }
    }

    private void SetHitStatus(string left, string right, string leftCounter, string rightCounter, int leftValue, int rightValue, int leftCounterValue, int rightCounterValue)
    {
        if (_battleInfo1_player != null)
        {
            switch (leftValue)
            {
                case -1:
                    {
                        _battleInfo1_player.color = _healthDmgColor;
                    }
                    break;
                case 0:
                    {
                        _battleInfo1_player.color = Color.black;
                    }
                    break;
                case 1:
                    {
                        _battleInfo1_player.color = _healthRecoveryColor;
                    }
                    break;
            }
            _battleInfo1_player.text = left;
        }
        if (_battleInfo2_player != null)
        {
            switch (leftCounterValue)
            {
                case -1:
                    {
                        _battleInfo2_player.color = _healthDmgColor;
                    }
                    break;
                case 0:
                    {
                        _battleInfo2_player.color = Color.black;
                    }
                    break;
                case 1:
                    {
                        _battleInfo2_player.color = _healthRecoveryColor;
                    }
                    break;
            }
            _battleInfo2_player.text = leftCounter;
        }
        if (_battleInfo1_enemy != null)
        {
            switch (rightValue)
            {
                case -1:
                    {
                        _battleInfo1_enemy.color = _healthDmgColor;
                    }
                    break;
                case 0:
                    {
                        _battleInfo1_enemy.color = Color.black;
                    }
                    break;
                case 1:
                    {
                        _battleInfo1_enemy.color = _healthRecoveryColor;
                    }
                    break;
            }
            _battleInfo1_enemy.text = right;
        }
        if (_battleInfo2_enemy != null)
        {
            switch (rightCounterValue)
            {
                case -1:
                    {
                        _battleInfo2_enemy.color = _healthDmgColor;
                    }
                    break;
                case 0:
                    {
                        _battleInfo2_enemy.color = Color.black;
                    }
                    break;
                case 1:
                    {
                        _battleInfo2_enemy.color = _healthRecoveryColor;
                    }
                    break;
            }
            _battleInfo2_enemy.text = rightCounter;
        }
    }

    public void DisplayHPChange()
    {
        if (_battleInfo2_player != null && _playerDmg != 0)
        {
            if (_playerDmg > 0)
            {
                _battleInfo2_player.color = _healthDmgColor;
                _battleInfo2_player.text = "-" + _playerDmg + " HP";
            }
            else
            {
                _battleInfo2_player.color = _healthRecoveryColor;
                _battleInfo2_player.text = "+" + -_playerDmg + " HP";
            }
        }

        if (_battleInfo2_enemy != null && _enemyDmg != 0)
        {
            if (_enemyDmg > 0)
            {
                _battleInfo2_enemy.color = _healthDmgColor;
                _battleInfo2_enemy.text = "-" + _enemyDmg + " HP";
            }
            else
            {
                _battleInfo2_enemy.color = _healthRecoveryColor;
                _battleInfo2_enemy.text = "+" + -_enemyDmg + " HP";
            }
        }
        _battleInfo2_enemy.gameObject.SetActive(true);
        _battleInfo2_enemy.gameObject.SetActive(true);
    }

    public void DisplayFriendText()
    {
        if (_friends.Count > 1 && _curFriend < _friends.Count)
        {
            _friendText.gameObject.transform.parent.gameObject.SetActive(true);
            //_friendText.gameObject.SetActive(true);
            _friendText.text = "Friend (" + _curFriend + "/" + (_friends.Count - 1) + "): " + _friends[_curFriend]._name;
        }
    }

    public void DisplayTerrianInfo()
    {
        Cell curCell = GameManager.Instance.GetCell((int)_playerCtrl._position.x, (int)_playerCtrl._position.y);
        if (curCell != null)
        {
            //_playerTerrian.gameObject.SetActive(true);
            switch (curCell._terrian)
            {
                case Cell.Terrians.Forest:
                    {
                        _battleInfo1_player.color = _healthRecoveryColor;
                        _battleInfo1_player.text = "+20% Agl";
                    }
                    break;
                case Cell.Terrians.Fort:
                    {
                        _battleInfo1_player.color = _healthRecoveryColor;
                        _battleInfo1_player.text = "+30% Def";
                    }
                    break;
                case Cell.Terrians.AttackingSpot:
                    {
                        _battleInfo1_player.color = _healthRecoveryColor;
                        _battleInfo1_player.text = "+20% Atk";
                    }
                    break;
                case Cell.Terrians.Danger:
                    {
                        _battleInfo1_player.color = _healthDmgColor;
                        _battleInfo1_player.text = "-10% All Stats";
                    }
                    break;
                case Cell.Terrians.Walkable:
                    {
                        _battleInfo1_player.color = Color.black;
                        _battleInfo1_player.text = "Neutral";
                    }
                    break;
            }
        }

        curCell = GameManager.Instance.GetCell((int)_enemyCtrl._position.x, (int)_enemyCtrl._position.y);
        if (curCell != null)
        {
            //_enemyTerrian.gameObject.SetActive(true);
            switch (curCell._terrian)
            {
                case Cell.Terrians.Forest:
                    {
                        _battleInfo1_enemy.color = _healthRecoveryColor;
                        _battleInfo1_enemy.text = "+20% Agl";
                    }
                    break;
                case Cell.Terrians.Fort:
                    {
                        _battleInfo1_enemy.color = _healthRecoveryColor;
                        _battleInfo1_enemy.text = "+30% Def";
                    }
                    break;
                case Cell.Terrians.AttackingSpot:
                    {
                        _battleInfo1_enemy.color = _healthRecoveryColor;
                        _battleInfo1_enemy.text = "+20% Atk";
                    }
                    break;
                case Cell.Terrians.Danger:
                    {
                        _battleInfo1_enemy.color = _healthDmgColor;
                        _battleInfo1_enemy.text = "-10% All Stats";
                    }
                    break;
                case Cell.Terrians.Walkable:
                    {
                        _battleInfo1_enemy.color = Color.black;
                        _battleInfo1_enemy.text = "Neutral";
                    }
                    break;
            }
        }
    }

    public void DisplayCounterInfo()
    {
        switch (_battleType)
        {
            case BattleTypes.Offensive:
                {
                    _battleInfo2_player.color = Color.black;
                    switch (_distance)
                    {
                        case 1:
                            {
                                _battleInfo2_player.text = "Melee";
                            }
                            break;
                        case 2:
                            {
                                _battleInfo2_player.text = "Range";
                            }
                            break;
                        default:
                            {
                                _battleInfo2_player.text = "Long Range";
                            }
                            break;
                    }
                    if (_canCounter)
                        _battleInfo2_enemy.color = _healthRecoveryColor;
                    else
                        _battleInfo2_enemy.color = _healthDmgColor;
                    // _distanceTextEnemy.text = "Distance: " + _distance + "\nCounter: " + _canCounter;
                    _battleInfo2_enemy.text = _canCounter ? "Can Counter" : "Cannot Counter";
                }
                break;
            case BattleTypes.Defensive:
                {
                    _battleInfo2_enemy.color = Color.black;
                    switch (_distance)
                    {
                        case 1:
                            {
                                _battleInfo2_enemy.text = "Melee";
                            }
                            break;
                        case 2:
                            {
                                _battleInfo2_enemy.text = "Range";
                            }
                            break;
                        default:
                            {
                                _battleInfo2_enemy.text = "Long Range";
                            }
                            break;
                    }
                    if (_canCounter)
                        _battleInfo2_player.color = _healthRecoveryColor;
                    else
                        _battleInfo2_player.color = _healthDmgColor;
                    //_distanceText.text = "Distance: " + _distance + "\nCounter: " + _canCounter;
                    _battleInfo2_player.text = _canCounter ? "Can Counter" : "Cannot Counter";
                }
                break;
        }
    }

    public void UICleanUp_FA()
    {
        //SetHitStatus("", "", "", "");
        _battleResultText.text = "";
        _triadStatus.text = "";
        _locationStatus.text = "";
        _timingStatus.text = "";
        _battleScoreText.text = "";

        //_playerTerrian.gameObject.SetActive(false);
        //_enemyTerrian.gameObject.SetActive(false);
        _BattleDecisionsVisual.SetActive(false);
        //_enemyBattleTellVisual1.gameObject.SetActive(false);
        //_enemyBattleTellVisual2.gameObject.SetActive(false);
    }

    #endregion
}
