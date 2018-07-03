using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barmanager : MonoBehaviour {

    public GameObject bar;
    public float speed;
    bool canflash;
    bool isincrease;
    // Use this for initialization
    void Start () {
        canflash = true;
        isincrease = true;
    }

    void incresebar()
    {
        if (bar.transform.localScale.x < 1)
        {
            bar.transform.localScale += new Vector3(speed, 0, 0) * Time.deltaTime;
            flash();
        }
        else
        {
            bar.transform.localScale = new Vector3(0, bar.transform.localScale.y, bar.transform.localScale.z);
        }
    }

    void flash()
    {
        if (canflash)
        {
            if (bar.transform.localScale.x > 0.1 && bar.transform.localScale.x < 0.95)
            {
                if (isincrease)
                {
                    bar.transform.localScale += new Vector3(0.05f, 0, 0) * Random.Range(1,3);
                }
                else
                {
                    bar.transform.localScale -= new Vector3(0.05f, 0, 0) * Random.Range(1, 1.5f);
                }
                canflash = false;
                isincrease = !isincrease;
                Invoke("resetflash", 0.1f);
            }

        }
    }

    void resetflash()
    {
        canflash = true;
    }
	
	// Update is called once per frame
	void Update () {
        incresebar();

    }
}
