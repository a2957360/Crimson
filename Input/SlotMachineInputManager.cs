using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineInputManager : MonoBehaviour {

    public List<Sprite> pictures;
    public List<Image> Squares;
    public List<GameObject> Buttons;
    public List<int> GamblingResult;
    public GameObject Finish;

    public float slowTimeOne;
    public float slowTimeTwo;
    public float slowTimeThree;
    public float slowTimeIncreasement;
    int settednum;
    int clicknum;
    public int firstslow;
    public int Secondslow;
    public int Thirdslow;
    public int stopcounter;
    bool _canClick;
    bool _canScrollOne;
    bool _canScrollTwo;
    bool _canScrollThree;

    // Use this for initialization
    void Start () {
        stopcounter = 0;
        settednum = 1;
        clicknum = 1;
        GamblingResult = new List<int>();
        GetResult();
        _canClick = true;
        _canScrollOne = true;
        _canScrollTwo = true;
        _canScrollThree = true;
    }
	
	// Update is called once per frame
	void Update () {
        randomPicture();
        slowStop();
    }

    public void interSlotMachine()
    {
        if (settednum == 4)
        {
            Invoke("SetResult", 2f);
        }
        else
        {
            if (clicknum <= 3)
            {
                SetResult();
            }
        }
    }

    void randomPicture()
    {
        if (clicknum <= 1)
        {
            Squares[0].sprite = pictures[Random.Range(0, pictures.Count - 1)];
        }
        if (clicknum <= 2)
        {
            Squares[1].sprite = pictures[Random.Range(0, pictures.Count - 1)];
        }
        if (clicknum <= 3)
        {
            Squares[2].sprite = pictures[Random.Range(0, pictures.Count - 1)];
        }
    }

    void slowStop()
    {
        if (settednum <= 1 && clicknum > 1 && stopcounter == 0)
        {
            if (_canScrollOne)
            {
                Squares[0].sprite = pictures[(firstslow + GamblingResult[0]) % pictures.Count];
                firstslow--;
                slowTimeOne *= slowTimeIncreasement;
                if (firstslow < 1)
                {
                    settednum++;
                    slowTimeOne = 0.001f;
                }
                _canScrollOne = false;
                Delayscroll("ResetscrollOne", slowTimeOne);
            }

        }
        if (settednum <= 2 && clicknum > 2 && stopcounter == 1)
        {
            if (_canScrollTwo)
            {
                Squares[1].sprite = pictures[(Secondslow + GamblingResult[1]) % pictures.Count];
                Secondslow--;
                slowTimeTwo *= slowTimeIncreasement;
                if (Secondslow < 1)
                {
                    settednum++;
                    slowTimeTwo = 0.001f;
                }
                _canScrollTwo = false;
                Delayscroll("ResetscrollTwo", slowTimeTwo);
            }

        }
        if (settednum <= 3 && clicknum > 3 && stopcounter == 2)
        {
            if (_canScrollThree)
            {
                Squares[2].sprite = pictures[(Thirdslow + GamblingResult[2]) % pictures.Count];
                Thirdslow--;
                slowTimeThree *= slowTimeIncreasement;
                if (Thirdslow < 1)
                {
                    settednum++;
                    slowTimeThree = 0.001f;
                    Finish.SetActive(true);
                }
                _canScrollThree = false;
                Delayscroll("ResetscrollThree", slowTimeThree);
            }
        }

        stopcounter++;
        if (stopcounter > 2)
        {
            stopcounter = 0;
        }

    }

    private void Delayscroll(string name, float st)
    {
        Invoke(name, st);
    }

    private void ResetscrollOne()
    {
        _canScrollOne = true;
    }

    private void ResetscrollTwo()
    {
        _canScrollTwo = true;
    }

    private void ResetscrollThree()
    {
        _canScrollThree = true;
    }

    void GetResult()
    {
        int samenum = Random.Range(0, pictures.Count - 1);
        int differentnum = Random.Range(0, pictures.Count - 1);
        if (differentnum == samenum)
        {
            differentnum = Random.Range(0, pictures.Count - 1);
        }
        int differetSquare = Random.Range(0, Squares.Count - 1);
        for (int i = 0; i < Squares.Count; i++)
        {
            if (i != differetSquare)
            {
                GamblingResult.Add(samenum);
            }
            else
            {
                GamblingResult.Add(differentnum);
            }
        }
    }

    void SetResult()
    {
        if (settednum <= 3 && settednum >= 1)
        {
            Buttons[clicknum - 1].SetActive(false);
            clicknum++;
            PauseClick();
        }
        else
        {
            if (settednum == 4)
            {
                PlayerInputManager.Instance._inputMode = PlayerInputManager.InputModes.ChrSelection;
                this.gameObject.SetActive(false);
            }
        }
    }

    public void restart()
    {
        settednum = 1;
        clicknum = 1;
        firstslow = 37;
        Secondslow = 37;
        Thirdslow = 37;
        GamblingResult.Clear();
        GetResult();
        for (int i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].SetActive(true);
            Buttons[i].SetActive(true);
            Buttons[i].SetActive(true);
        }
        Finish.SetActive(false);
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
}
