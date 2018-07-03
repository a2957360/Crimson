using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoytickSignController : MonoBehaviour
{

    public Transform Starposition;
    public float flashspeed;
    public float increasetime;
    public float decreasetime;
    private float _increasetime;
    private float _decreasetime;
    public float delayresponse;
    bool delaymove;

    // Use this for initialization
    void Start()
    {
        delaymove = true;
        //transform.position = Starposition.position;
        _increasetime = increasetime;
        _decreasetime = decreasetime;
    }

    void MoveSign()
    {
        if (delaymove)
        {
            transform.position = new Vector3(PlayerInputManager.Instance._cursorGridX, PlayerInputManager.Instance._cursorGridY, 0);
            PauseMove();
        }
    }

    private void PauseMove()
    {
        delaymove = false;
        Invoke("ResetMove", delayresponse);
    }

    private void ResetMove()
    {
        delaymove = true;
    }

    void FlashSign()
    {
        if (_increasetime > 0)
        {
            gameObject.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, flashspeed * Time.deltaTime);
            _increasetime -= Time.deltaTime;
            if (_increasetime < 0)
            {
                _decreasetime = decreasetime;

            }
        }
        if (_decreasetime > 0 && _increasetime < 0)
        {
            gameObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, flashspeed * Time.deltaTime);
            _decreasetime -= Time.deltaTime;
            if (_decreasetime < 0)
            {
                _increasetime = increasetime;

            }
        }
    }



    void Update()
    {
        FlashSign();
        if (CameraManager.Instance._canFreeMove)
        {
            MoveSign();
        }
    }
}
