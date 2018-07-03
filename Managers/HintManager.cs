using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : Singleton<HintManager>
{
    public bool _showHints;
    public float _hintChangeTime = 5.0f;

    public GameObject _hintLocation;

    /*
    0 - 15: Field Hints
    15~: Chr Selected/Path Hints
    */
    public List<GameObject> _hints = new List<GameObject>();

    private int _prev = -1;

    public void PlayNextHint()
    {
        if (_showHints && GameManager.Instance._playerPhase &&
            (PlayerInputManager.Instance._inputMode == PlayerInputManager.InputModes.ChrSelection
            || PlayerInputManager.Instance._inputMode == PlayerInputManager.InputModes.PathSelection)
            && !CombatManager.Instance._isOffensiveBattle
            && !CombatManager.Instance._isDefensiveBattle)
        {
            _hintLocation.SetActive(true);
            if (_prev != -1)
            {
                _hints[_prev].SetActive(false);
            }
            if (PlayerInputManager.Instance._inputMode == PlayerInputManager.InputModes.ChrSelection)
            {
                int index = Random.Range(0, 16);
                if (_hints[index] != null)
                {
                    _hints[index].SetActive(true);
                    _prev = index;
                }
                else
                {
                    _hintLocation.SetActive(false);
                }
            }
            else
            {
                int index = Random.Range(16, _hints.Count);
                if (_hints[index] != null)
                {
                    _hints[index].SetActive(true);
                    _prev = index;
                }
                else
                {
                    _hintLocation.SetActive(false);
                }
            }
        }
        else
        {
            _hintLocation.SetActive(false);
        }
    }

    public void StopHintPlay(bool permanent)
    {
        if (permanent)
            _showHints = false;
        _hintLocation.SetActive(false);
    }

    private void Start()
    {
        InvokeRepeating("PlayNextHint", _hintChangeTime, _hintChangeTime);
    }
}
