using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerController : MonoBehaviour {

    public bool Attack = true;
    bool increase = true;
    public float speed;

	// Update is called once per frame
	void Update () {
        scaleSelf();
    }

    void scaleSelf()
    {
        if (Attack)
        {
            if (increase)
            {
                this.gameObject.transform.localScale = new Vector3(2, 2, 2);
                increase = !increase;
            }
            else
            {
                if (this.gameObject.transform.localScale.x > 0.5f)
                {
                    this.gameObject.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f) * speed;
                }
                else
                {
                    increase = !increase;
                }
            }
        }
        else
        {
            if (increase)
            {
                this.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                increase = !increase;
            }
            else
            {
                if (this.gameObject.transform.localScale.x < 2f)
                {
                    this.gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f) * speed;
                }
                else
                {
                    increase = !increase;
                }
            }
        }

    }
}
