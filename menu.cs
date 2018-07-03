using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class menu : MonoBehaviour {

    public void MouseEnter()
    {
        Debug.Log("mouse enter");
    }


    public void PointerEnter(BaseEventData eventData)
    {
        print("It's me: " + name);
    }
}
