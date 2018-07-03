using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectInput : MonoBehaviour
{

    [System.Serializable]
    public class Mission
    {
        public string Name;
        public Sprite Picture;
        public string Description;
    }

    public List<Mission> Missionlist;
    public GameObject confirmation;
    public GameObject _screenDarken;
    public GameObject LeftArrow;
    public GameObject RightArrow;
    public Text MissionName;
    public Image MissionPiture;
    public Text MissionDescription;

    bool confirmornot = false;
    bool _canClick = true;
    bool _canMove = true;
    bool IsJoystick = false;
    public int num;

    public Vector3 mouseposition;

    // Use this for initialization
    void Start()
    {
        SoundManager.Instance.PlayMusic(true, true, 1);
    }

    // Update is called once per frame
    void Update()
    {
        checkArrow();
        CheckJoystick();
        if (_canMove)
        {
            ControllerToGridPos();
        }

        if (_canClick)
        {
            getButtondown();
        }
    }

    void getButtondown()
    {
        if (confirmornot)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                switch (num)
                {
                    case 1:
                        SoundManager.Instance.PlayConfirmation();
                        SceneManager.LoadScene("1");
                        break;
                    case 2:
                        SoundManager.Instance.PlayConfirmation();
                        SceneManager.LoadScene("2");
                        break;
                    case 3:
                        SoundManager.Instance.PlayConfirmation();
                        SceneManager.LoadScene("3");
                        break;
                    case 4:
                        SoundManager.Instance.PlayConfirmation();
                        SceneManager.LoadScene("4");
                        break;
                }
            }
            if (Input.GetButtonDown("Fire2"))
            {
                _screenDarken.gameObject.SetActive(false);
                confirmation.SetActive(false);
                confirmornot = false;
                PauseClick();
            }

        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                SoundManager.Instance.PlayConfirmation();
                _screenDarken.gameObject.SetActive(true);
                confirmation.SetActive(true);
                confirmornot = true;
                PauseClick();
            }
            if (Input.GetButtonDown("Fire2"))
            {
                SceneManager.LoadScene("Welcome");
            }
        }

    }

    void setcurserposition()
    {
        SoundManager.Instance.PlaySelectionChangeSound();
        MissionName.text = Missionlist[num - 1].Name;
        MissionPiture.sprite = Missionlist[num - 1].Picture;
        MissionDescription.text = Missionlist[num - 1].Description;
    }


    public void ControllerToGridPos()
    {
        if (confirmornot)
        {
            return;
        }
        if (Input.GetAxis("Horizontal") < 0 && num > 1)
        {
            num--;
            setcurserposition();
            PauseMove();
        }
        if (Input.GetAxis("Horizontal") > 0 && num < Missionlist.Count)
        {
            num++;
            setcurserposition();
            PauseMove();
        }
        if (Input.GetAxis("H") < 0 && num > 1)
        {
            num--;
            setcurserposition();
            PauseMove();
        }
        if (Input.GetAxis("H") > 0 && num < Missionlist.Count)
        {
            num++;
            setcurserposition();
            PauseMove();
        }
    }

    private void PauseClick()
    {
        _canClick = false;
        Invoke("ResetClick", 0.5f);
    }

    private void PauseMove()
    {
        _canMove = false;
        Invoke("ResetMove", 0.3f);
    }

    private void ResetClick()
    {
        _canClick = true;
    }

    private void ResetMove()
    {
        _canMove = true;
    }

    void checkArrow()
    {
        if (num == 1)
        {
            LeftArrow.SetActive(false);
            RightArrow.SetActive(true);
        }
        else if (num == Missionlist.Count)
        {
            LeftArrow.SetActive(true);
            RightArrow.SetActive(false);
        }
        else
        {
            LeftArrow.SetActive(true);
            RightArrow.SetActive(true);
        }
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
}
