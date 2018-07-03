using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChrController : MonoBehaviour
{
    #region Variables

    #region Public Stats

    [Header("Basics")]
    public string _name;
    public string _nickName;

    public Sprite _portrait;
    public Sprite _portraitCombat;
    public Sprite _portraitHurt;
    public Sprite _portraitFriend;
    public Sprite _icon;
    public GameObject _combatBody;
    public GameObject HpBarObject;
    public GameObject HpBar;
    public Text HpNum;
    public GameObject _HPBarInactiveUnit;

    public enum UnitTypes
    {
        Hero_Main,
        Hero_Unique,
        Hero_Generic,
        Enemy_Boss,
        Enemy_Melee,
        Enemy_Range,
        Enemy_SubBoss,
    }
    public UnitTypes _unitType = UnitTypes.Hero_Main;
    [Space(20)]

    [Header("Icons")]
    public GameObject _currentIcon;

    // enemy AI icons
    public GameObject _AIGuardIcon;
    public GameObject _AIApproachIcon;
    public GameObject _AIRandomIcon;
    public GameObject _AIPatrolIcon;
    public GameObject _AIRetreatIcon;
    //public GameObject _AIKillMainIcon;
    [Space(20)]

    [Header("Stats")]

    [HideInInspector]
    public int _MovementRange = 3;
    [Range(1, 5)]
    public int _BaseMovementRange = 3;
    public float _moveSpeed = 3.5f;
    [Range(0.1f, 0.5f)]
    public float _gridSnapDistance = 0.1f;
    [Range(1, 3)]
    public int intelligence = 2;

    [Range(50, 300)]
    public int _maxHP = 100;
    [HideInInspector]
    public int _Attack = 10; // max 50
    [HideInInspector]
    public int _Defense = 8; // max 50
    [HideInInspector]
    public int _Agility = 5; // max 50
    [Range(5, 50)]
    public int _BaseAttack = 10; // max 50
    [Range(5, 50)]
    public int _BaseDefense = 8; // max 50
    [Range(5, 50)]
    public int _BaseAgility = 5; // max 50

    [Range(0.0f, 0.5f)]
    public float _MeleeDuelChance = 0f;
    [Range(0.0f, 0.5f)]
    public float _RangeDuelChance = 0f;

    public bool _canCounterNear = false;
    public bool _canCounterMid = false;
    public bool _canCounterLong = false;
    [Space(20)]

    [Header("SE")]
    public AudioClip WalkSE;
    public AudioClip AttackSE;
    public AudioClip DodgeSE;
    public AudioClip BlockSE;
    public AudioClip HurtSE;
    public AudioClip HitSE;
    public AudioClip DefeatSE;
    [Space(20)]
    //public enum CounterTypes
    //{
    //    Melee,
    //    Range,
    //    Both,
    //}
    //public CounterTypes _counterType = CounterTypes.Melee;

    //public enum OffensiveSkills
    //{
    //    None,
    //    High_Status_Chance, // increase chance of inflict status effects on enemy
    //    High_Power, // clean landed power attacks do more dmg
    //    Steady_Hands, // clean landed precison attacks increase more momentum
    //}
    //public OffensiveSkills _offensiveSkill = OffensiveSkills.None;

    //public enum SupportSkills
    //{
    //    None,
    //    High_Boost_Chance, // increase chance of receive status boost effects during defensive skills
    //    High_Recovery, // increase health recovery amount on clean landed technical attack
    //}
    //public SupportSkills _defensiveSkill = SupportSkills.None;

    //public enum SurvivalSkills
    //{
    //    None,
    //    Second_Chance, // may survive fatal hit (1hp left) if health > 1
    //    Desperation_Power, // auto-cast Brave on self when health < 30%
    //    Desperation_Defense, // auto-cast Protect on self when health < 30%
    //    Desperation_Speed, // auto-cast Speed on self when health < 30%
    //}
    //public SurvivalSkills _survivalSkill = SurvivalSkills.None;

    [Header("Status Effects")]
    [Range(-3, 3)]
    public int _Brave = 0; // increase Attack, negative value means Weaken
    [Range(-3, 3)]
    public int _Protect = 0; // increase Defense, negative value means Wound
    [Range(-3, 3)]
    public int _Speed = 0; // increase Agility and movement range, negative value means Slow
    //private int _BraveQueue = 0;
    //private int _ProtectQueue = 0;
    //private int _SpeedQueue = 0;

    public enum AttackTypes
    {
        Melee,
        Spell,
        Pistol,
        Range,
        LongRange,
        Bow,
    }
    public AttackTypes _attackType = AttackTypes.Melee;

    [Space(20)]
    #endregion

    #region AI related
    [Header("AI")]
    public int _grouping;
    public enum Personalities : int
    {
        RiskTaking, // likes Power Attack, dodge
        Conservative, // likes precision, block
        Eccentric, // random taking actions
        Showy, // likes technique, counter
    };
    public Personalities _personality = Personalities.Eccentric;

    public enum AIPatterns : int
    {
        Guard, // stay at spawn pt, attack enemies to come in range
        Random, // move around randomly
        Approach, // try to approach a certain target
        Patrol,
        Retreat,
    };
    public AIPatterns _AIPattern = AIPatterns.Random;
    public Vector2 _patrolPt;

    [Space(20)]
    #endregion

    #region Dialogue

    [Header("Dialogue")]
    public string _dialogue_death = "(I'm dead)";
    public string _dialogue_1HP = "(not yet... can't die yet...)";
    public string _dialogue_hurt100 = "(Minor injury... not a issue...)";
    public string _dialogue_hurt80 = "(I'm should becareful...)";
    public string _dialogue_hurt50 = "(I'm not looking so good...)";
    public string _dialogue_hurt20 = "(I'm dying...)";

    public string _dialogue_critical = "(Critical Strike!)";
    public string _dialogue_perfectDodge = "(Perfect dodge!)";
    public string _dialogue_perfectCounter = "(Perfect counter!)";
    public string _dialogue_perfectGuard = "(Perfect guard!)";
    public string _dialogue_instantKill = "(Instant Kill!)";
    //public string _dialogue_countered = "(my attack countered!?)";
    //public string _dialogue_recoverHealth = "(Recovered some health)";
    [Space(20)]

    [Header("Friend Assist")]
    public string _dialogue_FA_offensive = "(let me help on offense)";
    public string _dialogue_FA_defensive = "(let me help on defense)";
    public string _dialogue_FA_Move = "(I'm going assist you!)";
    public enum FA_Offensive_Conditions : int
    {
        Critical_Hit, // assist when friend makes critical
        Technical_Hit,
        Precision_Hit,
        Power_Hit,
        Random,
    };
    public FA_Offensive_Conditions _FA_Offensive_Condition = FA_Offensive_Conditions.Critical_Hit;
    public enum FA_Defensive_Conditions : int
    {
        Perfect_Action,
        Dodge_Success,
        Block_Success,
        Counter_Success,
        Random
    };
    public FA_Defensive_Conditions _FA_Defensive_Condition = FA_Defensive_Conditions.Perfect_Action;
    public enum FA_Moves : int
    {
        Power = 1,
        Precision = -1,
        Technique = 0,
    }
    public FA_Moves _FA_Move = FA_Moves.Power;
    [Space(20)]

    [Header("Combat Preferences")]
    public string _dialogue_favAttack = "(Power Attack is my best move!)";
    public string _dialogue_favDefense = "(Counter is my specialty!)";
    public enum Fav_Offensives : int
    {
        Power = 1,
        Precision = -1,
        Technical = 0,
        Random = 2,
    };
    public Fav_Offensives _Fav_Offensive = Fav_Offensives.Power;

    public enum Fav_Defenses : int
    {
        Guard = 1,
        Counter = -1,
        Dodge = 0,
        Random = 2,
    };
    public Fav_Defenses _Fav_Defense = Fav_Defenses.Guard;

    public Sprite _Fav_Offense_Icon;
    public Sprite _Fav_Defense_Icon;

    [Space(20)]
    #endregion

    #region Private Stats
    [HideInInspector]
    public int _HP;
    [HideInInspector]
    public int _HP_Display;
    private bool _showHPChange = false;
    // positive status changes
    [HideInInspector]
    public bool _isProtected = false; // increase physical def
    [HideInInspector]
    public bool _isShelled = false; // increase magical def
    [HideInInspector]
    public bool _isStrengthened = false; // increase phy dmg
    [HideInInspector]
    public bool _isFocused = false; // increase magical dmg
    // negative status changes
    [HideInInspector]
    public bool _isFreezing = false;  // movement reduced
    [HideInInspector]
    public bool _isPoisoned = false; // dec stamina recovery rate
    [HideInInspector]
    public bool _isBleeding = false; // lose health
    [HideInInspector]
    public bool _isDazed = false; // can move, but can't take action
    [HideInInspector]
    public bool _isBurning = false; // lose health, lower def
    [HideInInspector]
    public bool _isExhausted = false; // lose turn, can't fight back, take double dmg
    #endregion

    #region Turn and Movement
    //[HideInInspector]
    [Header("Position and Turn")]
    public Vector2 _position;
    public Vector2 _prevPosition;
    //[HideInInspector]
    //  player's turn rank in a round, lower number is earlier, >= 200 means player cannot move this turn
    public int _turn = 0;
    [Space(20)]

    [HideInInspector]
    public bool _isSelected;
    [HideInInspector]
    public bool _isMoving;
    private List<Vector2> _moveList = new List<Vector2>();
    [HideInInspector]
    public bool _hasMoved = false;
    #endregion

    #region Chr States
    [HideInInspector]
    public bool _isAI = true;
    [HideInInspector]
    public bool _isDead = false;
    #endregion

    #region Components
    private Animator _anim;
    private SkeletonAnimation _animation;
    private Spine.AnimationState spineAnimationState;
    #endregion

    #endregion

    #region Core GameFlow

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _HP = _maxHP;
        _HP_Display = _maxHP;
    }

    void Start()
    {
        //if (gameObject.tag == "Player")
        //{
        //    _animation = GetComponentInChildren<SkeletonAnimation>();
        //    spineAnimationState = _animation.AnimationState;
        //}
        _animation = GetComponentInChildren<SkeletonAnimation>();
        spineAnimationState = _animation.AnimationState;
        SetPosition();
        updateHPInterface();
        if (MissionManager.Instance._enemyPatrolDest.Count > 0)
            if (_unitType == UnitTypes.Enemy_Boss)
                _patrolPt = MissionManager.Instance._enemyPatrolDest[0];
            else
                _patrolPt = MissionManager.Instance._enemyPatrolDest[Random.Range(0, MissionManager.Instance._enemyPatrolDest.Count)];
        _Attack = _BaseAttack;
        _MovementRange = _MovementRange = _BaseMovementRange;
        _Defense = _BaseDefense;
        _Agility = _BaseAgility;

        InvokeRepeating("HealthChange_Smooth", 2.0f, 0.05f);
    }

    void Update()
    {
        MoveToGrid();
    }

    #endregion

    #region Functions

    IEnumerator DoWalk()
    {
        //if (gameObject.tag == "Player")
        //{
        //    spineAnimationState.SetAnimation(0, "walk", true);
        //    yield return new WaitForSeconds(1.5f);
        //}
        spineAnimationState.SetAnimation(0, "walk", true);
        yield return new WaitForSeconds(1.5f);

    }

    IEnumerator DoIdel()
    {
        //if (gameObject.tag == "Player")
        //{
        //    spineAnimationState.SetAnimation(0, "idle", true);
        //    yield return new WaitForSeconds(1.5f);
        //}
        spineAnimationState.SetAnimation(0, "idle", true);
        yield return new WaitForSeconds(1.5f);

    }

    //change character rotation
    //true face right flase face left
    public void Flip(bool faceRight)
    {
        if (faceRight)
        {
            gameObject.transform.localScale = new Vector3(-Mathf.Abs(gameObject.transform.localScale.x), gameObject.transform.localScale.y, gameObject.transform.localScale.z);
            HpBarObject.transform.localScale = new Vector3(-Mathf.Abs(HpBarObject.transform.localScale.x), HpBarObject.transform.localScale.y, HpBarObject.transform.localScale.z);
        }
        else
        {
            gameObject.transform.localScale = new Vector3(Mathf.Abs(gameObject.transform.localScale.x), gameObject.transform.localScale.y, gameObject.transform.localScale.z);
            HpBarObject.transform.localScale = new Vector3(Mathf.Abs(HpBarObject.transform.localScale.x), HpBarObject.transform.localScale.y, HpBarObject.transform.localScale.z);
        }
    }

    void updateHPInterface()
    {
        float HpPercent = (float)_HP / (float)_maxHP;
        HpBar.transform.localScale = new Vector3(HpPercent, HpBar.transform.localScale.y, HpBar.transform.localScale.z);
        HpNum.text = _HP.ToString() + "/" + _maxHP.ToString();
    }

    public void MoveToPosition(List<Vector2> moveList)
    {
        if (moveList.Count > 0)
            AStarPathfinding.Instance.GenerateTargetIcon(moveList[moveList.Count - 1]);
        _isMoving = true;
        _moveList = moveList;
        //CameraManager.Instance.FollowTargetByVector(gameObject.transform.position);
        CameraManager.Instance.FollowTarget(gameObject);
    }

    public void MoveToGrid()
    {
        if (_isMoving)
        {
            Vector2 curPos = gameObject.transform.position;

            if (Vector3.Magnitude(_moveList[0] - curPos) < _gridSnapDistance)
            {
                if (WalkSE != null)
                {
                    SoundManager.Instance.PlaySE(true, WalkSE);
                }
                curPos = _moveList[0];
                _moveList.RemoveAt(0);
                //GameManager.Instance.CheckIfSpawnCrystal((int)curPos.x, (int)curPos.y);

                bool player = gameObject.tag == "Player";
                // Check for location trigger
                MissionManager.Instance.CheckTriggerEvent((int)curPos.x, (int)curPos.y, player ? 1 : 2);
            }
            else
            {
                curPos = Vector3.Lerp(curPos, _moveList[0], Time.deltaTime * _moveSpeed);
            }
            transform.position = curPos;
            if (_moveList.Count == 0)
            {
                _hasMoved = true;
                _isMoving = false;
                TurnFinished();

                //Menu condition
                //if (gameObject.tag == "Enemy")
                //    TurnFinished();
            }

            //Menu condition
            //if (gameObject.tag == "Player")
            //{
            //    MenuController.Instance.MoveAction = false;
            //    MenuController.Instance.AfterMoveAction = true;
            //}

        }
    }

    public void UnitSelected()
    {
        if (_turn < 200)
        {
            _prevPosition = _position;
            _hasMoved = false;
            _isSelected = true;
            SetCurrentSelectionStatus();
            if (gameObject.tag == "Enemy")
            {
                CameraManager.Instance.FollowTarget(gameObject);
            }
            AStarPathfinding.Instance.GenerateAStarCells(_attackType, _position, _MovementRange, gameObject.tag == "Player");

            //Menu condition
            //if (gameObject.tag == "Enemy")
            //{
            //    AStarPathfinding.Instance.GenerateAStarCells(_position, _MovementRange, gameObject.tag == "Player");
            //}
            //if (gameObject.tag == "Player")
            //{
            //    MenuController.Instance.ShowSelectionList = true;
            //}
        }
    }

    //to check enemy movement and moved hero's movement
    public void UnitCheckMovement()
    {
        AStarPathfinding.Instance.GenerateAStarCells(_attackType, _position, _MovementRange, gameObject.tag == "Player");
    }

    //public void showarea()
    //{
    //    AStarPathfinding.Instance.GenerateAStarCells(_position, _MovementRange, gameObject.tag == "Player");
    //}

    public void UnitDeSelected()
    {
        _isSelected = false;
        SetCurrentSelectionStatus();
        if (gameObject.tag != "Enemy")
        {
            CameraManager.Instance.UnfollowTarget();
        }
        AStarPathfinding.Instance.ClearPathCells();
    }

    public bool SoftDeselectUnit()
    {
        bool ret = false;

        if (_hasMoved)
        {
            CancelMove();
        }
        else
        {
            ret = true;
        }
        return ret;
    }

    void SetCurrentSelectionStatus()
    {
        if (_currentIcon != null && gameObject.tag == "Player")
        {
            if (_isSelected && !_currentIcon.activeInHierarchy)
            {
                _currentIcon.SetActive(true);
            }
            else if (!_isSelected && _currentIcon.activeInHierarchy)
            {
                _currentIcon.SetActive(false);
            }
        }
    }

    public void TurnFinished()
    {
        _turn += 200;
        //BuffApply();
        if (_anim != null)
        {
            _anim.SetBool("hasTurn", false);
        }

        if (_HPBarInactiveUnit != null)
            _HPBarInactiveUnit.SetActive(true);

        if (spineAnimationState != null)
        {
            StartCoroutine(DoIdel());
        }
        bool player = gameObject.tag == "Player";

        SetPosition();
        if (_prevPosition != _position)
        {
            GameManager.Instance.ChangeCellCost((int)_prevPosition.x, (int)_prevPosition.y, player ? 2 : 1, -10);
            GameManager.Instance.ChangeCellCost((int)_position.x, (int)_position.y, player ? 1 : 2, 10);
            _prevPosition = _position;
        }
        AdjustStats();

        CheckForCrystal();

        GameManager.Instance.SpillBlood((int)_position.x, (int)_position.y, Random.Range(1, 6), false);
        //if (Random.Range(0, 10) > 7)
        //    GameManager.Instance._crystalSpawnLimited = true;
        //PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.ChrSelection;
        GameManager.Instance.UnitTurnFinished(player ? 1 : 2);
    }

    public void NewTurnStarted()
    {
        if (_isDead)
        {
            return;
        }
        _turn = 10;
        _prevPosition = _position;
        buffRecover();
        if (_anim != null)
        {
            _anim.SetBool("hasTurn", true);
        }

        if (spineAnimationState != null)
        {
            StartCoroutine(DoWalk());
        }

        if (_HPBarInactiveUnit != null)
            _HPBarInactiveUnit.SetActive(false);
    }

    public void SetPosition()
    {
        _position = gameObject.transform.position;
    }

    public void CancelMove()
    {
        _position = _prevPosition;
        transform.position = _position;
        _hasMoved = false;
    }

    public int TakeDamage(int amount, bool fatal)
    {
        bool survive = _unitType == UnitTypes.Hero_Main || _unitType == UnitTypes.Hero_Unique || _unitType == UnitTypes.Enemy_Boss;
        if (survive)
            survive = _HP > 50 && Random.Range(0, 10) > 6;

        _HP = Mathf.Clamp(_HP - amount, 0, _maxHP);

        if (_HP <= 0)
        {
            if (fatal && !survive)
            {
                GameManager.Instance.SpillBlood((int)_position.x, (int)_position.y, amount + 50, true);
                _HP = 0;
                EventManager.Instance.InsertDialogue(gameObject.tag == "Player", _portraitHurt, _dialogue_death, false);
                if (gameObject.tag == "Player")
                {
                    FlashManager.Instance.StartLeft_Delay();
                }
                else
                {
                    FlashManager.Instance.StartRight_Dealy();
                }
            }
            else
            {
                GameManager.Instance.SpillBlood((int)_position.x, (int)_position.y, amount + 20, true);
                _HP = 1;
                EventManager.Instance.InsertDialogue(gameObject.tag == "Player", _portraitHurt, _dialogue_1HP, false);
            }
        }
        else
        {
            if (amount > 1)
            {
                //if (_HP == 0)
                //{
                //    EventManager.Instance.InsertDialogue(gameObject.tag == "Player", _portraitHurt, _dialogue_death, false);
                //}
                //else if (_HP == 1)
                //{
                //    EventManager.Instance.InsertDialogue(gameObject.tag == "Player", _portraitHurt, _dialogue_1HP, false);
                //}
                if (HP_Percent() < 0.2f)
                {
                    EventManager.Instance.InsertDialogue(gameObject.tag == "Player", _portraitHurt, _dialogue_hurt20, false);
                }
                else if (HP_Percent() < 0.5f)
                {
                    EventManager.Instance.InsertDialogue(gameObject.tag == "Player", _portraitHurt, _dialogue_hurt50, false);
                }
                else if (HP_Percent() < 0.8f)
                {
                    EventManager.Instance.InsertDialogue(gameObject.tag == "Player", _portraitCombat, _dialogue_hurt80, false);
                }
                else
                {
                    EventManager.Instance.InsertDialogue(gameObject.tag == "Player", _portraitCombat, _dialogue_hurt100, false);
                }
                GameManager.Instance.SpillBlood((int)_position.x, (int)_position.y, amount, amount > 20);
            }
        }

        updateHPInterface();
        Invoke("StartHPChange", 1.0f);

        return _HP;
    }

    public void RecoverHealth(int amount)
    {
        _HP = Mathf.Clamp(_HP + amount, 0, _maxHP);
        updateHPInterface();
    }

    public void CheckDeath()
    {
        if (_HP <= 0)
        {
            _isDead = true;
            Death();
        }
    }

    public void Death()
    {
        //_isDead = true;
        //Destroy(gameObject, 2.0f);
        _turn = 301;
        bool player = gameObject.tag == "Player";
        if (player)
        {
            GameManager.Instance.ChangeCellCost((int)_position.x, (int)_position.y, 2, -10);
            MissionManager.Instance.HeroDied();
            SoundManager.Instance.PlayerDefeatSound();
        }
        else
        {
            GameManager.Instance.ChangeCellCost((int)_position.x, (int)_position.y, 1, -10);
            MissionManager.Instance.EnemyDied();
            SoundManager.Instance.EnemyDefeatSound();
        }
        Invoke("Disappear", 0.8f);

    }

    private void Disappear()
    {
        gameObject.SetActive(false);
    }

    public float HP_Percent()
    {
        float ret = (float)_HP / (float)_maxHP;
        return ret;
    }

    /*
        0: Guard, // stay at spawn pt, attack enemies to come in range
        1: Random, // move around randomly
        2: Approach, // try to approach a certain target
        3: Patrol,
        4: Retreat,
        */
    public void ChangeAIPattern(int pattern)
    {
        if (pattern < System.Enum.GetNames(typeof(ChrController.AIPatterns)).Length)
        {
            _AIPattern = (ChrController.AIPatterns)pattern;
        }

        if (_AIGuardIcon != null && _AIPatrolIcon != null && _AIRandomIcon != null && _AIRetreatIcon != null && _AIApproachIcon != null)
        {
            switch (_AIPattern)
            {
                case AIPatterns.Approach:
                    {
                        _AIGuardIcon.SetActive(false);
                        _AIPatrolIcon.SetActive(false);
                        _AIRandomIcon.SetActive(false);
                        _AIRetreatIcon.SetActive(false);
                        _AIApproachIcon.SetActive(true);
                        //_AIKillMainIcon.SetActive(false);
                    }
                    break;
                case AIPatterns.Guard:
                    {
                        _AIGuardIcon.SetActive(true);
                        _AIPatrolIcon.SetActive(false);
                        _AIRandomIcon.SetActive(false);
                        _AIRetreatIcon.SetActive(false);
                        _AIApproachIcon.SetActive(false);
                        //_AIKillMainIcon.SetActive(false);
                    }
                    break;
                case AIPatterns.Patrol:
                    {
                        _AIGuardIcon.SetActive(false);
                        _AIPatrolIcon.SetActive(true);
                        _AIRandomIcon.SetActive(false);
                        _AIRetreatIcon.SetActive(false);
                        _AIApproachIcon.SetActive(false);
                        //_AIKillMainIcon.SetActive(false);
                    }
                    break;
                case AIPatterns.Random:
                    {
                        _AIGuardIcon.SetActive(false);
                        _AIPatrolIcon.SetActive(false);
                        _AIRandomIcon.SetActive(true);
                        _AIRetreatIcon.SetActive(false);
                        _AIApproachIcon.SetActive(false);
                        //_AIKillMainIcon.SetActive(false);
                    }
                    break;
                case AIPatterns.Retreat:
                    {
                        _AIGuardIcon.SetActive(false);
                        _AIPatrolIcon.SetActive(false);
                        _AIRandomIcon.SetActive(false);
                        _AIRetreatIcon.SetActive(true);
                        _AIApproachIcon.SetActive(false);
                        //_AIKillMainIcon.SetActive(false);
                    }
                    break;
                    //case AIPatterns.Kill_Main:
                    //    {
                    //        _AIGuardIcon.SetActive(false);
                    //        _AIPatrolIcon.SetActive(false);
                    //        _AIRandomIcon.SetActive(false);
                    //        _AIRetreatIcon.SetActive(false);
                    //        _AIApproachIcon.SetActive(false);
                    //        //_AIKillMainIcon.SetActive(true);
                    //    }
                    //    break;
            }
        }
    }

    public void Buffer(string buffName, int buffnum)
    {
        switch (buffName)
        {
            case "Atk":
                if (_Brave > -3 && _Brave < 3)
                {
                    _Brave += buffnum;
                }
                break;
            case "Def":
                if (_Protect > -3 && _Protect < 3)
                {
                    _Protect += buffnum;
                }
                break;
            case "Agl":
                if (_Speed > -3 && _Speed < 3)
                {
                    _Speed += buffnum;
                }
                break;
        }
    }

    void buffRecover()
    {
        if (_Brave < 0)
        {
            _Brave++;
        }
        if (_Brave > 0)
        {
            _Brave--;
        }
        if (_Protect < 0)
        {
            _Protect++;
        }
        if (_Protect > 0)
        {
            _Protect--;
        }
        if (_Speed < 0)
        {
            _Speed++;
        }
        if (_Speed > 0)
        {
            _Speed--;
        }

        AdjustStats();
    }

    //public void BuffQueue(int brave, int protect, int speed)
    //{
    //    _BraveQueue += brave;
    //    _ProtectQueue += protect;
    //    _SpeedQueue += speed;
    //}

    public void BuffApply(int brave, int protect, int speed)
    {
        _Brave = Mathf.Clamp(_Brave + brave, -3, 3);
        _Protect = Mathf.Clamp(_Protect + protect, -3, 3);
        _Speed = Mathf.Clamp(_Speed + speed, -3, 3);

        AdjustStats();
    }

    public void AdjustStats()
    {
        float attackModifier = 1.0f;
        float defenseModifier = 1.0f;
        float agilityModifier = 1.0f;

        if (_Brave > 0)
            attackModifier += 0.1f;
        else if (_Brave < 0)
            attackModifier -= 0.1f;

        if (_Protect > 0)
            defenseModifier += 0.2f;
        else if (_Protect < 0)
            defenseModifier -= 0.2f;

        if (_Speed > 0)
        {
            agilityModifier += 0.1f;
            _MovementRange = _BaseMovementRange + 1;
        }
        else if (_Speed < 0)
        {
            agilityModifier -= 0.1f;
            _MovementRange = _BaseMovementRange - 1;
        }
        else
        {
            _MovementRange = _BaseMovementRange;
        }

        Cell curCell = GameManager.Instance.GetCell((int)_position.x, (int)_position.y);
        if (curCell != null)
        {
            switch (curCell._terrian)
            {
                case Cell.Terrians.Forest:
                    {
                        agilityModifier += 0.2f;
                    }
                    break;
                case Cell.Terrians.Fort:
                    {
                        defenseModifier += 0.3f;
                    }
                    break;
                case Cell.Terrians.AttackingSpot:
                    {
                        attackModifier += 0.2f;
                    }
                    break;
                case Cell.Terrians.Danger:
                    {
                        attackModifier -= 0.1f;
                        defenseModifier -= 0.1f;
                        agilityModifier -= 0.1f;
                    }
                    break;
            }
        }

        _Attack = (int)(_BaseAttack * attackModifier);
        _Defense = (int)(_BaseDefense * defenseModifier);
        _Agility = (int)(_BaseAgility * agilityModifier);
    }

    public void CheckForCrystal()
    {
        List<PickupController> crystals = GameManager.Instance._crystals.FindAll(c => c._position == _position);

        if (crystals.Count > 0)
        {
            int recovery = 0;
            for (int i = crystals.Count - 1; i >= 0; i--)
            {
                switch (crystals[i]._crystal_size)
                {
                    case PickupController.Crystal_Sizes.Small:
                        {
                            recovery = Random.Range(10, 20);
                        }
                        break;
                    case PickupController.Crystal_Sizes.Med:
                        {
                            recovery = Random.Range(20, 35);
                        }
                        break;
                    case PickupController.Crystal_Sizes.Large:
                        {
                            recovery = Random.Range(35, 50);
                        }
                        break;
                }
                TakeDamage(-recovery, false);
                Destroy(crystals[i].gameObject);
            }
        }
    }

    public void HealthChange_Smooth()
    {
        if (_showHPChange)
        {
            if (_HP_Display < _HP)
            {
                _HP_Display = Mathf.Clamp(_HP_Display + 5, 0, _HP);
            }
            else if (_HP_Display > _HP)
            {
                _HP_Display = Mathf.Clamp(_HP_Display - 5, _HP, _maxHP);
            }
            else
            {
                _showHPChange = false;
            }
        }
    }

    public void StartHPChange()
    {
        _showHPChange = true;
    }

    #endregion

    #region Sound Effects

    public void PlayHitSound()
    {
        if (HitSE != null)
        {
            SoundManager.Instance.PlaySE(gameObject.tag == "Player", HitSE);
        }
    }

    public void PlayHurtSound()
    {
        PlayHitSound();
        if (HurtSE != null)
        {
            SoundManager.Instance.PlayGameSE(HurtSE);
        }
    }

    public void PlayDefeatSound()
    {
        PlayHitSound();
        if (DefeatSE != null)
        {
            SoundManager.Instance.PlayGameSE(DefeatSE);
        }
    }

    public void PlayAttackSound()
    {
        if (AttackSE != null)
        {
            SoundManager.Instance.PlaySE(gameObject.tag == "Player", AttackSE);
        }
    }

    public void PlayDodgeSound()
    {
        if (DodgeSE != null)
        {
            SoundManager.Instance.PlaySE(gameObject.tag == "Player", DodgeSE);
        }
    }

    public void PlayBlockSound()
    {
        if (BlockSE != null)
        {
            SoundManager.Instance.PlaySE(gameObject.tag == "Player", BlockSE);
        }
    }

    #endregion
}
