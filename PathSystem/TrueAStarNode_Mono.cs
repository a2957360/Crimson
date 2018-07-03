using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueAStarNode_Mono : MonoBehaviour
{
    public Color _color1;
    public Color _color2;

    private SpriteRenderer _spRenderer;

    void Awake()
    {
        _spRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_spRenderer != null)
            _spRenderer.color = Color.Lerp(_color1, _color2, Mathf.PingPong(Time.time, 1));
    }
}
