using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{

    public GameObject cursor;
    public List<Text> Optionlist;
    bool _canClick = true;
    public int num;

    public GameObject _mapGridObj;
    public GameObject _darkenImage;

    // Use this for initialization
    void Start()
    {
        num = 1;
        if (PlayerInputManager.Instance._Music)
        {
            Optionlist[0].text = "On";
        }
        else
        {
            Optionlist[0].text = "Off";
        }

        if (PlayerInputManager.Instance._Hint)
        {
            Optionlist[1].text = "On";
        }
        else
        {
            Optionlist[1].text = "Off";
        }

        if (PlayerInputManager.Instance._MapGrid)
        {
            Optionlist[2].text = "On";
        }
        else
        {
            Optionlist[2].text = "Off";
        }

        if (PlayerInputManager.Instance._ExtraPath)
        {
            Optionlist[3].text = "On";
        }
        else
        {
            Optionlist[3].text = "Off";
        }

        if (PlayerInputManager.Instance._EasyMode)
        {
            Optionlist[4].text = "On";
        }
        else
        {
            Optionlist[4].text = "Off";
        }

        if (PlayerInputManager.Instance._WeatherEffect)
        {
            Optionlist[5].text = "On";
        }
        else
        {
            Optionlist[5].text = "Off";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_canClick)
        {
            if (PlayerInputManager.Instance._inputMode == PlayerInputManager.InputModes.Option)
            {
                changeOption();
            }
        }
    }

    public void changeOption()
    {
        if (Input.GetAxis("Vertical") > 0 && num > 1)
        {
            num--;
            setcurserposition();
            PauseClick();
        }
        if (Input.GetAxis("Vertical") < 0 && num < Optionlist.Count)
        {
            num++;
            setcurserposition();
            PauseClick();
        }
        if (Input.GetAxis("V") > 0 && num > 1)
        {
            num--;
            setcurserposition();
            PauseClick();
        }
        if (Input.GetAxis("V") < 0 && num < Optionlist.Count)
        {
            num++;
            setcurserposition();
            PauseClick();
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            SwitchOnorOff();
            PauseClick();
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            SwitchOnorOff();
            PauseClick();
        }
        if (Input.GetAxis("H") > 0)
        {
            SwitchOnorOff();
            PauseClick();
        }
        if (Input.GetAxis("H") < 0)
        {
            SwitchOnorOff();
            PauseClick();
        }
    }


    private void PauseClick()
    {
        _canClick = false;
        Invoke("ResetClick", 0.3f);
    }

    private void ResetClick()
    {
        _canClick = true;
    }

    public void setcurserposition()
    {
        if (num > 0 && num <= Optionlist.Count)
        {
            cursor.SetActive(true);
            SoundManager.Instance.PlaySelectionChangeSound();
            cursor.transform.position = Optionlist[num - 1].transform.position;
        }
        else
        {
            cursor.SetActive(false);
        }
    }

    void SwitchOnorOff()
    {
        SoundManager.Instance.PlayConfirmation();
        switch (num)
        {
            case 1:
                if (PlayerInputManager.Instance._Music)
                {
                    PlayerInputManager.Instance._Music = false;
                    SoundManager.Instance.MuteAll();
                    Optionlist[num - 1].text = "Off";
                }
                else
                {
                    PlayerInputManager.Instance._Music = true;
                    SoundManager.Instance.UnMute();
                    Optionlist[num - 1].text = "On";
                }
                break;
            case 2:
                if (PlayerInputManager.Instance._Hint)
                {
                    PlayerInputManager.Instance._Hint = false;
                    HintManager.Instance.StopHintPlay(true);
                    Optionlist[num - 1].text = "Off";
                }
                else
                {
                    HintManager.Instance._showHints = true;
                    PlayerInputManager.Instance._Hint = true;
                    Optionlist[num - 1].text = "On";
                }
                break;
            case 3:
                if (PlayerInputManager.Instance._MapGrid)
                {
                    PlayerInputManager.Instance._MapGrid = false;
                    if (_mapGridObj != null)
                        _mapGridObj.SetActive(false);
                    Optionlist[num - 1].text = "Off";
                }
                else
                {
                    PlayerInputManager.Instance._MapGrid = true;
                    if (_mapGridObj != null)
                        _mapGridObj.SetActive(true);
                    Optionlist[num - 1].text = "On";
                }
                break;
            case 4:
                if (PlayerInputManager.Instance._ExtraPath)
                {
                    PlayerInputManager.Instance._ExtraPath = false;
                    AStarPathfinding.Instance._showAStarPath = false;
                    Optionlist[num - 1].text = "Off";
                }
                else
                {
                    PlayerInputManager.Instance._ExtraPath = true;
                    AStarPathfinding.Instance._showAStarPath = true;
                    Optionlist[num - 1].text = "On";
                }
                break;
            case 5:
                if (PlayerInputManager.Instance._EasyMode)
                {
                    PlayerInputManager.Instance._EasyMode = false;
                    CombatManager.Instance._easyMode = false;
                    Optionlist[num - 1].text = "Off";
                }
                else
                {
                    PlayerInputManager.Instance._EasyMode = true;
                    CombatManager.Instance._easyMode = true;
                    Optionlist[num - 1].text = "On";
                }
                break;
            case 6:
                if (PlayerInputManager.Instance._WeatherEffect)
                {
                    WeatherManager.Instance.DisableWeather();
                    PlayerInputManager.Instance._WeatherEffect = false;
                    Optionlist[num - 1].text = "Off";
                }
                else
                {
                    WeatherManager.Instance.EnableWeather();
                    PlayerInputManager.Instance._WeatherEffect = true;
                    Optionlist[num - 1].text = "On";
                }
                break;
        }

    }

    void OnEnable()
    {
        if (_darkenImage != null)
            _darkenImage.SetActive(true);
    }

    //void OnDisable()
    //{
    //    if (_darkenImage != null)
    //        _darkenImage.SetActive(false);
    //}
}
