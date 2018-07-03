using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuInput : MonoBehaviour
{
    public GameObject cursor;
    public GameObject Confirm;
    public GameObject _screenDarken;
    public List<GameObject> Buttonlist;
    bool _canClick = true;
    bool IsJoystick = false;
    public int num = 1;
    public int disableButton = 0;

    public Vector3 mouseposition;
    public GameObject _darkenImage;

    // Update is called once per frame
    void Update()
    {
        CheckJoystick();
        if (_canClick)
        {
            if (IsJoystick && PlayerInputManager.Instance._inputMode != PlayerInputManager.InputModes.MainMenuConfirm)
            {
                ControllerToGridPos();
            }
        }
    }

    public void setcurserposition()
    {
        if (num > 0 && num <= Buttonlist.Count)
        {
            cursor.SetActive(true);
            cursor.transform.position = Buttonlist[num - 1].transform.position;
            if (PlayerInputManager.Instance._inputMode == PlayerInputManager.InputModes.MainMenu)
            {
                SoundManager.Instance.PlaySelectionChangeSound();
            }
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
        if (Input.GetAxis("Vertical") > 0 && num > 1 + disableButton)
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
        if (Input.GetAxis("V") > 0 && num > 1 + disableButton)
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
        Invoke("ResetClick", 0.2f);
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

    void OnEnable()
    {
        if (_darkenImage != null)
            _darkenImage.SetActive(true);
    }

    void OnDisable()
    {
        if (_darkenImage != null)
            _darkenImage.SetActive(false);
    }

    public void disableSlection()
    {
        for (int i = 0; i < disableButton; i++)
        {
            Buttonlist[i].GetComponent<Image>().enabled = true;
            Buttonlist[i].GetComponent<Image>().color = Color.red;
        }
    }

    public void showSlection()
    {
        for (int i = 0; i < disableButton; i++)
        {
            Buttonlist[i].GetComponent<Image>().enabled = false;
            Buttonlist[i].GetComponent<Image>().color = Color.black;
        }
    }
}
