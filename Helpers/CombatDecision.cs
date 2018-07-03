//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class CombatDecision : MonoBehaviour {

//    public Text Decision1;
//    public Text Decision2;
//    public Text Decision3;

//    void setdecision()
//    {
//        if (CombatManager.Instance._decisionValues.Count > 0)
//        {
//            if (CombatManager.Instance._decisionValues[0] == 0)
//            {
//                Decision1.text = "Precision Attack";
//            }
//            if (CombatManager.Instance._decisionValues[0] == 1)
//            {
//                Decision1.text = "Power Attack";
//            }
//            if (CombatManager.Instance._decisionValues[0] == -1)
//            {
//                Decision1.text = "Technique Attack";
//            }
//        }
//        else
//        {
//            Decision1.text = " ";
//        }
//        if (CombatManager.Instance._decisionValues.Count > 1)
//        {
//            if (CombatManager.Instance._decisionValues[1] == 0)
//            {
//                Decision2.text = "Precision Attack";
//            }
//            if (CombatManager.Instance._decisionValues[1] == 1)
//            {
//                Decision2.text = "Power Attack";
//            }
//            if (CombatManager.Instance._decisionValues[1] == -1)
//            {
//                Decision2.text = "Technique Attack";
//            }
//        }
//        else
//        {
//            Decision2.text = " ";
//        }
//        if (CombatManager.Instance._decisionValues.Count > 2)
//        {
//            if (CombatManager.Instance._decisionValues[2] == 0)
//            {
//                Decision3.text = "Precision Attack";
//            }
//            if (CombatManager.Instance._decisionValues[2] == 1)
//            {
//                Decision3.text = "Power Attack";
//            }
//            if (CombatManager.Instance._decisionValues[2] == -1)
//            {
//                Decision3.text = "Technique Attack";
//            }
//        }
//        else
//        {
//            Decision3.text = " ";
//        }
//    }
	
//	// Update is called once per frame
//	void Update () {
//        setdecision();
//    }
//}
