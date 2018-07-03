//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class MenuController : Singleton<MenuController>
//{

//    public List<GameObject> Signals;
//    public List<GameObject> MainSelectionlist;
//    public List<GameObject> AttackSelectionlist;
//    public List<GameObject> ItemSelectionlist;
//    List<List<GameObject>> List = new List<List<GameObject>>();
//    public float moveSignspeed;
//    private float _moveSignspeed;

//    int SignalNo;
//    int i;
//    int Moveover;
//    bool delaybutton = false;

//    public bool ShowSelectionList = false;
//    public bool ShowAttackSelectionList = false;
//    public bool ShowItemSelectionList = false;
//    public bool MoveAction = false;
//    public bool AfterMoveAction = false;

//    void MoveSign()
//    {
//        if (_moveSignspeed <= 0)
//        {
//            if (Input.GetAxis("Vertical") < 0)
//            {
//                if (i < List[SignalNo].Count - 1)
//                {
//                    i++;
//                }
//                Signals[SignalNo].transform.position = List[SignalNo][i].transform.position;
//                _moveSignspeed = moveSignspeed;
//            }
//            else if (Input.GetAxis("Vertical") > 0)
//            {
//                if (i > 0 + Moveover && SignalNo == 0)
//                {
//                    i--;
//                }
//                if (i > 0 && SignalNo != 0)
//                {
//                    i--;
//                }
//                Signals[SignalNo].transform.position = List[SignalNo][i].transform.position;
//                _moveSignspeed = moveSignspeed;
//            }
//        }
//        else
//        {
//            _moveSignspeed -= Time.deltaTime;
//        }

//    }

//    public void MoveWithMouse(int num)
//    {
//        if (ShowSelectionList && !MoveAction)
//        {
//            i = num;
//            Signals[SignalNo].transform.position = List[SignalNo][i].transform.position;
//        }
//    }

//    public void Fire1Button()
//    {
//        if (!MoveAction && ShowSelectionList)
//        {
//            if (List[SignalNo][i].name == "Move")
//            {
//                MoveAction = true;
//                GameManager.Instance._curUnit.showarea();
//            }
//            if (List[SignalNo][i].name == "Attack")
//            {
//                //ShowAttackSelectionList = true;
//                //SignalNo = 1;
//                //i = 0;

//            }
//            if (List[SignalNo][i].name == "Item")
//            {
//                ShowItemSelectionList = true;
//                SignalNo = 2;
//                i = 0;
//            }
//            if (List[SignalNo][i].name == "Stay")
//            {
//                ShowSelectionList = false;
//                MoveAction = false;
//                AfterMoveAction = false;
//                delaybutton = false;
//                Moveover = 0;
//                i = 0;
//                Signals[SignalNo].transform.position = List[SignalNo][i].transform.position;
//                GameManager.Instance._curUnit.TurnFinished();
//            }
//        }
//    }

//    void Pressbutton()
//    {
//        if (delaybutton)
//        {
//            if (Input.GetButtonUp("Fire1"))
//            {
//                if (List[SignalNo][i].name == "Move")
//                {
//                    MoveAction = true;
//                    GameManager.Instance._curUnit.showarea();
//                    delaybutton = false;
//                    Invoke("resetbutton", 0.2f);
//                }
//                if (List[SignalNo][i].name == "Attack")
//                {
//                    //ShowAttackSelectionList = true;
//                    //SignalNo = 1;
//                    //i = 0;
//                    delaybutton = false;
//                    Invoke("resetbutton", 0.2f);
//                }
//                if (List[SignalNo][i].name == "Item")
//                {
//                    //ShowItemSelectionList = true;
//                    //SignalNo = 2;
//                    //i = 0;
//                    delaybutton = false;
//                    Invoke("resetbutton", 0.2f);
//                }
//                if (List[SignalNo][i].name == "Stay")
//                {
//                    ShowSelectionList = false;
//                    MoveAction = false;
//                    AfterMoveAction = false;
//                    Moveover = 0;
//                    GameManager.Instance._curUnit.TurnFinished();
//                    i = 0;
//                    Signals[SignalNo].transform.position = List[SignalNo][i].transform.position;
//                    delaybutton = false;
//                }

//            }

