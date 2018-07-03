using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResultController : MonoBehaviour
{
    public GameObject LPower;
    public GameObject LPrecision;
    public GameObject LTechnique;
    public GameObject LBlock;
    public GameObject LCounter;
    public GameObject LDodge;
    public GameObject LRandom;

    public GameObject RPower;
    public GameObject RPrecision;
    public GameObject RTechnique;
    public GameObject RBlock;
    public GameObject RCounter;
    public GameObject RDodge;
    public GameObject RRandom;

    // Use this for initialization
    void Start()
    {
        //LPower.SetActive(false);
        //LPrecision.SetActive(false);
        //LTechnique.SetActive(false);
        //LBlock.SetActive(false);
        //LCounter.SetActive(false);
        //LDodge.SetActive(false);
        //RPower.SetActive(false);
        //RPrecision.SetActive(false);
        //RTechnique.SetActive(false);
        //RBlock.SetActive(false);
        //RCounter.SetActive(false);
        //RDodge.SetActive(false);
        //LRandom.SetActive(false);
        //RRandom.SetActive(false);
    }

    //1 power 2 technique 3 precision
    //4 block 5 dodge 6 counter
    // 7 random
    public void ShowBattleResult(int Left, int Right)
    {
        switch (Left)
        {
            case 1: // power
                LPower.SetActive(true);
                LPrecision.SetActive(false);
                LTechnique.SetActive(false);
                LBlock.SetActive(false);
                LCounter.SetActive(false);
                LDodge.SetActive(false);
                LRandom.SetActive(false);
                break;
            case 2: //technique
                LPower.SetActive(false);
                LPrecision.SetActive(false);
                LTechnique.SetActive(true);
                LBlock.SetActive(false);
                LCounter.SetActive(false);
                LDodge.SetActive(false);
                LRandom.SetActive(false);
                break;
            case 3: //precision
                LPower.SetActive(false);
                LPrecision.SetActive(true);
                LTechnique.SetActive(false);
                LBlock.SetActive(false);
                LCounter.SetActive(false);
                LDodge.SetActive(false);
                LRandom.SetActive(false);
                break;
            case 4: //guard
                LPower.SetActive(false);
                LPrecision.SetActive(false);
                LTechnique.SetActive(false);
                LBlock.SetActive(true);
                LCounter.SetActive(false);
                LDodge.SetActive(false);
                LRandom.SetActive(false);
                break;
            case 5: //dodge
                LPower.SetActive(false);
                LPrecision.SetActive(false);
                LTechnique.SetActive(false);
                LBlock.SetActive(false);
                LCounter.SetActive(false);
                LDodge.SetActive(true);
                LRandom.SetActive(false);
                break;
            case 6: //counter
                LPower.SetActive(false);
                LPrecision.SetActive(false);
                LTechnique.SetActive(false);
                LBlock.SetActive(false);
                LCounter.SetActive(true);
                LDodge.SetActive(false);
                LRandom.SetActive(false);
                break;
            case 7:
                LPower.SetActive(false);
                LPrecision.SetActive(false);
                LTechnique.SetActive(false);
                LBlock.SetActive(false);
                LCounter.SetActive(false);
                LDodge.SetActive(false);
                LRandom.SetActive(true);
                break;
        }

        switch (Right)
        {
            case 1: // power
                RPower.SetActive(true);
                RPrecision.SetActive(false);
                RTechnique.SetActive(false);
                RBlock.SetActive(false);
                RCounter.SetActive(false);
                RDodge.SetActive(false);
                RRandom.SetActive(false);
                break;
            case 2: // technique
                RPower.SetActive(false);
                RPrecision.SetActive(false);
                RTechnique.SetActive(true);
                RBlock.SetActive(false);
                RCounter.SetActive(false);
                RDodge.SetActive(false);
                RRandom.SetActive(false);
                break;
            case 3: // precision
                RPower.SetActive(false);
                RPrecision.SetActive(true);
                RTechnique.SetActive(false);
                RBlock.SetActive(false);
                RCounter.SetActive(false);
                RDodge.SetActive(false);
                RRandom.SetActive(false);
                break;
            case 4: // guard
                RPower.SetActive(false);
                RPrecision.SetActive(false);
                RTechnique.SetActive(false);
                RBlock.SetActive(true);
                RCounter.SetActive(false);
                RDodge.SetActive(false);
                RRandom.SetActive(false);
                break;
            case 5: // dodge
                RPower.SetActive(false);
                RPrecision.SetActive(false);
                RTechnique.SetActive(false);
                RBlock.SetActive(false);
                RCounter.SetActive(false);
                RDodge.SetActive(true);
                RRandom.SetActive(false);
                break;
            case 6: //counter
                RPower.SetActive(false);
                RPrecision.SetActive(false);
                RTechnique.SetActive(false);
                RBlock.SetActive(false);
                RCounter.SetActive(true);
                RDodge.SetActive(false);
                RRandom.SetActive(false);
                break;
            case 7:
                RPower.SetActive(false);
                RPrecision.SetActive(false);
                RTechnique.SetActive(false);
                RBlock.SetActive(false);
                RCounter.SetActive(false);
                RDodge.SetActive(false);
                RRandom.SetActive(true);
                break;
        }
    }
}
