using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMoveTextController : MonoBehaviour
{
    public Text _moveText;

    public Color _colorRed1;
    public Color _colorRed2;

    public Color _colorGreen1;
    public Color _colorGreen2;

    public Color _colorBlue1;
    public Color _colorBlue2;

    private Color _curColor1;
    private Color _curColor2;

    private void Start()
    {
        _curColor1 = _colorGreen1;
        _curColor2 = _colorGreen2;
    }

    void Update()
    {
        _moveText.color = Color.Lerp(_curColor1, _curColor2, Mathf.PingPong(Time.time, 1));
    }

    public void SetColorText(string text, int color)
    {
        _moveText.text = text;

        if (color >= 1)
        {
            _curColor1 = _colorRed1;
            _curColor2 = _colorRed2;
        }
        else if (color <= -1)
        {
            _curColor1 = _colorGreen1;
            _curColor2 = _colorGreen2;
        }
        else
        {
            _curColor1 = _colorBlue1;
            _curColor2 = _colorBlue2;
        }
    }
}
