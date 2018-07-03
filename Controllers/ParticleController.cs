using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour {

    public GameObject AtkUp;
    public GameObject DefUp;
    public GameObject AglUp;
    public GameObject AtkDown;
    public GameObject DefDown;
    public GameObject AglDown;

    public void Activebuff(ChrController curunit)
    {
        if (curunit._Brave > 0)
        {
            AtkUp.SetActive(true);
        }
        else
        {
            AtkUp.SetActive(false);
        }
        if (curunit._Brave < 0)
        {
            AtkDown.SetActive(true);
        }
        else
        {
            AtkDown.SetActive(false);
        }
        if (curunit._Protect > 0)
        {
            DefUp.SetActive(true);
        }
        else
        {
            DefUp.SetActive(false);
        }
        if (curunit._Protect < 0)
        {
            DefDown.SetActive(true);
        }
        else
        {
            DefDown.SetActive(false);
        }
        if (curunit._Speed > 0)
        {
            AglUp.SetActive(true);
        }
        else
        {
            AglUp.SetActive(false);
        }
        if (curunit._Speed < 0)
        {
            AglDown.SetActive(true);
        }
        else
        {
            AglDown.SetActive(false);
        }
    }

    public void disactiveBuff()
    {
        AtkDown.SetActive(false);
        AtkUp.SetActive(false);
        DefUp.SetActive(false);
        DefDown.SetActive(false);
        AglUp.SetActive(false);
        AglDown.SetActive(false);
    }


}
