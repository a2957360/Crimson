using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInputManager : Singleton<PlayerInputManager>
{
    #region Variables
    [HideInInspector]
    public TargetCell _target = null;
    public ChrController _HeroCursorHover = null;
    public ChrController _EnemyCursorHover = null;

    List<ChrController> Enemylist = new List<ChrController>();
    int EnemyIndex = 0;

    #region Components
    public GameObject ConfirmSelection;
    public Text ConfirmationText;

    public GameObject BattleConfirmSelection;
    public GameObject BattleCSBGLong;
    public GameObject BattleCSBGShort;
    public GameObject BattleConfirmSelectionButtonB;

    public GameObject RichTextImageHolder;
    public GameObject RichTextImageHolder2;
    public GameObject RichTextImageHolder2Buttons;

    public List<Sprite> ContorlsImage;
    public Sprite SuccessGoalImage;

    [HideInInspector]
    public int ContorlsImageNo = 0;

    [HideInInspector]
    public int ConfirmationState;

    public GameObject MainMenu;
    public MainMenuInput MainMenuScript;

    public GameObject TutorialMenu;
    public TutorialContorller TutorialScript;

    public SlotMachineInputManager SlotMachine;

    public OptionController optionscript;
    public GameObject option;
    #endregion

    #region Editor Variables
    public float _scrollingEdgeWidth = 50.0f;
    [HideInInspector]
    public bool _playerPhase = true;
    public float _shortPause = 0.18f;
    public float _regularPause = 0.35f;
    public float _longPause = 0.5f;

    public bool _showAttackRange = true;

    public bool _EasyMode = false;
    public bool _MapGrid = true;
    public bool _ExtraPath = true;
    public bool _Music = true;
    public bool _Hint = true;
    public bool _WeatherEffect = true;
    #endregion

    #region Cursor
    float _cursorOriginX;
    float _cursorOriginY;

    public int _cursorGridX;
    public int _cursorGridY;

    int _mapWidth;
    int _mapHeight;

    int _screenWidth;
    int _screenHeight;

    bool _isValidCursorSelection = true;
    bool _canClick = true; // stop spamming
    bool _canMove = true;

    //[HideInInspector]
    //public bool _pathSelectionMode = false;
    //[HideInInspector]
    //public bool _watchMode = false;
    //bool Menumode = false;

    public enum InputModes : int
    {
        None,
        ChrSelection,
        PathSelection,
        ConfirmSlection,
        Battle,
        Cinema,
        MainMenu,
        Option,
        MainMenuConfirm,
        SlotMachine,
        Controls,
        BattleSelection,
        BattleTypeDecision,
        Tutorial,
        TutorialConfirm,
    };
    public InputModes _inputMode = InputModes.ChrSelection;
    public InputModes _Templemode = InputModes.None;

    bool IsJoystick;
    #endregion 

    #region Debug
    public bool _debugMode = true;
    public Text _debugMouseText;
    public GameObject _darkenImage;
    #endregion

    #endregion

    #region Core GameFlow

    void Start()
    {
        IsJoystick = false;
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;
        _mapWidth = GameManager.Instance._mapWidth;
        _mapHeight = GameManager.Instance._mapHeight;
        _cursorOriginX = GameManager.Instance._mapCenterX - _mapWidth * 0.5f;
        _cursorOriginY = GameManager.Instance._mapCenterY - _mapHeight * 0.5f;

        CheckJoystick();

        InvokeRepeating("ShowPath", 2.0f, 0.1f);
        InvokeRepeating("ShowPotentialInfo", 2.0f, 0.1f);

        ConfirmationState = 0;
    }

    void LateUpdate()
    {
        //CheckJoystick();
        //Menumode = MenuController.Instance.ShowSelectionList && !MenuController.Instance.MoveAction;
        if (_canMove)
        {
            if (_inputMode == InputModes.ChrSelection || _inputMode == InputModes.PathSelection)
            {
                ControllerToGridPos();
            }
        }

        if (_canClick)
        {
            if (_playerPhase)
            {
                //if (_inputMode != InputModes.ConfirmSlection && _inputMode != InputModes.MainMenu && _inputMode != InputModes.MainMenuConfirm && _inputMode != InputModes.Cinema)
                //if (_inputMode == InputModes.ChrSelection || _inputMode == InputModes.PathSelection)
                //{
                //    if (IsJoystick)
                //    {
                //        ControllerToGridPos();
                //    }
                //    else
                //    {
                //        MouseToGridPos();
                //    }
                //}
                InputListener();

                if (_debugMode)
                {
                    UpdateDebugInfo();
                }
            }
            else
            {
                if (_inputMode == InputModes.Battle)
                {
                    if (Input.GetButtonDown("Fire5"))
                    {
                        MainMenuScript.disableButton = 3;
                        MainMenuScript.disableSlection();
                        MainMenuScript.num = MainMenuScript.disableButton + 1;
                        MainMenuScript.setcurserposition();
                        showMainmenu();
                        PauseClick();
                    }
                    ListenToCombatPress();
                }
                else if (_inputMode == InputModes.BattleTypeDecision)
                {
                    ListenToCombatDecision();
                }
                else if (_inputMode == InputModes.Cinema)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        EventManager.Instance.PlayEventNextPart();
                        PauseClick();
                    }
                }
                else if (_inputMode == InputModes.MainMenu)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        if (MainMenuScript.num == 4)
                        {
                            RichTextImageHolder2.SetActive(true);
                            RichTextImageHolder.SetActive(false);
                            MainMenu.SetActive(false);
                            _inputMode = InputModes.Controls;
                            MainMenuScript.num = 1 + MainMenuScript.disableButton;
                            MainMenuScript.setcurserposition();
                            RichTextImageHolder2.GetComponent<Image>().sprite = ContorlsImage[ContorlsImageNo];
                            RichTextImageHolder2Buttons.SetActive(true);
                            if (_darkenImage != null)
                                _darkenImage.SetActive(true);
                            PauseClick();
                        }
                        else
                        {
                            _inputMode = InputModes.MainMenuConfirm;
                            MainMenuScript.Confirm.SetActive(true);
                            MainMenuScript._screenDarken.SetActive(true);
                        }
                    }
                    if (Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire5"))
                    {
                        CameraManager.Instance._canFreeMove = true;
                        RichTextImageHolder.SetActive(false);
                        if (_Templemode == InputModes.BattleTypeDecision)
                        {
                            BattleConfirmSelection.SetActive(true);
                        }
                        MainMenuScript.showSlection();
                        MainMenuScript.disableButton = 0;

                        _inputMode = _Templemode;
                        MainMenuScript.num = 1;
                        MainMenuScript.setcurserposition();
                        MainMenu.SetActive(false);
                        PauseClick();

                        SoundManager.Instance.PlayBattleMusic(true, false);
                    }
                }
                else if (_inputMode == InputModes.MainMenuConfirm)
                {
                    if (Input.GetButtonDown("Fire2"))
                    {
                        Instance._inputMode = InputModes.MainMenu;
                        MainMenuScript.Confirm.SetActive(false);
                        MainMenuScript._screenDarken.SetActive(false);
                        PauseClick();
                    }

                    if (Input.GetButtonDown("Fire1"))
                    {
                        switch (MainMenuScript.num)
                        {
                            case 5:
                                SceneManager.LoadScene("Welcome");
                                break;
                            case 6:
                                Application.Quit();
                                break;
                        }
                    }
                }
                else if (_inputMode == InputModes.Controls)
                {
                    if (Input.GetAxis("Horizontal") < 0 && ContorlsImageNo > 0)
                    {
                        ContorlsImageNo--;
                        RichTextImageHolder2.GetComponent<Image>().sprite = ContorlsImage[ContorlsImageNo];
                        PauseClick();
                    }
                    if (Input.GetAxis("Horizontal") > 0 && ContorlsImageNo < (ContorlsImage.Count - 1))
                    {
                        ContorlsImageNo++;
                        RichTextImageHolder2.GetComponent<Image>().sprite = ContorlsImage[ContorlsImageNo];
                        PauseClick();
                    }
                    if (Input.GetAxis("H") < 0 && ContorlsImageNo > 0)
                    {
                        ContorlsImageNo--;
                        RichTextImageHolder2.GetComponent<Image>().sprite = ContorlsImage[ContorlsImageNo];
                        PauseClick();
                    }
                    if (Input.GetAxis("H") > 0 && ContorlsImageNo < (ContorlsImage.Count - 1))
                    {
                        ContorlsImageNo++;
                        RichTextImageHolder2.GetComponent<Image>().sprite = ContorlsImage[ContorlsImageNo];
                        PauseClick();
                    }
                    if (Input.GetButtonDown("Fire2"))
                    {
                        RichTextImageHolder2.SetActive(false);
                        RichTextImageHolder.SetActive(true);
                        RichTextImageHolder2Buttons.SetActive(false);
                        ContorlsImageNo = 0;
                        MainMenu.SetActive(true);
                        _inputMode = InputModes.MainMenu;
                        PauseClick();
                    }
                }
            }
        }
    }

    #endregion

    #region Functions

    public void InputListener()
    {
        if (_isValidCursorSelection)
        {
            switch (_inputMode)
            {
                case InputModes.ChrSelection:
                    {
                        if (Input.GetButtonDown("Fire1"))
                        {
                            SelectUnit(1);
                            PauseClick();
                        }
                        //to check enemy movement and moved hero's movement
                        if (Input.GetButtonDown("Fire2"))
                        {
                            AStarPathfinding.Instance.ClearPathCells();
                            SelectUnitCheckMovement();
                            PauseClick();
                        }
                        if (Input.GetButtonDown("Fire4"))
                        {
                            CameraManager.Instance.ChangeZoom();
                            PauseClick();
                        }
                        if (Input.GetButtonDown("Fire5"))
                        {
                            showMainmenu();
                            PauseClick();
                        }
                        if (Input.GetButtonDown("R1"))
                        {
                            //true find next
                            LocationNewCharacter(true);

                            PauseClick();
                        }
                        if (Input.GetButtonDown("L1"))
                        {
                            //false find pre
                            LocationNewCharacter(false);
                            PauseClick();
                        }
                    }
                    break;
                case InputModes.MainMenu:
                    {
                        if (Input.GetButtonDown("Fire1"))
                        {
                            if (MainMenuScript.num == 2)
                            {
                                RichTextImageHolder.SetActive(false);
                                MainMenu.SetActive(false);
                                option.SetActive(true);
                                _inputMode = InputModes.Option;
                                MainMenuScript.num = 1;
                                MainMenuScript.setcurserposition();
                                PauseClick();
                            }
                            else if (MainMenuScript.num == 4)
                            {
                                RichTextImageHolder2.SetActive(true);
                                RichTextImageHolder.SetActive(false);
                                MainMenu.SetActive(false);
                                _inputMode = InputModes.Controls;
                                MainMenuScript.num = 1 + MainMenuScript.disableButton;
                                MainMenuScript.setcurserposition();
                                RichTextImageHolder2.GetComponent<Image>().sprite = ContorlsImage[ContorlsImageNo];
                                RichTextImageHolder2Buttons.SetActive(true);
                                if (_darkenImage != null)
                                    _darkenImage.SetActive(true);
                                PauseClick();
                            }
                            else if (MainMenuScript.num == 3)
                            {
                                RichTextImageHolder.SetActive(false);
                                MainMenu.SetActive(false);
                                TutorialMenu.SetActive(true);
                                _inputMode = InputModes.Tutorial;
                                MainMenuScript.num = 1 + MainMenuScript.disableButton;
                                MainMenuScript.setcurserposition();
                                PauseClick();
                            }
                            else
                            {
                                _inputMode = InputModes.MainMenuConfirm;
                                MainMenuScript.Confirm.SetActive(true);
                                MainMenuScript._screenDarken.SetActive(true);
                            }
                        }
                        else if (Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire5"))
                        {
                            CameraManager.Instance._canFreeMove = true;
                            RichTextImageHolder.SetActive(false);
                            if (_Templemode == InputModes.BattleTypeDecision)
                            {
                                BattleConfirmSelection.SetActive(true);
                            }
                            MainMenuScript.showSlection();
                            MainMenuScript.disableButton = 0;

                            _inputMode = _Templemode;
                            MainMenuScript.num = 1;
                            MainMenuScript.setcurserposition();
                            MainMenu.SetActive(false);
                            PauseClick();

                            if (_Templemode == InputModes.Battle || _Templemode == InputModes.BattleTypeDecision)
                                SoundManager.Instance.PlayBattleMusic(true, false);
                            else
                                SoundManager.Instance.PlayMusic(true, false, 0);
                        }
                        //else if (Input.GetButtonDown("Fire3"))
                        //{
                        //    MissionManager.Instance.MissionOverEnd();
                        //}
                        //else if (Input.GetButtonDown("Fire4"))
                        //{
                        //    MissionManager.Instance.MissionWinEnd_AfterEndCutscene();
                        //}
                    }
                    break;
                case InputModes.Option:
                    {
                        if (Input.GetButtonDown("Fire2"))
                        {
                            RichTextImageHolder.SetActive(true);
                            MainMenu.SetActive(true);
                            option.SetActive(false);
                            _inputMode = InputModes.MainMenu;
                            PauseClick();
                        }
                    }
                    break;
                case InputModes.Controls:
                    if (Input.GetAxis("Horizontal") < 0 && ContorlsImageNo > 0)
                    {
                        ContorlsImageNo--;
                        RichTextImageHolder2.GetComponent<Image>().sprite = ContorlsImage[ContorlsImageNo];
                        SoundManager.Instance.PlaySelectionChangeSound();
                        PauseClick();
                    }
                    if (Input.GetAxis("Horizontal") > 0 && ContorlsImageNo < (ContorlsImage.Count - 1))
                    {
                        ContorlsImageNo++;
                        RichTextImageHolder2.GetComponent<Image>().sprite = ContorlsImage[ContorlsImageNo];
                        SoundManager.Instance.PlaySelectionChangeSound();
                        PauseClick();
                    }
                    if (Input.GetAxis("H") < 0 && ContorlsImageNo > 0)
                    {
                        ContorlsImageNo--;
                        RichTextImageHolder2.GetComponent<Image>().sprite = ContorlsImage[ContorlsImageNo];
                        SoundManager.Instance.PlaySelectionChangeSound();
                        PauseClick();
                    }
                    if (Input.GetAxis("H") > 0 && ContorlsImageNo < (ContorlsImage.Count - 1))
                    {
                        ContorlsImageNo++;
                        RichTextImageHolder2.GetComponent<Image>().sprite = ContorlsImage[ContorlsImageNo];
                        SoundManager.Instance.PlaySelectionChangeSound();
                        PauseClick();
                    }
                    if (Input.GetButtonDown("Fire2"))
                    {
                        RichTextImageHolder2.SetActive(false);
                        RichTextImageHolder.SetActive(true);
                        RichTextImageHolder2Buttons.SetActive(false);
                        ContorlsImageNo = 0;
                        MainMenu.SetActive(true);
                        _inputMode = InputModes.MainMenu;
                        PauseClick();
                    }
                    break;
                case InputModes.Tutorial:
                    if (Input.GetButtonDown("Fire1"))
                    {
                        TutorialScript.Confirm.SetActive(true);
                        _inputMode = InputModes.TutorialConfirm;
                        PauseClick();
                    }
                    if (Input.GetButtonDown("Fire2"))
                    {
                        TutorialMenu.SetActive(false);
                        RichTextImageHolder.SetActive(true);
                        MainMenu.SetActive(true);
                        _inputMode = InputModes.MainMenu;
                        PauseClick();
                    }
                    break;
                case InputModes.TutorialConfirm:
                    if (Input.GetButtonDown("Fire1"))
                    {
                        TutorialScript.Confirm.SetActive(false);
                        TutorialMenu.SetActive(false);
                        _inputMode = InputModes.Tutorial;

                        TutorialManager.Instance.PlayTutorialEvent(TutorialScript.num - 1);
                        PauseClick();
                    }
                    if (Input.GetButtonDown("Fire2"))
                    {
                        TutorialScript.Confirm.SetActive(false);
                        TutorialMenu.SetActive(true);
                        _inputMode = InputModes.Tutorial;
                        PauseClick();
                    }
                    break;
                case InputModes.MainMenuConfirm:
                    {
                        if (Input.GetButtonDown("Fire2"))
                        {
                            Instance._inputMode = InputModes.MainMenu;
                            MainMenuScript.Confirm.SetActive(false);
                            MainMenuScript._screenDarken.SetActive(false);
                            PauseClick();
                        }

                        if (Input.GetButtonDown("Fire1"))
                        {
                            switch (MainMenuScript.num)
                            {
                                case 1:
                                    GameManager.Instance.FinishAllCharacterturn();
                                    CameraManager.Instance._canFreeMove = true;
                                    _inputMode = InputModes.ChrSelection;
                                    MainMenuScript.num = 1;
                                    MainMenuScript.setcurserposition();
                                    MainMenu.SetActive(false);
                                    RichTextImageHolder.SetActive(false);
                                    MainMenuScript.Confirm.SetActive(false);
                                    MainMenuScript._screenDarken.SetActive(false);
                                    break;
                                case 2:
                                    break;
                                case 3:
                                    break;
                                case 4:
                                    break;
                                case 5:
                                    SceneManager.LoadScene("Welcome");
                                    break;
                                case 6:
                                    Application.Quit();
                                    break;

                            }
                        }
                    }
                    break;
                case InputModes.PathSelection:
                    {
                        if (Input.GetButtonDown("Fire1"))
                        {
                            //ClickOnPath();
                            //PauseClick();
                            _inputMode = InputModes.ConfirmSlection;
                            CheckConfirm();
                            PauseClick();
                        }
                        else if (Input.GetButtonDown("Fire2"))
                        {
                            DeselectUnit();
                            PauseClick();
                        }
                        else if (Input.GetButtonDown("Fire3"))
                        {
                            ToggleShowAttackRange();
                            PauseClick();
                        }
                        if (Input.GetButtonDown("Fire5"))
                        {
                            showMainmenu();
                            PauseClick();
                        }
                    }
                    break;
                case InputModes.ConfirmSlection:
                    {
                        if (Input.GetButtonDown("Fire1") && ConfirmationState != 2)
                        {
                            GameManager.Instance._curEnemy = null;
                            if (AStarPathfinding.Instance._path.Count == 0 && Enemylist.Count != 0)
                            {
                                _cursorGridX = (int)(GameManager.Instance._curUnit._position.x);
                                _cursorGridY = (int)(GameManager.Instance._curUnit._position.y);
                            }
                            ClickOnPath();
                            //clear all button
                            ConfirmationState = 4;
                            ConfirmSelection.SetActive(false);

                            Enemylist.Clear();
                            EnemyIndex = 0;

                            PauseClick();
                        }
                        else if (Input.GetButtonDown("Fire2"))
                        {
                            //clear all button
                            ConfirmationState = 4;
                            ConfirmSelection.SetActive(false);
                            CameraManager.Instance._canFreeMove = true;
                            if (_target != null)
                                _target.DeactivateFrame();
                            GameManager.Instance._curEnemy = null;

                            Enemylist.Clear();
                            EnemyIndex = 0;

                            _inputMode = InputModes.PathSelection;
                            PauseClick();
                        }
                        else if (Input.GetButtonDown("Fire3") && (ConfirmationState == 2 || ConfirmationState == 3))
                        {
                            //clear all button
                            ConfirmationState = 4;
                            ConfirmSelection.SetActive(false);
                            ClickOnEnemyorCanAttackEnemy();

                            Enemylist.Clear();
                            EnemyIndex = 0;
                            _inputMode = InputModes.BattleSelection;
                            PauseClick();
                        }
                        else if (Input.GetButtonDown("Fire4"))
                        {
                            if (_target != null && ConfirmationState == 2)
                            {
                                _target.ChangeAttackPosIndex();
                                GetConfirmationPathInfo();
                            }
                            else if (ConfirmationState == 3 && Enemylist.Count != 0)
                            {
                                ChangeAttackEnemy();
                                GetConfirmationEnemyInfo();
                            }
                            PauseClick();
                        }
                        //if (Input.GetButtonDown("Fire5"))
                        //{
                        //    MainMenu.SetActive(true);
                        //    _Templemode = _inputMode;
                        //    _inputMode = InputModes.MainMenu;
                        //    CameraManager.Instance._canFreeMove = false;
                        //    PauseClick();
                        //}
                    }
                    break;
                case InputModes.BattleSelection:
                    {
                        ConfirmationState = 5;
                        if (Input.GetButtonDown("Fire2"))
                        {
                            BattleConfirmSelection.SetActive(false);
                            ClickOnEnemyorCanAttackEnemy();
                            PauseClick();
                        }
                        if (Input.GetButtonDown("Fire3"))
                        {
                            BattleConfirmSelection.SetActive(false);
                            ClickOnEnemyorCanAttackEnemy();
                            PauseClick();
                        }
                        if (Input.GetButtonDown("Fire4"))
                        {
                            BattleConfirmSelection.SetActive(false);
                            ClickOnEnemyorCanAttackEnemy();
                            PauseClick();
                        }

                    }
                    break;
                case InputModes.Battle:
                    {
                        if (Input.GetButtonDown("Fire5"))
                        {
                            MainMenuScript.disableButton = 3;
                            MainMenuScript.disableSlection();
                            MainMenuScript.num = MainMenuScript.disableButton + 1;
                            MainMenuScript.setcurserposition();
                            showMainmenu();
                            PauseClick();
                        }
                        ListenToCombatPress();
                    }
                    break;
                case InputModes.BattleTypeDecision:
                    {
                        ListenToCombatDecision();
                    }
                    break;
                case InputModes.Cinema:
                    {
                        if (Input.GetButtonDown("Fire1"))
                        {
                            EventManager.Instance.PlayEventNextPart();
                            PauseClick();
                        }
                    }
                    break;
                case InputModes.SlotMachine:
                    {
                        if (Input.GetButtonDown("Fire1"))
                        {
                            SlotMachine.interSlotMachine();
                            PauseClick();
                        }
                    }
                    break;
            }
        }
    }

    void CheckConfirm()
    {
        //_potential = null;
        _target = AStarPathfinding.Instance.FindTargetCell(new Vector2(_cursorGridX, _cursorGridY));
        if (_target != null)
        {
            //  _curEnemy is set
            GameManager.Instance._curEnemy = _target._targetChr;
            _target.GetAttackRoute();

            //if (AStarPathfinding.Instance._path.Count > 0)
            //{
            //    //_inputMode = InputModes.ConfirmSlection;
            //    CameraManager.Instance._canFreeMove = false;
            //    ConfirmSelection.SetActive(true);
            //}
            //else if (CheckAttackableRange())
            //{
            //    //_inputMode = InputModes.ConfirmSlection;
            //    CameraManager.Instance._canFreeMove = false;
            //    ConfirmSelection.SetActive(true);
            //}
            //else
            //{
            //    _inputMode = InputModes.PathSelection;
            //}

            CameraManager.Instance._canFreeMove = false;
            //2 just Attack
            ConfirmationState = 2;
            ConfirmSelection.SetActive(true);
            GetConfirmationPathInfo();
        }
        else
        {
            Enemylist = AStarPathfinding.Instance.CheckActionableByPosition(new Vector2(_cursorGridX, _cursorGridY), GameManager.Instance._curUnit._attackType);
            GameManager.Instance._curEnemy = null;
            if (AStarPathfinding.Instance._path.Count != 0)
            {
                if (Enemylist.Count != 0)
                {
                    //  _curEnemy is set
                    GameManager.Instance._curEnemy = Enemylist[EnemyIndex];
                    _cursorGridX = (int)Enemylist[EnemyIndex]._position.x;
                    _cursorGridY = (int)Enemylist[EnemyIndex]._position.y;
                    //3 move and attack
                    ConfirmationState = 3;
                    ConfirmSelection.SetActive(true);
                    GetConfirmationEnemyInfo();
                }
                else
                {
                    //_inputMode = InputModes.ConfirmSlection;
                    CameraManager.Instance._canFreeMove = false;
                    //0 Move
                    ConfirmationState = 0;
                    ConfirmSelection.SetActive(true);
                    GetConfirmationPathInfo();
                }
            }
            else if (GameManager.Instance._curUnit._position == new Vector2(_cursorGridX, _cursorGridY))
            {
                if (Enemylist.Count != 0)
                {
                    //  _curEnemy is set
                    GameManager.Instance._curEnemy = Enemylist[EnemyIndex];
                    _cursorGridX = (int)Enemylist[EnemyIndex]._position.x;
                    _cursorGridY = (int)Enemylist[EnemyIndex]._position.y;
                    //3 move and attack
                    ConfirmationState = 3;
                    ConfirmSelection.SetActive(true);
                    GetConfirmationEnemyInfo();
                }
                else
                {
                    CameraManager.Instance._canFreeMove = false;
                    //1 stay
                    ConfirmationState = 1;
                    ConfirmSelection.SetActive(true);
                    GetConfirmationPathInfo();
                }
            }
            else
            {
                _inputMode = InputModes.PathSelection;
            }
        }
    }

    public void ListenToCombatDecision()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            BattleConfirmSelection.SetActive(false);
            CombatManager.Instance.BattleAuto();
        }
        else if (Input.GetButtonDown("Fire3"))
        {
            BattleConfirmSelection.SetActive(false);
            CombatManager.Instance.BattleDecisionDelayStart();
        }
        else if (Input.GetButtonDown("Fire2") && GameManager.Instance._momentum >= 60 && GameManager.Instance._playerPhase)
        {
            BattleConfirmSelection.SetActive(false);
            CombatManager.Instance.BattleInstantDeath();
        }
        if (Input.GetButtonDown("Fire5"))
        {
            MainMenuScript.disableButton = 3;
            MainMenuScript.disableSlection();
            MainMenuScript.num = MainMenuScript.disableButton + 1;
            MainMenuScript.setcurserposition();
            BattleConfirmSelection.SetActive(false);
            showMainmenu();
            PauseClick();
        }
    }

    public void ListenToCombatPress()
    {
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    CombatManager.Instance.ChangeDecisionValue(-1);
        //    PauseClick();
        //}
        //else if (Input.GetButtonDown("Fire2"))
        //{
        //    CombatManager.Instance.ChangeDecisionValue(1);
        //    PauseClick();
        //}
        //else if (Input.GetButtonDown("Fire3"))
        //{
        //    CombatManager.Instance.ChangeDecisionValue(0);
        //    PauseClick();
        //}
        //else if (Input.GetButtonDown("Fire4"))
        //{
        //    CombatManager.Instance.BattleDecisionEnd();
        //}
        if (Input.GetButtonDown("Fire1"))
        {
            CombatManager.Instance.BattleDecisionEnd();
        }
        else if (Input.GetAxis("Horizontal") >= 1 || Input.GetAxis("H") >= 1)
        {
            CombatManager.Instance.ChangeDecisionValue(-1);
            PauseClick();
        }
        else if (Input.GetAxis("Horizontal") <= -1 || Input.GetAxis("H") <= -1)
        {
            CombatManager.Instance.ChangeDecisionValue(1);
            PauseClick();
        }
        else if (Input.GetAxis("Vertical") >= 1 || Input.GetAxis("V") >= 1)
        {
            CombatManager.Instance.ChangeLocationValue(1);
            PauseClick();
        }
        else if (Input.GetAxis("Vertical") <= -1 || Input.GetAxis("V") <= -1)
        {
            CombatManager.Instance.ChangeLocationValue(-1);
            PauseClick();
        }
        else if (Input.GetButtonDown("R1"))
        {
            CombatManager.Instance.ChangeFriendIndex(true);
        }
        else if (Input.GetButtonDown("L1"))
        {
            CombatManager.Instance.ChangeFriendIndex(false);
        }
    }

    public void ClickOnPath()
    {
        // added safety when clicking
        //AStarPathfinding.Instance.GeneratePath(new Vector2(_cursorGridX, _cursorGridY), true);
        if (AStarPathfinding.Instance._path.Count > 0)
        {
            GameManager.Instance.MoveCurrentUnit();
        }
        //else if (GameManager.Instance._curEnemy != null)
        //{
        //    GameManager.Instance._curUnit.TurnFinished();
        //}
        else if (GameManager.Instance._curUnit._position == new Vector2(_cursorGridX, _cursorGridY))
        {
            GameManager.Instance._curUnit.TurnFinished();
        }
        //else if (GameManager.Instance._curEnemy != null && Vector3.Magnitude(GameManager.Instance._curEnemy._position - GameManager.Instance._curUnit._position) == GameManager.Instance._curUnit._AttackRange)
        //{
        //    GameManager.Instance._curUnit.TurnFinished();
        //}
    }

    void ClickOnEnemyorCanAttackEnemy()
    {
        if (AStarPathfinding.Instance._path.Count > 0)
        {
            GameManager.Instance.MoveCurrentUnit();
        }
        else if (GameManager.Instance._curEnemy != null)
        {
            GameManager.Instance._curUnit.TurnFinished();
        }
        else if (GameManager.Instance._curUnit._position == new Vector2(_cursorGridX, _cursorGridY))
        {
            GameManager.Instance._curUnit.TurnFinished();
        }
    }

    //public bool CheckAttackableRange()
    //{
    //    if (GameManager.Instance._curUnit != null && GameManager.Instance._curEnemy != null)
    //    {
    //        float distance = Mathf.Abs((GameManager.Instance._curUnit._position - GameManager.Instance._curEnemy._position).x) + Mathf.Abs((GameManager.Instance._curUnit._position - GameManager.Instance._curEnemy._position).y);
    //        if (distance == GameManager.Instance._curUnit._AttackRange)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    // type 1: hero 2: enemy
    private void SelectUnit(int type)
    {
        GameManager.Instance.FindUnit(_cursorGridX, _cursorGridY, type);
    }

    //to check enemy movement and moved hero's movement
    private void SelectUnitCheckMovement()
    {
        GameManager.Instance.FindUnitMovement(_cursorGridX, _cursorGridY);
    }

    private void DeselectUnit()
    {
        GameManager.Instance.SoftDeselectCurUnit();
    }

    public void ShowPath()
    {
        //_potential = null;
        if (_playerPhase && _isValidCursorSelection && _inputMode == InputModes.PathSelection)
        {
            Vector2 curGrid = new Vector2(_cursorGridX, _cursorGridY);
            AStarPathfinding.Instance.GeneratePath(curGrid);

            if (_showAttackRange)
            {
                if (AStarPathfinding.Instance.FindAStarCell(curGrid, false, false) || GameManager.Instance._curUnit._position == curGrid)
                    AStarPathfinding.Instance.GenerateTargetIcon(curGrid);
                else
                    AStarPathfinding.Instance.ClearTargetIcons();
            }

            //_potential = AStarPathfinding.Instance.FindTargetCell(curGrid);
        }
    }

    public void ShowPotentialInfo()
    {
        if (_playerPhase && _isValidCursorSelection)
        {
            //Vector2 curGrid = new Vector2(_cursorGridX, _cursorGridY);
            if (_inputMode == InputModes.PathSelection)
            {
                _EnemyCursorHover = GameManager.Instance.CheckUnit(_cursorGridX, _cursorGridY, 2);
                _HeroCursorHover = null;
            }
            else if (_inputMode == InputModes.ChrSelection)
            {
                _HeroCursorHover = GameManager.Instance.CheckUnit(_cursorGridX, _cursorGridY, 1);
                _EnemyCursorHover = GameManager.Instance.CheckUnit(_cursorGridX, _cursorGridY, 2);
            }
        }
        else
        {
            _EnemyCursorHover = null;
            _HeroCursorHover = null;
        }
    }

    //public void ShowAttackRange()
    //{
    //if (_showAttackRange && _playerPhase && _isValidCursorSelection && _inputMode == InputModes.PathSelection)
    //{
    //    Vector2 curGrid = new Vector2(_cursorGridX, _cursorGridY);
    //    if (AStarPathfinding.Instance.FindAStarCell(curGrid, false) || GameManager.Instance._curUnit._position == curGrid)
    //        AStarPathfinding.Instance.GenerateTargetIcon(curGrid);
    //    else
    //        AStarPathfinding.Instance.ClearTargetIcons();
    //}
    //}

    public void ToggleShowAttackRange()
    {
        _showAttackRange = !_showAttackRange;

        if (!_showAttackRange)
            AStarPathfinding.Instance.ClearTargetIcons();
        //if (AStarPathfinding.Instance._targetIcons.Count == 0)
        //    _showAttackRange = true;
        //else
        //    _showAttackRange = false;

        //if (_showAttackRange)
        //{
        //    AStarPathfinding.Instance.GenerateTargetIconCurrent();
        //}
        //else
        //{
        //    AStarPathfinding.Instance.ClearTargetIcons();
        //}
    }

    //public void EdgeScroll()
    //{
    //    if (_playerPhase)
    //    {
    //        if (Input.mousePosition.x > _screenWidth - _scrollingEdgeWidth)
    //        {
    //            CameraManager.Instance.MoveCameraRight();
    //        }

    //        if (Input.mousePosition.x < 0 + _scrollingEdgeWidth)
    //        {
    //            CameraManager.Instance.MoveCameraLeft();
    //        }

    //        if (Input.mousePosition.y > _screenHeight - _scrollingEdgeWidth)
    //        {
    //            CameraManager.Instance.MoveCameraUp();
    //        }

    //        if (Input.mousePosition.y < 0 + _scrollingEdgeWidth)
    //        {
    //            CameraManager.Instance.MoveCameraDown();
    //        }
    //    }
    //}

    #endregion

    #region Helpers

    private void PauseClick()
    {
        _canClick = false;
        Invoke("ResetClick", _regularPause);
    }

    private void ShortPauseClick()
    {
        _canMove = false;
        Invoke("resetAxis", _shortPause);
        //_canClick = false;
        //Invoke("ResetClick", _shortPause);
    }

    private void resetAxis()
    {
        _canMove = true;
    }

    private void ResetClick()
    {
        _canClick = true;
    }

    public void MouseToGridPos()
    {
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if (!screenRect.Contains(Input.mousePosition))
        {
            _isValidCursorSelection = false;
        }
        else
        {
            _isValidCursorSelection = true;
            var _cursorPos = Input.mousePosition;
            _cursorPos = Camera.main.ScreenToWorldPoint(_cursorPos);
            _cursorPos.x = _cursorPos.x - _cursorOriginX;
            _cursorPos.y = _cursorPos.y - _cursorOriginY;
            if (_cursorPos.x > 0 && _cursorPos.y > 0 && _cursorPos.x < _mapWidth && _cursorPos.y < _mapHeight)
            {
                _cursorGridX = (int)(_cursorPos.x);
                _cursorGridY = (int)(_cursorPos.y);
            }
            else
            {
                _isValidCursorSelection = false;
            }
        }
    }

    void ControllerToGridPos()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(Input.GetAxis("Vertical")) || Mathf.Abs(Input.GetAxis("H")) > Mathf.Abs(Input.GetAxis("V")))
        {
            if ((Input.GetAxis("Horizontal") > 0 || Input.GetAxis("H") > 0) && _cursorGridX < GameManager.Instance._mapWidth - 1)
            {
                _cursorGridX++;
                ShortPauseClick();
            }
            if ((Input.GetAxis("Horizontal") < 0 || Input.GetAxis("H") < 0) && _cursorGridX > 0)
            {
                _cursorGridX--;
                ShortPauseClick();
            }
        }
        else if (Mathf.Abs(Input.GetAxis("Horizontal")) < Mathf.Abs(Input.GetAxis("Vertical")) || Mathf.Abs(Input.GetAxis("H")) < Mathf.Abs(Input.GetAxis("V")))
        {
            if ((Input.GetAxis("Vertical") > 0 || Input.GetAxis("V") > 0) && _cursorGridY < GameManager.Instance._mapHeight - 1)
            {
                _cursorGridY++;
                ShortPauseClick();
            }
            if ((Input.GetAxis("Vertical") < 0 || Input.GetAxis("V") < 0) && _cursorGridY > 0)
            {
                _cursorGridY--;
                ShortPauseClick();
            }
        }

        //if (Input.GetAxis("Horizontal") > 0 && _cursorGridX < GameManager.Instance._mapWidth - 1)
        //{
        //    _cursorGridX++;
        //    ShortPauseClick();
        //}
        //if (Input.GetAxis("Horizontal") < 0 && _cursorGridX > 0)
        //{
        //    _cursorGridX--;
        //    ShortPauseClick();
        //}
        //if (Input.GetAxis("Vertical") > 0 && _cursorGridY < GameManager.Instance._mapHeight - 1)
        //{
        //    _cursorGridY++;
        //    ShortPauseClick();
        //}
        //if (Input.GetAxis("Vertical") < 0 && _cursorGridY > 0)
        //{
        //    _cursorGridY--;
        //    ShortPauseClick();
        //}
        //if (Input.GetAxis("H") > 0 && _cursorGridX < GameManager.Instance._mapWidth - 1)
        //{
        //    _cursorGridX++;
        //    ShortPauseClick();
        //}
        //if (Input.GetAxis("H") < 0 && _cursorGridX > 0)
        //{
        //    _cursorGridX--;
        //    ShortPauseClick();
        //}
        //if (Input.GetAxis("V") > 0 && _cursorGridY < GameManager.Instance._mapHeight - 1)
        //{
        //    _cursorGridY++;
        //    ShortPauseClick();
        //}
        //if (Input.GetAxis("V") < 0 && _cursorGridY > 0)
        //{
        //    _cursorGridY--;
        //    ShortPauseClick();
        //}
    }

    void CheckJoystick()
    {
        //Get Joystick Names
        string[] temp = Input.GetJoystickNames();

        //Check whether array contains anything
        if (temp.Length > 0)
        {
            //Iterate over every element
            for (int i = 0; i < temp.Length; ++i)
            {
                //Check if the string is empty or not
                if (!string.IsNullOrEmpty(temp[i]))
                {
                    IsJoystick = true;
                }
                else
                {
                    IsJoystick = false;
                }
            }
        }
    }

    public void GetConfirmationPathInfo()
    {
        if (ConfirmationText != null)
        {
            if (_target != null)
            {
                ConfirmationText.text = "Path: " + (_target._attackPosIndex + 1).ToString() + "/" + _target._attackPositions.Count;
            }
            else
            {
                ConfirmationText.text = " ";
            }
            SoundManager.Instance.PlayConfirmation();
            HintManager.Instance.StopHintPlay(false);
        }
    }

    public void GetConfirmationEnemyInfo()
    {
        if (ConfirmationText != null)
        {
            if (Enemylist.Count != 0)
            {
                ConfirmationText.text = "Target: " + (EnemyIndex + 1).ToString() + "/" + Enemylist.Count;
            }
            else
            {
                ConfirmationText.text = " ";
            }
            SoundManager.Instance.PlayConfirmation();
            HintManager.Instance.StopHintPlay(false);
        }
    }

    void ChangeAttackEnemy()
    {
        if (EnemyIndex < (Enemylist.Count - 1))
        {
            EnemyIndex++;
        }
        else
        {
            EnemyIndex = 0;
        }
        GameManager.Instance._curEnemy = Enemylist[EnemyIndex];
        _cursorGridX = (int)Enemylist[EnemyIndex]._position.x;
        _cursorGridY = (int)Enemylist[EnemyIndex]._position.y;
    }

    //next is ture find next false find pre
    void LocationNewCharacter(bool Next)
    {
        List<ChrController> hlist = GameManager.Instance._heros.FindAll(hs => hs._turn < 200 && !hs._isDead);
        if (hlist.Count < 1)
        {
            return;
        }
        if (GameManager.Instance._heros.Find(s => s._position.x == _cursorGridX && s._position.y == _cursorGridY && s._turn < 200))
        {
            for (int i = 0; i < hlist.Count; i++)
            {
                if (hlist[i]._position.x == _cursorGridX && hlist[i]._position.y == _cursorGridY)
                {
                    if (Next)
                    {
                        if (i < (hlist.Count - 1))
                        {
                            _cursorGridX = (int)hlist[i + 1]._position.x;
                            _cursorGridY = (int)hlist[i + 1]._position.y;
                            break;
                        }
                        else
                        {
                            _cursorGridX = (int)hlist[0]._position.x;
                            _cursorGridY = (int)hlist[0]._position.y;
                            break;
                        }
                    }
                    else
                    {
                        if (i > 0)
                        {
                            _cursorGridX = (int)hlist[i - 1]._position.x;
                            _cursorGridY = (int)hlist[i - 1]._position.y;
                            break;
                        }
                        else
                        {
                            _cursorGridX = (int)hlist[hlist.Count - 1]._position.x;
                            _cursorGridY = (int)hlist[hlist.Count - 1]._position.y;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            _cursorGridX = (int)hlist[0]._position.x;
            _cursorGridY = (int)hlist[0]._position.y;
        }
    }

    void showMainmenu()
    {
        SoundManager.Instance.PlayConfirmation();
        SoundManager.Instance.PlayMenu(true);
        //SoundManager.Instance.PlayMusic(false, false, 0);
        MainMenu.SetActive(true);
        RichTextImageHolder.SetActive(true);
        RichTextImageHolder.GetComponent<Image>().sprite = SuccessGoalImage;
        _Templemode = _inputMode;
        _inputMode = InputModes.MainMenu;
        CameraManager.Instance._canFreeMove = false;
    }

    #endregion

    #region Debug

    public void UpdateDebugInfo()
    {
        if (_debugMouseText != null)
        {
            if (_isValidCursorSelection)
                _debugMouseText.text = "X: " + _cursorGridX + " | Y: " + _cursorGridY;
            else
                _debugMouseText.text = "Invalid";
        }
    }

    #endregion
}
