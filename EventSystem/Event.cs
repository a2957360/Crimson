using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    // each Dialogue consists of a speaker, line, and on which side of the screen is the dialogue happening
    [System.Serializable]
    public class Dialogue
    {
        [Header("Event")]
        // show a image/richText on screen, such as for 
        public bool _isImage = false;
        public bool _leftSide;
        public int _speakerNum;
        public int _line;
        [Space(10)]

        [Header("Camera")]
        // if _x and _y != -1, then focus camera on that loc
        public int _x = -1;
        public int _y = -1;

        // if _targetName set, then try to find unit then focus camera on it
        public string _targetName = "";
        [Space(10)]

        [Header("Extra")]
        // does this dialogue also trigger an event/script
        public bool _sendEvent = false;
        public int _BG;
        //[Space (10)]
    }

    #region Variables

    [Header("Basic")]
    public string _Id = "";

    public enum TriggerTypes : int
    {
        Battle,
        Special,
        Calendar,
        GameState,
        BattleTutorial,
        MissionWin,
        MissionLoss,
        Spawn,
        Encounter,
        Tutorial,
        UnitDefeat,
        GameState_End,
        Story_Explain,
    };
    public TriggerTypes _trigger = TriggerTypes.Special;

    // if true, then this event requires button press to advance dialogue
    public bool _waitMode = false;

    // _mission is used for Calendar and GameState
    public int _mission = 0;

    public bool _useTutorialImages = false;
    [Space(10)]

    [Header("Battle Tell")]

    /*
     * For battle trigger type:
     * 1 : Power Attack
     * 2 : Technique Attack
     * 3 : Precision Attack
     * 4 : Guard
     * 5 : Dodge
     * 6 : Counter
     */
    public int _battleType = 0;

    /*
     * For battle trigger type:
     * 1 : Easy (Enemy just tell you their move)
     * 2 : Medium (Enemy will give u a hint of their move)
     * 3 : Hard (enemy will not give any clue, or say random things)
     * */
    public int _battleDiffculty = 0;

    /* 
     * 1 : High attack or guessing opponent high attack
     * 0 : Med attack or guessing opponent med attack
     * -1 : Low attack or guessing opponent low attack
     * */
    public int _battleTargeting = 0;

    public bool _isRandom = false;
    public bool _isRed = false;
    public bool _isGreen = false;
    public bool _isBlue = false;

    [Space(10)]

    [Header("Battle Outcome/Death")]

    /*
     * For battle trigger type:
     * 1 : Light (little or no damage suffered)
     * 2 : Damage (suffered damage)
     * 3 : Death
     * */
    public int _battleOutomeType = 0;
    public int _battleOutcomeSubType = 0;
    public bool _leftSide = false;

    [Space(10)]

    [Header("Special")]

    // event can only occur if _phaseLimit < _phase
    // for Special and Calendar
    public bool _hasPhaseLimit = false;
    public int _phaseLimit = 0;
    // how much phase (in mission manager) does this event increases
    public int _phaseChange = 0;

    [Space(10)]

    [Header("Calendar")]

    // which round does this event occur
    // this number should be >= 2, a calendar event on first round would cause issues
    public int _round = 2;

    [Space(10)]

    [Header("GameState")]

    /*    
     * For GameState: 
     * 0 : Beginning of Mission
     * 1 : End of Mission
     * */
    public int _gameStateType = 0;

    [Space(10)]

    [Header("Script Target")]

    // For send msg to a function in event manager
    public List<string> _targetFunc = new List<string>();
    public List<int> _funcPara = new List<int>();

    [Space(10)]

    [Header("Dialogue")]
    public List<string> _lines;
    public List<Dialogue> _dialogueLines;
    #endregion

}
