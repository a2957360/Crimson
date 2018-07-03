using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashManager : Singleton<FlashManager>
{
    public GameObject LeftFLash;
    public GameObject RightFLash;
    public float delayStopTime;

    public void StartLeft()
    {
        LeftFLash.SetActive(true);
        Invoke("Stop", delayStopTime * 1.5f);
    }

    public void StartRight()
    {
        RightFLash.SetActive(true);
        Invoke("Stop", delayStopTime * 1.5f);
    }

    public void StartLeft_Delay()
    {
        Invoke("StartLeft", delayStopTime);
    }

    public void StartRight_Dealy()
    {
        Invoke("StartRight", delayStopTime);
    }

    private void Stop()
    {
        LeftFLash.SetActive(false);
        RightFLash.SetActive(false);
    }

}
