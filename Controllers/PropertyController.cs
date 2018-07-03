using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyController : MonoBehaviour
{

    public bool isPlayerBar;
    public GameObject WholeUI;
    public Image Photo;
    public Text Name;
    public Text HP;
    public Image HPBar;
    public Text Atk;
    public Text Def;
    public Text Agl;
    public Text Mov;
    public Text AtkText;
    public Text DefText;
    public Text AglText;
    public Text MovText;
    public Image RoleIcon;
    public GameObject AtkUp;
    public GameObject DefUp;
    public GameObject AglUp;
    public GameObject AtkDown;
    public GameObject DefDown;
    public GameObject AglDown;
    public GameObject Power;
    public GameObject Precision;
    public GameObject Technique;
    public GameObject Atkrandom;
    public GameObject Block;
    public GameObject Counter;
    public GameObject Dodge;
    public GameObject Defrandom;
    public GameObject BattleIcon;
    public Text BattleIconText;

    float tempHp = 0;
    string ChrName = "";
    float delayHpTimer = 0.4f;


    void setproporty()
    {
        if (isPlayerBar)
        {
            ChrController curchr = null;
            if (GameManager.Instance._playerPhase)
            {
                curchr = GameManager.Instance._curUnit;
            }
            else
            {
                curchr = GameManager.Instance._curPlayerEnemy;
            }
            if (GameManager.Instance._playerPhase && curchr != null)
            {
                if (CombatManager.Instance._progress == CombatManager.BattleProgress.None && (curchr.gameObject.tag == "Enemy" || curchr._turn >= 200))
                {
                    curchr = null;
                }
            }
            if (curchr == null && GameManager.Instance._playerPhase && PlayerInputManager.Instance._HeroCursorHover != null)
                curchr = PlayerInputManager.Instance._HeroCursorHover;
            if (curchr != null)
            {
                WholeUI.SetActive(true);
                Photo.sprite = curchr._portrait;
                Name.text = curchr._name;

                HP.text = curchr._HP_Display.ToString();
                HPBar.fillAmount = (float)curchr._HP_Display / (float)curchr._maxHP;


                //Atk.text = curchr._Attack.ToString();
                //Def.text = curchr._Defense.ToString();
                //Agl.text = curchr._Agility.ToString();
                //Mov.text = curchr._MovementRange.ToString();
                UpdateStatsAndSetColor(curchr);
                RoleIcon.sprite = curchr._icon;
                if (curchr._Brave > 0)
                {
                    AtkUp.SetActive(true);
                }
                else
                {
                    AtkUp.SetActive(false);
                }
                if (curchr._Brave < 0)
                {
                    AtkDown.SetActive(true);
                }
                else
                {
                    AtkDown.SetActive(false);
                }
                if (curchr._Protect > 0)
                {
                    DefUp.SetActive(true);
                }
                else
                {
                    DefUp.SetActive(false);
                }
                if (curchr._Protect < 0)
                {
                    DefDown.SetActive(true);
                }
                else
                {
                    DefDown.SetActive(false);
                }
                if (curchr._Speed > 0)
                {
                    AglUp.SetActive(true);
                }
                else
                {
                    AglUp.SetActive(false);
                }
                if (curchr._Speed < 0)
                {
                    AglDown.SetActive(true);
                }
                else
                {
                    AglDown.SetActive(false);
                }
                if (CombatManager.Instance._isOffensiveBattle == true)
                {
                    if (CombatManager.Instance._isBattleSelectPhase == false)
                    {
                        BattleIconText.text = "";
                        BattleIcon.SetActive(false);
                        return;
                    }
                    BattleIcon.SetActive(true);
                    Power.SetActive(false);
                    Precision.SetActive(false);
                    Technique.SetActive(false);
                    Atkrandom.SetActive(false);
                    Block.SetActive(false);
                    Counter.SetActive(false);
                    Dodge.SetActive(false);
                    Defrandom.SetActive(false);
                    BattleIconText.text = "Favorite\nOffense";
                    switch (curchr._Fav_Offensive)
                    {
                        case ChrController.Fav_Offensives.Power:
                            BattleIcon.GetComponent<Image>().sprite = Power.GetComponent<Image>().sprite;
                            break;
                        case ChrController.Fav_Offensives.Precision:
                            BattleIcon.GetComponent<Image>().sprite = Precision.GetComponent<Image>().sprite;
                            break;
                        case ChrController.Fav_Offensives.Technical:
                            BattleIcon.GetComponent<Image>().sprite = Technique.GetComponent<Image>().sprite;
                            break;
                        case ChrController.Fav_Offensives.Random:
                            BattleIcon.GetComponent<Image>().sprite = Atkrandom.GetComponent<Image>().sprite;
                            break;
                    }
                }
                else if (CombatManager.Instance._isDefensiveBattle == true)
                {
                    if (CombatManager.Instance._isBattleSelectPhase == false)
                    {
                        BattleIconText.text = "";
                        BattleIcon.SetActive(false);
                        return;
                    }
                    BattleIcon.SetActive(true);
                    Power.SetActive(false);
                    Precision.SetActive(false);
                    Technique.SetActive(false);
                    Atkrandom.SetActive(false);
                    Block.SetActive(false);
                    Counter.SetActive(false);
                    Dodge.SetActive(false);
                    Defrandom.SetActive(false);
                    BattleIconText.text = "Favorite\nDefense";
                    switch (curchr._Fav_Defense)
                    {
                        case ChrController.Fav_Defenses.Guard:
                            BattleIcon.GetComponent<Image>().sprite = Block.GetComponent<Image>().sprite;
                            break;
                        case ChrController.Fav_Defenses.Counter:
                            BattleIcon.GetComponent<Image>().sprite = Counter.GetComponent<Image>().sprite;
                            break;
                        case ChrController.Fav_Defenses.Dodge:
                            BattleIcon.GetComponent<Image>().sprite = Dodge.GetComponent<Image>().sprite;
                            break;
                        case ChrController.Fav_Defenses.Random:
                            BattleIcon.GetComponent<Image>().sprite = Defrandom.GetComponent<Image>().sprite;
                            break;
                    }
                }
                else
                {
                    BattleIcon.SetActive(false);
                    BattleIconText.text = "Favorite\nTactics";
                    switch (curchr._Fav_Offensive)
                    {
                        case ChrController.Fav_Offensives.Power:
                            Power.SetActive(true);
                            Precision.SetActive(false);
                            Technique.SetActive(false);
                            Atkrandom.SetActive(false);
                            break;
                        case ChrController.Fav_Offensives.Precision:
                            Power.SetActive(false);
                            Precision.SetActive(true);
                            Technique.SetActive(false);
                            Atkrandom.SetActive(false);
                            break;
                        case ChrController.Fav_Offensives.Technical:
                            Power.SetActive(false);
                            Precision.SetActive(false);
                            Technique.SetActive(true);
                            Atkrandom.SetActive(false);
                            break;
                        case ChrController.Fav_Offensives.Random:
                            Power.SetActive(false);
                            Precision.SetActive(false);
                            Technique.SetActive(false);
                            Atkrandom.SetActive(true);
                            break;
                    }
                    switch (curchr._Fav_Defense)
                    {
                        case ChrController.Fav_Defenses.Guard:
                            Block.SetActive(true);
                            Counter.SetActive(false);
                            Dodge.SetActive(false);
                            Defrandom.SetActive(false);
                            break;
                        case ChrController.Fav_Defenses.Counter:
                            Block.SetActive(false);
                            Counter.SetActive(true);
                            Dodge.SetActive(false);
                            Defrandom.SetActive(false);
                            break;
                        case ChrController.Fav_Defenses.Dodge:
                            Block.SetActive(false);
                            Counter.SetActive(false);
                            Dodge.SetActive(true);
                            Defrandom.SetActive(false);
                            break;
                        case ChrController.Fav_Defenses.Random:
                            Block.SetActive(false);
                            Counter.SetActive(false);
                            Dodge.SetActive(false);
                            Defrandom.SetActive(true);
                            break;
                    }
                }
            }
            else
            {
                WholeUI.SetActive(false);
            }
        }
        else
        {
            ChrController curchr = null;
            if (GameManager.Instance._playerPhase)
            {
                curchr = GameManager.Instance._curEnemy;
                if (curchr == null && PlayerInputManager.Instance._EnemyCursorHover != null)
                    curchr = PlayerInputManager.Instance._EnemyCursorHover;
            }
            else if (GameManager.Instance._enemyPhase)
            {
                curchr = GameManager.Instance._curUnit;
            }
            if (curchr != null)
            {
                WholeUI.SetActive(true);
                Photo.sprite = curchr._portrait;
                Name.text = curchr._name;

                HP.text = curchr._HP_Display.ToString();
                HPBar.fillAmount = (float)curchr._HP_Display / (float)curchr._maxHP;
                //if (delayHpTimer > 0)
                //{
                //    delayHpTimer -= Time.deltaTime;
                //    return;
                //}
                //else
                //{
                //    HP.text = curchr._HP_Display.ToString();
                //    HPBar.fillAmount = curchr._HP_Display / curchr._maxHP;
                //    delayHpTimer = 0.4f;
                //}
                //Atk.text = curchr._Attack.ToString();
                //Def.text = curchr._Defense.ToString();
                //Agl.text = curchr._Agility.ToString();
                //Mov.text = curchr._MovementRange.ToString();
                UpdateStatsAndSetColor(curchr);
                RoleIcon.sprite = curchr._icon;
                if (curchr._Brave > 0)
                {
                    AtkUp.SetActive(true);
                }
                else
                {
                    AtkUp.SetActive(false);
                }
                if (curchr._Brave < 0)
                {
                    AtkDown.SetActive(true);
                }
                else
                {
                    AtkDown.SetActive(false);
                }
                if (curchr._Protect > 0)
                {
                    DefUp.SetActive(true);
                }
                else
                {
                    DefUp.SetActive(false);
                }
                if (curchr._Protect < 0)
                {
                    DefDown.SetActive(true);
                }
                else
                {
                    DefDown.SetActive(false);
                }
                if (curchr._Speed > 0)
                {
                    AglUp.SetActive(true);
                }
                else
                {
                    AglUp.SetActive(false);
                }
                if (curchr._Speed < 0)
                {
                    AglDown.SetActive(true);
                }
                else
                {
                    AglDown.SetActive(false);
                }
                if (CombatManager.Instance._isOffensiveBattle == true)
                {
                    if (CombatManager.Instance._isBattleSelectPhase == false)
                    {
                        BattleIconText.text = "";
                        BattleIcon.SetActive(false);
                        return;
                    }
                    BattleIcon.SetActive(true);
                    Power.SetActive(false);
                    Precision.SetActive(false);
                    Technique.SetActive(false);
                    Atkrandom.SetActive(false);
                    Block.SetActive(false);
                    Counter.SetActive(false);
                    Dodge.SetActive(false);
                    Defrandom.SetActive(false);
                    BattleIconText.text = "Favorite\nDefense";
                    switch (curchr._Fav_Defense)
                    {
                        case ChrController.Fav_Defenses.Guard:
                            BattleIcon.GetComponent<Image>().sprite = Block.GetComponent<Image>().sprite;
                            break;
                        case ChrController.Fav_Defenses.Counter:
                            BattleIcon.GetComponent<Image>().sprite = Counter.GetComponent<Image>().sprite;
                            break;
                        case ChrController.Fav_Defenses.Dodge:
                            BattleIcon.GetComponent<Image>().sprite = Dodge.GetComponent<Image>().sprite;
                            break;
                        case ChrController.Fav_Defenses.Random:
                            BattleIcon.GetComponent<Image>().sprite = Defrandom.GetComponent<Image>().sprite;
                            break;
                    }
                }
                else if (CombatManager.Instance._isDefensiveBattle == true)
                {
                    if (CombatManager.Instance._isBattleSelectPhase == false)
                    {
                        BattleIconText.text = "";
                        BattleIcon.SetActive(false);
                        return;
                    }
                    BattleIcon.SetActive(true);
                    Power.SetActive(false);
                    Precision.SetActive(false);
                    Technique.SetActive(false);
                    Atkrandom.SetActive(false);
                    Block.SetActive(false);
                    Counter.SetActive(false);
                    Dodge.SetActive(false);
                    Defrandom.SetActive(false);
                    BattleIconText.text = "Favorite\nOffense";
                    switch (curchr._Fav_Offensive)
                    {
                        case ChrController.Fav_Offensives.Power:
                            BattleIcon.GetComponent<Image>().sprite = Power.GetComponent<Image>().sprite;
                            break;
                        case ChrController.Fav_Offensives.Precision:
                            BattleIcon.GetComponent<Image>().sprite = Precision.GetComponent<Image>().sprite;
                            break;
                        case ChrController.Fav_Offensives.Technical:
                            BattleIcon.GetComponent<Image>().sprite = Technique.GetComponent<Image>().sprite;
                            break;
                        case ChrController.Fav_Offensives.Random:
                            BattleIcon.GetComponent<Image>().sprite = Atkrandom.GetComponent<Image>().sprite;
                            break;
                    }
                }
                else
                {
                    BattleIcon.SetActive(false);
                    BattleIconText.text = "Favorite\nTactics";
                    switch (curchr._Fav_Offensive)
                    {
                        case ChrController.Fav_Offensives.Power:
                            Power.SetActive(true);
                            Precision.SetActive(false);
                            Technique.SetActive(false);
                            Atkrandom.SetActive(false);
                            break;
                        case ChrController.Fav_Offensives.Precision:
                            Power.SetActive(false);
                            Precision.SetActive(true);
                            Technique.SetActive(false);
                            Atkrandom.SetActive(false);
                            break;
                        case ChrController.Fav_Offensives.Technical:
                            Power.SetActive(false);
                            Precision.SetActive(false);
                            Technique.SetActive(true);
                            Atkrandom.SetActive(false);
                            break;
                        case ChrController.Fav_Offensives.Random:
                            Power.SetActive(false);
                            Precision.SetActive(false);
                            Technique.SetActive(false);
                            Atkrandom.SetActive(true);
                            break;
                    }
                    switch (curchr._Fav_Defense)
                    {
                        case ChrController.Fav_Defenses.Guard:
                            Block.SetActive(true);
                            Counter.SetActive(false);
                            Dodge.SetActive(false);
                            Defrandom.SetActive(false);
                            break;
                        case ChrController.Fav_Defenses.Counter:
                            Block.SetActive(false);
                            Counter.SetActive(true);
                            Dodge.SetActive(false);
                            Defrandom.SetActive(false);
                            break;
                        case ChrController.Fav_Defenses.Dodge:
                            Block.SetActive(false);
                            Counter.SetActive(false);
                            Dodge.SetActive(true);
                            Defrandom.SetActive(false);
                            break;
                        case ChrController.Fav_Defenses.Random:
                            Block.SetActive(false);
                            Counter.SetActive(false);
                            Dodge.SetActive(false);
                            Defrandom.SetActive(true);
                            break;
                    }
                }
            }
            else
            {
                WholeUI.SetActive(false);
            }
        }

    }

    void Start()
    {
        InvokeRepeating("setproporty", 2.0f, 0.05f);
        //AtkUp.SetActive(false);
        //DefUp.SetActive(false);
        //AglUp.SetActive(false);
    }

    public void UpdateStatsAndSetColor(ChrController chr)
    {
        if (chr._Attack > chr._BaseAttack)
        {
            Atk.color = Color.green;
            AtkText.color = Color.green;
        }
        else if (chr._Attack < chr._BaseAttack)
        {
            Atk.color = Color.red;
            AtkText.color = Color.red;
        }
        else
        {
            Atk.color = Color.black;
            AtkText.color = Color.black;
        }

        if (chr._Defense > chr._BaseDefense)
        {
            Def.color = Color.green;
            DefText.color = Color.green;
        }
        else if (chr._Defense < chr._BaseDefense)
        {
            Def.color = Color.red;
            DefText.color = Color.red;
        }
        else
        {
            Def.color = Color.black;
            DefText.color = Color.black;
        }

        if (chr._Agility > chr._BaseAgility)
        {
            Agl.color = Color.green;
            AglText.color = Color.green;
        }
        else if (chr._Agility < chr._BaseAgility)
        {
            Agl.color = Color.red;
            AglText.color = Color.red;
        }
        else
        {
            Agl.color = Color.black;
            AglText.color = Color.black;
        }

        if (chr._MovementRange > chr._BaseMovementRange)
        {
            Mov.color = Color.green;
            MovText.color = Color.green;
        }
        else if (chr._MovementRange < chr._BaseMovementRange)
        {
            Mov.color = Color.red;
            MovText.color = Color.red;
        }
        else
        {
            Mov.color = Color.black;
            MovText.color = Color.black;
        }

        Atk.text = chr._Attack.ToString();
        Def.text = chr._Defense.ToString();
        Agl.text = chr._Agility.ToString();
        Mov.text = chr._MovementRange.ToString();
    }

    //// Update is called once per frame
    //void Update () {
    //       setproporty();
    //   }
}
