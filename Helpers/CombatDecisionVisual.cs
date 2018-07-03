using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatDecisionVisual : MonoBehaviour
{
    public Image _LeftBG;
    public Image _MiddleBG;
    public Image _RightBG;
    public Image _Advantage;
    public Image _Disadvantage;

    public Image _Power;
    public Image _Pre;
    public Image _Tech;
    public Image _Counter;
    public Image _Dodge;
    public Image _Block;

    public GameObject AtkPosition;
    public GameObject HighBG;
    public GameObject MidBG;
    public GameObject LowBG;

    public GameObject HeadA;
    public GameObject BodyA;
    public GameObject LegA;
    public GameObject HeadD;
    public GameObject BodyD;
    public GameObject LegD;

    public Text _locationText;
    public Text _timingText;


    public void StartDecisionVisual(bool offensive)
    {
        gameObject.SetActive(true);
        if (offensive)
        {
            HeadA.GetComponentInChildren<Image>().sprite = GameManager.Instance._curUnit._icon;
            BodyA.GetComponentInChildren<Image>().sprite = GameManager.Instance._curUnit._icon;
            LegA.GetComponentInChildren<Image>().sprite = GameManager.Instance._curUnit._icon;
            _LeftBG.sprite = _Tech.sprite;
            _MiddleBG.sprite = _Pre.sprite;
            _RightBG.sprite = _Power.sprite;
            _Advantage.sprite = _Dodge.sprite;
            _Disadvantage.sprite = _Block.sprite;
            BodyA.SetActive(true);
        }
        else
        {
            _LeftBG.sprite = _Dodge.sprite;
            _MiddleBG.sprite = _Counter.sprite;
            _RightBG.sprite = _Block.sprite;
            _Advantage.sprite = _Tech.sprite;
            _Disadvantage.sprite = _Power.sprite;
            BodyD.SetActive(true);
        }
        AtkPosition.SetActive(true);
        MidBG.SetActive(true);
        CombatManager.Instance._locationDecisionValue = 0;
        CombatManager.Instance.DisplayPlayerActionName();
    }

    public void DisplayLocation(int val)
    {
        if (CombatManager.Instance._battleType == CombatManager.BattleTypes.Offensive)
        {
            switch (val)
            {
                case 0:
                    {
                        HeadA.SetActive(false);
                        BodyA.SetActive(true);
                        LegA.SetActive(false);
                        HeadD.SetActive(false);
                        BodyD.SetActive(false);
                        LegD.SetActive(false);
                        HighBG.SetActive(false);
                        MidBG.SetActive(true);
                        LowBG.SetActive(false);
                    }
                    break;
                case -1:
                    {
                        HeadA.SetActive(false);
                        BodyA.SetActive(false);
                        LegA.SetActive(true);
                        HeadD.SetActive(false);
                        BodyD.SetActive(false);
                        LegD.SetActive(false);
                        HighBG.SetActive(false);
                        MidBG.SetActive(false);
                        LowBG.SetActive(true);
                    }
                    break;
                case 1:
                    {
                        HeadA.SetActive(true);
                        BodyA.SetActive(false);
                        LegA.SetActive(false);
                        HeadD.SetActive(false);
                        BodyD.SetActive(false);
                        LegD.SetActive(false);
                        HighBG.SetActive(true);
                        MidBG.SetActive(false);
                        LowBG.SetActive(false);
                    }
                    break;
            }
        }
        else
        {
            switch (val)
            {
                case 0:
                    {
                        HeadA.SetActive(false);
                        BodyA.SetActive(false);
                        LegA.SetActive(false);
                        HeadD.SetActive(false);
                        BodyD.SetActive(true);
                        LegD.SetActive(false);

                        HighBG.SetActive(false);
                        MidBG.SetActive(true);
                        LowBG.SetActive(false);
                    }
                    break;
                case -1:
                    {
                        HeadA.SetActive(false);
                        BodyA.SetActive(false);
                        LegA.SetActive(false);
                        HeadD.SetActive(false);
                        BodyD.SetActive(false);
                        LegD.SetActive(true);

                        HighBG.SetActive(false);
                        MidBG.SetActive(false);
                        LowBG.SetActive(true);
                    }
                    break;
                case 1:
                    {
                        HeadA.SetActive(false);
                        BodyA.SetActive(false);
                        LegA.SetActive(false);
                        HeadD.SetActive(true);
                        BodyD.SetActive(false);
                        LegD.SetActive(false);

                        HighBG.SetActive(true);
                        MidBG.SetActive(false);
                        LowBG.SetActive(false);
                    }
                    break;
            }
        }
    }

    public void DisplayTiming(int val)
    {
        if (_timingText != null)
        {
            switch (val)
            {
                case 0:
                    {
                        _timingText.text = "SAME";
                    }
                    break;
                case -1:
                    {
                        _timingText.text = "SLOWER";
                    }
                    break;
                case 1:
                    {
                        _timingText.text = "FASTER";
                    }
                    break;
            }
        }
    }

    public void DisplayDecisionVisual(bool isActive, bool offensive, int decision)
    {
        if (!isActive)
        {
            _LeftBG.sprite = null;
            _MiddleBG.sprite = null;
            _RightBG.sprite = null;
            BodyA.SetActive(false);
            HeadA.SetActive(false);
            LegA.SetActive(false);
            BodyD.SetActive(false);
            HeadD.SetActive(false);
            LegD.SetActive(false);
            AtkPosition.SetActive(false);
            HighBG.SetActive(false);
            MidBG.SetActive(false);
            LowBG.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            //gameObject.SetActive(true);
            if (offensive)
            {
                switch (decision)
                {
                    case 0:
                        {
                            _LeftBG.sprite = _Power.sprite;
                            _MiddleBG.sprite = _Tech.sprite;
                            _RightBG.sprite = _Pre.sprite;
                            _Advantage.sprite = _Block.sprite;
                            _Disadvantage.sprite = _Counter.sprite;
                        }
                        break;
                    case -1:
                        {
                            _LeftBG.sprite = _Tech.sprite;
                            _MiddleBG.sprite = _Pre.sprite;
                            _RightBG.sprite = _Power.sprite;
                            _Advantage.sprite = _Dodge.sprite;
                            _Disadvantage.sprite = _Block.sprite;
                        }
                        break;
                    case 1:
                        {
                            _LeftBG.sprite = _Pre.sprite;
                            _MiddleBG.sprite = _Power.sprite;
                            _RightBG.sprite = _Tech.sprite;
                            _Advantage.sprite = _Counter.sprite;
                            _Disadvantage.sprite = _Dodge.sprite;
                        }
                        break;
                }
            }
            else
            {
                switch (decision)
                {
                    case 0:
                        {
                            _LeftBG.sprite = _Block.sprite;
                            _MiddleBG.sprite = _Dodge.sprite;
                            _RightBG.sprite = _Counter.sprite;
                            _Advantage.sprite = _Power.sprite;
                            _Disadvantage.sprite = _Pre.sprite;
                        }
                        break;
                    case -1:
                        {
                            _LeftBG.sprite = _Dodge.sprite;
                            _MiddleBG.sprite = _Counter.sprite;
                            _RightBG.sprite = _Block.sprite;
                            _Advantage.sprite = _Tech.sprite;
                            _Disadvantage.sprite = _Power.sprite;
                        }
                        break;
                    case 1:
                        {
                            _LeftBG.sprite = _Counter.sprite;
                            _MiddleBG.sprite = _Block.sprite;
                            _RightBG.sprite = _Dodge.sprite;
                            _Advantage.sprite = _Pre.sprite;
                            _Disadvantage.sprite = _Tech.sprite;
                        }
                        break;
                }
            }
        }
    }

}