//            if (Input.GetButtonDown("Fire2"))
//            {
//                if (ShowAttackSelectionList)
//                {
//                    ShowAttackSelectionList = false;
//                    SignalNo = 0;
//                    i = 1;
//                    return;
//                }
//                else if (ShowItemSelectionList)
//                {
//                    ShowItemSelectionList = false;
//                    SignalNo = 0;
//                    i = 2;
//                    return;
//                }
//                delaybutton = false;
//                Invoke("resetbutton", 0.2f);
//            }

//            if (Input.GetButtonDown("Fire2"))
//            {
//                if (AfterMoveAction)
//                {
//                    MoveAction = false;
//                }
//                else
//                {

//                    ShowSelectionList = false;
//                    MoveAction = false;
//                    AfterMoveAction = false;
//                    Moveover = 0;
//                    GameObject destorypath = GameObject.Find("wholepath");
//                    if (destorypath != null)
//                    {
//                        Destroy(destorypath);
//                    }
//                    GameObject destoryitem = GameObject.Find("avaiablecells");
//                    if (destoryitem != null)
//                    {
//                        Destroy(destoryitem);
//                    }
//                    i = 0;
//                    Signals[SignalNo].transform.position = List[SignalNo][i].transform.position;
//                }
//                delaybutton = false;
//                Invoke("resetbutton", 0.2f);
//            }

//        }
//    }
//    void resetbutton()
//    {
//        delaybutton = true;
//    }

//    void dismoveaciton()
//    {
//        if (Input.GetButtonDown("Fire2"))
//        {
//            if (MoveAction)
//            {
//                MoveAction = false;
//            }
//        }
//    }

//    void showlist()
//    {
//        if (AfterMoveAction)
//        {
//            Moveover = 1;
//            MainSelectionlist[0].GetComponent<Image>().color = new Color(255,0,0,0.5f);
//        }
//        else
//        {
//            Moveover = 0;
//            MainSelectionlist[0].GetComponent<Image>().color = new Color(255, 255, 255,1.0f);
//        }

//        if (i == 0 && AfterMoveAction && SignalNo == 0)
//        {
//            i++;
//            Signals[SignalNo].transform.position = MainSelectionlist[i].transform.position;
//        }

//        if (ShowSelectionList)
//        {
//            for (int i = 0; i < MainSelectionlist.Count; i++)
//            {
//                MainSelectionlist[i].SetActive(true);
//            }
//            Signals[0].SetActive(true);
//        }
//        else
//        {
//            for (int i = 0; i < MainSelectionlist.Count; i++)
//            {
//                MainSelectionlist[i].SetActive(false);
//            }
//            Signals[0].SetActive(false);
//        }

//        if (ShowAttackSelectionList)
//        {
//            for (int i = 0; i < AttackSelectionlist.Count; i++)
//            {
//                AttackSelectionlist[i].SetActive(true);
//            }
//            Signals[1].SetActive(true);
//        }
//        else
//        {
//            for (int i = 0; i < AttackSelectionlist.Count; i++)
//            {
//                AttackSelectionlist[i].SetActive(false);
//            }
//            Signals[1].SetActive(false);
//        }

//        if (ShowItemSelectionList)
//        {
//            for (int i = 0; i < ItemSelectionlist.Count; i++)
//            {
//                ItemSelectionlist[i].SetActive(true);
//            }
//            Signals[2].SetActive(true);
//        }
//        else
//        {
//            for (int i = 0; i < ItemSelectionlist.Count; i++)
//            {
//                ItemSelectionlist[i].SetActive(false);
//            }
//            Signals[2].SetActive(false);
//        }
//    }

//    // Use this for initialization
//    void Start () {
//        _moveSignspeed = moveSignspeed;
//        i = 0;
//        Moveover = 0;
//        SignalNo = 0;
//        List.Add(MainSelectionlist);
//        List.Add(AttackSelectionlist);
//        List.Add(ItemSelectionlist);
//    }
	
//	// Update is called once per frame
//	void Update () {
//        //showlist();
//        //dismoveaciton();
//        //if (!MoveAction && ShowSelectionList && GameManager.Instance._curUnit != null && !PlayerInputManager.Instance._watchMode && PlayerInputManager.Instance._playerPhase)
//        //{
//        //    Invoke("resetbutton", 0.2f);
//        //    MoveSign();
//        //    Pressbutton();
//        //}
//    }
//}
