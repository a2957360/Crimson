using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBarController : MonoBehaviour {

    public float MaxTime;
    float currentTime;

	// Use this for initialization
	void Start () {
        currentTime = MaxTime;
    }

    void TimeCounter()
    {
        if (currentTime > 0)
        {
            if (currentTime < 0.1f)
            {
                currentTime = 0;
                this.gameObject.transform.localScale = new Vector3(0, 1, 1);
                return;
            }
            currentTime -= Time.deltaTime;
            this.gameObject.transform.localScale = new Vector3((currentTime / MaxTime) * 1, 1, 1);
        }
    }
	
	// Update is called once per frame
	void Update () {
        TimeCounter();
    }
}
