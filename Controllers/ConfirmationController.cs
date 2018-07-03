using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationController : MonoBehaviour {

    public Image ButtonAMove;
    public Image ButtonAStay;
    public Image ButtonB;
    public Image ButtonX;
    public Image ButtonY;
    public GameObject TextBg;

    //0 Move 1 Stay 2 Attack 3 Move and attack
    void SetConfirmationButton()
    {
        switch (PlayerInputManager.Instance.ConfirmationState)
        {
            case 0:
                ButtonAMove.color = new Color(1, 1, 1, 1);
                ButtonAMove.gameObject.SetActive(true);
                ButtonAStay.gameObject.SetActive(false);
                ButtonB.color = new Color(1, 1, 1, 1);
                ButtonY.color = new Color(1, 1, 1, 0.2f);
                ButtonX.color = new Color(1, 1, 1, 0.2f);
                TextBg.SetActive(false);
                break;
            case 1:
                ButtonAMove.color = new Color(1, 1, 1, 1);
                ButtonAMove.gameObject.SetActive(false);
                ButtonAStay.gameObject.SetActive(true);
                ButtonB.color = new Color(1, 1, 1, 1);
                ButtonY.color = new Color(1, 1, 1, 0.2f);
                ButtonX.color = new Color(1, 1, 1, 0.2f);
                TextBg.SetActive(false);
                break;
            case 2:
                ButtonAMove.color = new Color(1, 1, 1, 0.2f);
                ButtonB.color = new Color(1, 1, 1, 1);
                ButtonY.color = new Color(1, 1, 1, 1);
                ButtonX.color = new Color(1, 1, 1, 1);
                TextBg.SetActive(true);
                break;
            case 3:
                ButtonAMove.color = new Color(1, 1, 1, 1);
                ButtonB.color = new Color(1, 1, 1, 1);
                ButtonY.color = new Color(1, 1, 1, 1);
                ButtonX.color = new Color(1, 1, 1, 1);
                TextBg.SetActive(true);
                break;
            case 4:
                ButtonAMove.gameObject.SetActive(false);
                ButtonAStay.gameObject.SetActive(true);
                ButtonB.color = new Color(1, 1, 1, 0);
                ButtonY.color = new Color(1, 1, 1, 0);
                ButtonX.color = new Color(1, 1, 1, 0);
                TextBg.SetActive(false);
                break;
            case 5:
                ButtonAMove.color = new Color(1, 1, 1, 0.2f);
                ButtonB.color = new Color(1, 1, 1, 1);
                ButtonY.color = new Color(1, 1, 1, 1);
                ButtonX.color = new Color(1, 1, 1, 1);
                TextBg.SetActive(true);
                break;
        }


    }
	// Update is called once per frame
	void Update () {
        SetConfirmationButton();
    }
}
