using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WelcomeInput : MonoBehaviour
{

    public GameObject cursor;
    public List<GameObject> Buttonlist;
    public GameObject confirmation;
    public GameObject _screenDarken;
    bool confirmornot = false;
    bool _canClick = true;
    bool IsJoystick = false;
    public int num;

    public List<Image> Herolist;
    public int Heronum;
    public float displaySpeed;
    public GameObject light;

    public Vector3 mouseposition;

    // Use this for initialization
    void Start()
    {
        num = 1;
        SoundManager.Instance.PlayMusic(true, true, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CheckJoystick();
        if (_canClick)
        {
            ControllerToGridPos();
        }
        getButtondown();

        //showMainCharacter();
        //lightChange();
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
                        {
                            SceneManager.LoadScene("1");
                            SoundManager.Instance.PlayConfirmation();
                        }
                        break;
                    case 3:
                        {
                            SceneManager.LoadScene("0");
                            SoundManager.Instance.PlayConfirmation();
                        }
                        break;
                    case 4:
                        Application.Quit();
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
                if (num == 2)
                {
                    SoundManager.Instance.PlayConfirmation();
                    SceneManager.LoadScene("LevelSelect");
                    return;
                }
                SoundManager.Instance.PlayConfirmation();
                _screenDarken.gameObject.SetActive(true);
                confirmation.SetActive(true);
                confirmornot = true;
                PauseClick();
            }
        }

    }

    void setcurserposition()
    {
        if (num > 0 && num <= Buttonlist.Count)
        {
            SoundManager.Instance.PlaySelectionChangeSound();
            cursor.SetActive(true);
            cursor.transform.position = Buttonlist[num - 1].transform.position;
        }
        else
        {
            cursor.SetActive(false);
        }
    }

    public void MouseToButton(int inputnum)
    {
        if (!IsJoystick)
        {
            num = inputnum;
            setcurserposition();
            Debug.Log("Enter");
        }
    }

    void checkIfMouseOnButton()
    {
        mouseposition = Input.mousePosition;
        if (num > 0 && num <= Buttonlist.Count)
        {
            Vector2 buttonposition = Buttonlist[num - 1].GetComponent<RectTransform>().anchoredPosition;
            float width = Buttonlist[num - 1].GetComponent<RectTransform>().rect.width;
            float height = Buttonlist[num - 1].GetComponent<RectTransform>().rect.height;
            Vector2 buttonsize = new Vector2(width, height);
            buttonposition -= new Vector2(width * 0.5f, height * 0.5f);
            Rect buttonArea = new Rect(buttonposition, buttonsize);
            if (!buttonArea.Contains(Input.mousePosition))
            {
                num = 0;
                setcurserposition();
                Debug.Log("Exist");
            }
        }
    }

    public void ControllerToGridPos()
    {
        if (confirmornot)
        {
            return;
        }
        if (Input.GetAxis("Vertical") > 0 && num > 1)
        {
            num--;
            setcurserposition();
            PauseClick();
        }
        if (Input.GetAxis("Vertical") < 0 && num < Buttonlist.Count)
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
        if (Input.GetAxis("V") < 0 && num < Buttonlist.Count)
        {
            num++;
            setcurserposition();
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

    void showMainCharacter()
    {
        if (Herolist[Heronum].color.a < 1)
        {
            Herolist[Heronum].color += new Color(0, 0, 0, 0.1f) * displaySpeed;
            if (Heronum > 0)
            {
                Herolist[Heronum - 1].color -= new Color(0, 0, 0, 0.1f) * displaySpeed;
            }
            else if (Herolist[Herolist.Count - 1].color.a > 0)
            {
                Herolist[Herolist.Count - 1].color -= new Color(0, 0, 0, 0.1f) * displaySpeed;
            }
        }
        else
        {
            Heronum++;
            if (Heronum == Herolist.Count)
            {
                Heronum = 0;
            }
        }
    }

    void heronum0()
    {
        Heronum = 0;
    }

    void lightChange()
    {
        if (light.transform.rotation.x < 360)
        {
            light.transform.Rotate(1, 0, 0);
        }
        else
        {
            light.transform.rotation = Quaternion.Euler(-45, 180, 0);
        }
    }
}
