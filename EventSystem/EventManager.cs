using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : Singleton<EventManager>
{
    public List<Sprite> _LPortraits;
    public List<Sprite> _SPortraits;

    // for General Stuff
    public List<Sprite> _richTextImages;

    // for MissionWin/MissionLoss/Death/Story
    public List<Sprite> _richTextEvents;

    public List<Sprite> _tutorialEvents;

    public List<Sprite> _eventBGs;

    [Header("Events")]
    public List<Event> _events;
    [Space(20)]

    [Header("Battle Events")]
    public List<Event> _battleEvents;
    [Space(20)]

    //[HideInInspector]
    public Event _curEvent;
    public Event _curEventLinger;
    //[HideInInspector]
    public int _curIndex;
    private bool _play = false;
    public bool _waitMode = false;

    //public Text _infoUI;
    // Text _battleUI;

    // for the small portrait
    public GameObject _leftDialogueUI;
    public GameObject _rightDialogueUI;

    public Image _leftSpeaker;
    public Image _rightSpeaker;

    public Text _leftText;
    public Text _rightText;

    public Image _leftTextBG;
    public Image _rightTextBG;

    public GameObject _leftConfirm;
    public GameObject _rightConfirm;

    // for the large portrait
    public GameObject _leftLDialogueUI;
    public GameObject _rightLDialogueUI;

    public Image _leftLSpeaker;
    public Image _rightLSpeaker;

    public Image _leftLTextBG;
    public Image _rightLTextBG;

    public Text _leftLText;
    public Text _rightLText;

    public GameObject _leftLConfirm;
    public GameObject _rightLConfirm;

    public GameObject _centerLDialogueUI;
    public Text _centerLText;

    public Image _richTextImage; // small
    public Image _richTextImage2; // medium
    public Image _richTextImage3; // large
    public Image _ImageDarken;

    public Image _eventBG;

    public float _autoPlayTime = 3.0f;
    private float _autoPlayTimer = 0;

    public Sprite _icon_Dodge;
    public Sprite _icon_Guard;
    public Sprite _icon_Counter;
    public Sprite _icon_Power;
    public Sprite _icon_Technical;
    public Sprite _icon_Precision;
    public Sprite _icon_Random;

    public void SetBattlePortraits(Sprite left, Sprite right)
    {
        StopEventPlay();
        CombatManager.Instance._enemyBattleTellVisual1.gameObject.SetActive(false);
        CombatManager.Instance._enemyBattleTellVisual2.gameObject.SetActive(false);
        CombatManager.Instance._playerBattleTellVisual.gameObject.SetActive(false);
        _leftSpeaker.sprite = left;
        _rightSpeaker.sprite = right;
    }

    public void FindCalendarEvent(int mission, int round)
    {
        _curEvent = _events.Find(e => (e._trigger == Event.TriggerTypes.Calendar && (e._mission == mission) && (e._round == round)));
        if (_curEvent != null)
        {
            if (_curEvent._hasPhaseLimit && _curEvent._phaseLimit <= MissionManager.Instance._curPhase)
                _curEvent = null;
        }
        if (_curEvent != null)
        {
            MissionManager.Instance._curPhase += _curEvent._phaseChange;
            StartEvent();
        }
    }

    public void FindBattleEvent(int type, int diffculty, int targeting, Sprite left, Sprite right)
    {
        SetBattlePortraits(left, right);

        List<Event> temp = new List<Event>();

        switch (diffculty)
        {
            case 1:
                {
                    temp = _battleEvents.FindAll(e => (e._battleDiffculty == 1) && (e._battleType == type) && (e._battleTargeting == targeting));
                }
                break;
            case 2:
                {
                    temp = _battleEvents.FindAll(e => (e._battleDiffculty == 1 || e._battleDiffculty == 2) && (e._battleType == type) && (e._battleTargeting == targeting));
                }
                break;
            case 3:
                {
                    temp = _battleEvents.FindAll(e => (e._battleType == type) && (e._battleTargeting == targeting));
                }
                break;
        }

        if (temp.Count > 0)
        {
            _curEvent = temp[Random.Range(0, temp.Count)];
            Invoke("StartEvent", 0.1f);
        }
    }

    //public void FindBattleOutcomeEvent(int outcomeType, int outComeSubtype, bool leftside, Sprite left, Sprite right)
    //{
    //    SetBattlePortraits(left, right);
    //    _curEvent = _events.Find(e => (e._trigger == Event.TriggerTypes.Battle) && (e._battleOutomeType == outcomeType) && (e._battleOutcomeSubType == outComeSubtype) && e._leftSide == leftside);
    //    StartEvent();
    //}

    //public void FindSpecialEvent(int type, int subType)
    //{
    //    _curEvent = _events.Find(e => (e._trigger == Event.TriggerTypes.Special) && (e._subType == subType) && (e._type == type));
    //    StartEvent();
    //}

    public void FindGameStateEvent(int mission, int gameStateType)
    {
        _curEvent = _events.Find(e => (e._trigger == Event.TriggerTypes.GameState) && (e._mission == mission) && (e._gameStateType == gameStateType));
        StartEvent();
    }

    public void PlaySpecialEvent(Event special)
    {
        _curEvent = special;
        if (_curEvent != null)
        {
            if (_curEvent._trigger == Event.TriggerTypes.Story_Explain
            || _curEvent._trigger == Event.TriggerTypes.GameState
            || _curEvent._trigger == Event.TriggerTypes.GameState_End)
            {
                _eventBG.gameObject.SetActive(true);
                _eventBG.sprite = _eventBGs[0];
            }
            else if (_curEvent._trigger == Event.TriggerTypes.BattleTutorial || _curEvent._trigger == Event.TriggerTypes.Tutorial)
            {
                if (_ImageDarken != null)
                    _ImageDarken.gameObject.SetActive(true);
            }
            MissionManager.Instance._curPhase += _curEvent._phaseChange;
            Invoke("StartEvent", 0.1f);
        }
    }

    #region inserts

    public void InsertDialogue(bool leftSide, Sprite portrait, string dialogue, bool wait)
    {
        if (leftSide)
        {
            _leftDialogueUI.SetActive(true);
            SetAlpha(_leftTextBG, true);
            SetAlphaText(_leftText, true);
            _leftConfirm.SetActive(wait);
            _leftSpeaker.sprite = portrait;
            _leftText.text = dialogue;
        }
        else
        {
            _rightDialogueUI.SetActive(true);
            SetAlpha(_rightTextBG, true);
            SetAlphaText(_rightText, true);
            _rightConfirm.SetActive(wait);
            _rightSpeaker.sprite = portrait;
            _rightText.text = dialogue;
        }
    }

    public void DeleteDialogue(bool leftSide, bool rightSide)
    {
        if (leftSide)
            _leftDialogueUI.SetActive(false);
        if (rightSide)
            _rightDialogueUI.SetActive(false);
    }

    #endregion

    #region event core

    private void StartEvent()
    {
        if (_curEvent != null)
        {
            if (_curEvent._trigger == Event.TriggerTypes.Battle)
            {
                if (_curEvent._isRandom)
                {
                    CombatManager.Instance._enemyBattleTellVisual1.sprite = _icon_Random;
                    CombatManager.Instance._enemyBattleTellVisual1.gameObject.SetActive(true);
                }
                else
                {
                    if (_curEvent._isRed)
                    {
                        if (_curEvent._battleType <= 3) // offense
                        {
                            CombatManager.Instance._enemyBattleTellVisual1.sprite = _icon_Power;
                        }
                        else
                        {
                            CombatManager.Instance._enemyBattleTellVisual1.sprite = _icon_Guard;
                        }
                        CombatManager.Instance._enemyBattleTellVisual1.gameObject.SetActive(true);
                        if (_curEvent._isGreen)
                        {
                            if (_curEvent._battleType <= 3) // offense
                            {
                                CombatManager.Instance._enemyBattleTellVisual2.sprite = _icon_Precision;
                            }
                            else
                            {
                                CombatManager.Instance._enemyBattleTellVisual2.sprite = _icon_Counter;
                            }
                            CombatManager.Instance._enemyBattleTellVisual2.gameObject.SetActive(true);
                        }
                        else
                        {
                            if (_curEvent._isBlue)
                            {
                                if (_curEvent._battleType <= 3) // offense
                                {
                                    CombatManager.Instance._enemyBattleTellVisual2.sprite = _icon_Technical;
                                }
                                else
                                {
                                    CombatManager.Instance._enemyBattleTellVisual2.sprite = _icon_Dodge;
                                }
                                CombatManager.Instance._enemyBattleTellVisual2.gameObject.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        if (_curEvent._isGreen)
                        {
                            if (_curEvent._battleType <= 3) // offense
                            {
                                CombatManager.Instance._enemyBattleTellVisual1.sprite = _icon_Precision;
                            }
                            else
                            {
                                CombatManager.Instance._enemyBattleTellVisual1.sprite = _icon_Counter;
                            }
                            CombatManager.Instance._enemyBattleTellVisual1.gameObject.SetActive(true);
                            if (_curEvent._isBlue)
                            {
                                if (_curEvent._battleType <= 3) // offense
                                {
                                    CombatManager.Instance._enemyBattleTellVisual2.sprite = _icon_Technical;
                                }
                                else
                                {
                                    CombatManager.Instance._enemyBattleTellVisual2.sprite = _icon_Dodge;
                                }
                                CombatManager.Instance._enemyBattleTellVisual2.gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            if (_curEvent._isBlue)
                            {
                                if (_curEvent._battleType <= 3) // offense
                                {
                                    CombatManager.Instance._enemyBattleTellVisual1.sprite = _icon_Technical;
                                }
                                else
                                {
                                    CombatManager.Instance._enemyBattleTellVisual1.sprite = _icon_Dodge;
                                }
                                CombatManager.Instance._enemyBattleTellVisual1.gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
            else if (_curEvent._trigger == Event.TriggerTypes.GameState || _curEvent._trigger == Event.TriggerTypes.GameState_End)
            {
                SoundManager.Instance.PlayMusic(true, true, 0);
            }
            else if (_curEvent._trigger == Event.TriggerTypes.Story_Explain)
            {
                SoundManager.Instance.PlayMusic(true, true, 1);
            }
            //_curEventLinger = null;
            //StopEventPlay();
            _curIndex = 0;
            _autoPlayTimer = 0;
            _play = true;
            _waitMode = _curEvent._waitMode;
            HintManager.Instance.StopHintPlay(false);
            if (_waitMode)
            {
                PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.Cinema;
                CameraManager.Instance._canFreeMove = false;
            }
            else
            {
                _leftLConfirm.SetActive(false);
                _rightLConfirm.SetActive(false);
                _leftConfirm.SetActive(false);
                _rightConfirm.SetActive(false);
            }
            PlayDialogue();
        }
    }

    public void PlayDialogue()
    {
        Event.Dialogue dia = _curEvent._dialogueLines[_curIndex];

        if (dia._targetName != "")
        {
            ChrController chr = GameManager.Instance._heros.Find(h => h._name == dia._targetName && !h._isDead);
            if (chr == null)
                chr = GameManager.Instance._enemies.Find(h => h._name == dia._targetName && !h._isDead);

            if (chr != null)
            {
                CameraManager.Instance.FollowTargetByVector(chr._position);
            }
        }
        else if (dia._x != -1 && dia._y != -1)
        {
            CameraManager.Instance.FollowTargetByVector(new Vector2(dia._x, dia._y));
        }

        if (dia._isImage)
        {
            if (_curEvent._trigger == Event.TriggerTypes.Story_Explain)
            {
                if (_richTextImage2 != null)
                {
                    _richTextImage2.gameObject.SetActive(true);
                    _richTextImage2.sprite = _richTextEvents[dia._speakerNum];
                }
                if (_ImageDarken != null)
                    _ImageDarken.gameObject.SetActive(true);
                _centerLDialogueUI.SetActive(true);
                _centerLText.text = _curEvent._lines[dia._line];
                _eventBG.gameObject.SetActive(true);
                _eventBG.sprite = _eventBGs[dia._BG];
            }
            else if (_curEvent._trigger == Event.TriggerTypes.GameState || _curEvent._trigger == Event.TriggerTypes.GameState_End)
            {
                if (_richTextImage != null)
                {
                    _richTextImage.gameObject.SetActive(true);

                    if (!_curEvent._useTutorialImages)
                        _richTextImage.sprite = _richTextImages[dia._speakerNum];
                    else
                        _richTextImage.sprite = _tutorialEvents[dia._speakerNum];
                }
            }
            else
            {
                if (_richTextImage2 != null)
                {
                    _richTextImage2.gameObject.SetActive(true);
                    if (_curEvent._trigger == Event.TriggerTypes.UnitDefeat
                        || _curEvent._trigger == Event.TriggerTypes.MissionLoss
                        || _curEvent._trigger == Event.TriggerTypes.MissionWin)
                    {
                        _richTextImage2.sprite = _richTextEvents[dia._speakerNum];
                    }
                    else
                    {
                        if (_curEvent._useTutorialImages
                            || _curEvent._trigger == Event.TriggerTypes.Tutorial
                            || _curEvent._trigger == Event.TriggerTypes.BattleTutorial)
                            _richTextImage2.sprite = _tutorialEvents[dia._speakerNum];
                        else

                            _richTextImage2.sprite = _richTextImages[dia._speakerNum];
                    }
                    if (_ImageDarken != null)
                        _ImageDarken.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (dia._leftSide)
            {
                if (_curEvent._trigger == Event.TriggerTypes.GameState || _curEvent._trigger == Event.TriggerTypes.GameState_End)
                {
                    _eventBG.gameObject.SetActive(true);
                    _eventBG.sprite = _eventBGs[dia._BG];
                    _leftLDialogueUI.SetActive(true);
                    if (dia._speakerNum < _LPortraits.Count)
                        _leftLSpeaker.sprite = _LPortraits[dia._speakerNum];
                    _leftLText.text = _curEvent._lines[dia._line];
                    SetAlpha(_leftLTextBG, true);
                    SetAlphaText(_leftLText, true);
                    SetAlpha(_rightLTextBG, false);
                    SetAlphaText(_rightLText, false);
                    if (_waitMode)
                    {
                        _leftLConfirm.SetActive(true);
                        _rightLConfirm.SetActive(false);
                    }
                }
                else
                {
                    _leftDialogueUI.SetActive(true);
                    if (_curEvent._trigger != Event.TriggerTypes.Battle)
                    {
                        if (dia._speakerNum < _SPortraits.Count)
                            _leftSpeaker.sprite = _SPortraits[dia._speakerNum];
                    }
                    _leftText.text = _curEvent._lines[dia._line];
                    SetAlpha(_leftTextBG, true);
                    SetAlphaText(_leftText, true);
                    SetAlpha(_rightTextBG, false);
                    SetAlphaText(_rightText, false);
                    if (_waitMode)
                    {
                        _leftConfirm.SetActive(true);
                        _rightConfirm.SetActive(false);
                    }
                }
            }
            else
            {
                if (_curEvent._trigger == Event.TriggerTypes.GameState || _curEvent._trigger == Event.TriggerTypes.GameState_End)
                {
                    _eventBG.gameObject.SetActive(true);
                    _eventBG.sprite = _eventBGs[dia._BG];
                    _rightLDialogueUI.SetActive(true);
                    if (dia._speakerNum < _LPortraits.Count)
                        _rightLSpeaker.sprite = _LPortraits[dia._speakerNum];
                    _rightLText.text = _curEvent._lines[dia._line];
                    SetAlpha(_leftLTextBG, false);
                    SetAlphaText(_leftLText, false);
                    SetAlpha(_rightLTextBG, true);
                    SetAlphaText(_rightLText, true);
                    if (_waitMode)
                    {
                        _leftLConfirm.SetActive(false);
                        _rightLConfirm.SetActive(true);
                    }
                }
                else
                {
                    _rightDialogueUI.SetActive(true);
                    if (_curEvent._trigger != Event.TriggerTypes.Battle)
                    {
                        if (dia._speakerNum < _SPortraits.Count)
                            _rightSpeaker.sprite = _SPortraits[dia._speakerNum];
                    }
                    _rightText.text = _curEvent._lines[dia._line];
                    SetAlpha(_leftTextBG, false);
                    SetAlphaText(_leftText, false);
                    SetAlpha(_rightTextBG, true);
                    SetAlphaText(_rightText, true);
                    if (_waitMode)
                    {
                        _leftConfirm.SetActive(false);
                        _rightConfirm.SetActive(true);
                    }
                }
            }
        }

        if (dia._sendEvent && _curEvent._targetFunc.Count > 0)
        {
            for (int i = 0; i < _curEvent._targetFunc.Count; i++)
            {
                SendMessage(_curEvent._targetFunc[i], _curEvent._funcPara[i]);
            }
        }
        //if (!_waitMode)
        //    Invoke("PlayEventNextPart", _autoPlayTime);
    }

    public void StopEventPlay()
    {
        _curIndex = 0;
        _curEventLinger = _curEvent;
        _curEvent = null;
        _play = false;
        _leftDialogueUI.SetActive(false);
        _rightDialogueUI.SetActive(false);
        _leftLDialogueUI.SetActive(false);
        _rightLDialogueUI.SetActive(false);
        _centerLDialogueUI.SetActive(false);
        _richTextImage.gameObject.SetActive(false);
        _richTextImage2.gameObject.SetActive(false);
        _richTextImage3.gameObject.SetActive(false);

        if (_waitMode)
        {
            PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.ChrSelection;
            CameraManager.Instance._canFreeMove = true;
        }

        Invoke("StopEventPlayAfter", 0.1f);
    }

    public void StopEventPlayAfter()
    {
        _eventBG.gameObject.SetActive(false);
        if (_ImageDarken != null)
            _ImageDarken.gameObject.SetActive(false);

        if (_curEventLinger != null)
        {
            switch (_curEventLinger._trigger)
            {
                case Event.TriggerTypes.Special:
                    {
                        GameManager.Instance.FinishUnitTurn(GameManager.Instance._curUnit.gameObject.tag == "Player" ? 1 : 2);
                    }
                    break;
                case Event.TriggerTypes.BattleTutorial:
                    {
                        CombatManager.Instance._progress = CombatManager.BattleProgress.Decision;
                        PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.None;
                        CombatManager.Instance.BattleDecision();
                    }
                    break;
                case Event.TriggerTypes.GameState:
                    {
                        if (_curEventLinger._gameStateType == 0)
                        {
                            GameManager.Instance.NewRound();
                        }
                    }
                    break;
                case Event.TriggerTypes.MissionWin:
                    {
                        MissionManager.Instance.MissionWinEnd();
                    }
                    break;
                case Event.TriggerTypes.MissionLoss:
                    {
                        MissionManager.Instance.MissionOverEnd();
                    }
                    break;
                case Event.TriggerTypes.Spawn:
                    {
                        GameManager.Instance.NewRoundAfterSpawn();
                    }
                    break;
                case Event.TriggerTypes.Tutorial:
                    {
                        PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.Tutorial;
                        PlayerInputManager.Instance.TutorialMenu.SetActive(true);
                    }
                    break;
                case Event.TriggerTypes.Encounter:
                    {
                        GameManager.Instance.FinishUnitTurn(GameManager.Instance._curUnit.gameObject.tag == "Player" ? 1 : 2);
                    }
                    break;
                case Event.TriggerTypes.UnitDefeat:
                    {
                        GameManager.Instance.FinishUnitTurn(GameManager.Instance._curUnit.gameObject.tag == "Player" ? 1 : 2);
                    }
                    break;
                case Event.TriggerTypes.GameState_End:
                    {
                        MissionManager.Instance.MissionWinEnd_AfterEndCutscene();
                    }
                    break;
                case Event.TriggerTypes.Story_Explain:
                    {
                        GameManager.Instance.MissionStart();
                    }
                    break;
            }
        }
    }

    public void PlayEventNextPart()
    {
        ++_curIndex;
        if (_curEvent != null)
        {

            if (_curIndex >= _curEvent._dialogueLines.Count)
            {
                StopEventPlay();
            }
            else
            {
                SoundManager.Instance.PlayDialogueAdvanceSound();
                PlayDialogue();
            }
        }
        else
        {
            StopEventPlay();
        }
    }

    public void SetAlpha(Image img, bool full)
    {
        Color temp = img.color;
        if (full)
            temp.a = 1;
        else
            temp.a = 0.5f;
        img.color = temp;
    }

    public void SetAlphaText(Text text, bool full)
    {
        Color temp = text.color;
        if (full)
            temp.a = 1;
        else
            temp.a = 0.7f;
        text.color = temp;
    }
    #endregion

    #region Triggered events

    /*
        0: Guard, // stay at spawn pt, attack enemies to come in range
        1: Random, // move around randomly
        2: Approach, // try to approach a certain target
        3: Patrol
        4: Retreat
    */
    public void ChangeEnemyAIPattern(int pattern)
    {
        foreach (ChrController chr in GameManager.Instance._enemies)
        {
            //chr._AIPattern = (ChrController.AIPatterns)pattern;
            chr.ChangeAIPattern(pattern);
        }
    }

    public void ChangeEnemyAIPattern_BossGuard(int pattern)
    {
        foreach (ChrController chr in GameManager.Instance._enemies)
        {
            if (chr._unitType == ChrController.UnitTypes.Enemy_Boss)
                chr.ChangeAIPattern(0);
            else
                chr.ChangeAIPattern(pattern);
        }
    }

    public void ChangeEnemyAIPattern_BossApproach(int pattern)
    {
        foreach (ChrController chr in GameManager.Instance._enemies)
        {
            if (chr._unitType == ChrController.UnitTypes.Enemy_Boss)
                chr.ChangeAIPattern(2);
            else
                chr.ChangeAIPattern(pattern);
        }
    }

    // 0: false 1: true
    public void AllEnemySpawnerControl(int value)
    {
        foreach (UnitSpawner spawner in MissionManager.Instance._spawners)
        {
            if (!spawner._playerSide)
                spawner._active = value == 0 ? false : true;
        }
    }

    public void AllEnemiesWeaken(int value)
    {
        foreach (ChrController chr in GameManager.Instance._enemies)
        {
            if (!chr._isDead)
            {
                chr.BuffApply(-5, -5, -5);
                if (chr._unitType == ChrController.UnitTypes.Enemy_Boss)
                {
                    chr.ChangeAIPattern(value);
                }
                else
                {
                    if (Random.Range(0, 10) > 3)
                        chr.ChangeAIPattern(1);
                }
            }
        }
    }

    public void AllEnemiesWeaken_BossUnaffected(int value)
    {
        foreach (ChrController chr in GameManager.Instance._enemies)
        {
            if (!chr._isDead)
            {
                if (chr._unitType != ChrController.UnitTypes.Enemy_Boss)
                {
                    chr.BuffApply(-5, -5, -5);
                    if (Random.Range(0, 10) > 3)
                        chr.ChangeAIPattern(1);
                }
            }
        }
    }

    public void AllEnemiesWeaken_MeleeCharge(int value)
    {
        foreach (ChrController chr in GameManager.Instance._enemies)
        {
            if (!chr._isDead)
            {
                if (chr._unitType != ChrController.UnitTypes.Enemy_Boss)
                {
                    chr.BuffApply(-5, -5, -5);
                    if (Random.Range(0, 10) > 3)
                        chr.ChangeAIPattern(1);
                    else if (chr._unitType == ChrController.UnitTypes.Enemy_Melee)
                        chr.ChangeAIPattern(2);
                }
                else
                {
                    chr.ChangeAIPattern(value);
                }
            }
        }
    }

    public void MeleeCharge(int value)
    {
        foreach (ChrController chr in GameManager.Instance._enemies)
        {
            if (!chr._isDead)
            {
                if (chr._unitType == ChrController.UnitTypes.Enemy_Boss || chr._unitType == ChrController.UnitTypes.Enemy_Melee)
                {
                    chr.ChangeAIPattern(value);
                }
            }
        }
    }

    public void ChangeEnemyBossPattern(int pattern)
    {
        foreach (ChrController chr in GameManager.Instance._enemies)
        {
            if (chr._unitType == ChrController.UnitTypes.Enemy_Boss)
                chr.ChangeAIPattern(pattern);
        }
    }

    public void ChangeTrueBossBehavior(int pattern)
    {
        ChrController chr = GameManager.Instance._enemies.Find(e => e._name == "Mercurius");
        //Debug.Log(chr);
        if (chr != null)
            chr.ChangeAIPattern(pattern);
    }

    #endregion
}
