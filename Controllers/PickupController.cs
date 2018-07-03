using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    public Vector2 _position;

    public GameObject _crystal_S;
    public GameObject _crystal_M;
    public GameObject _crystal_L;

    public enum Crystal_Sizes : int
    {
        Small,
        Med,
        Large,
    };
    public Crystal_Sizes _crystal_size = Crystal_Sizes.Small;

    // 1: small, 2: med, 3: large
    public void SetCrystalSize(int value, Vector2 pos)
    {
        if (_crystal_S != null && _crystal_M != null && _crystal_L != null)
        {
            _position = pos;
            switch (value)
            {
                case 1:
                    {
                        _crystal_size = Crystal_Sizes.Small;
                        _crystal_S.SetActive(true);
                        _crystal_M.SetActive(false);
                        _crystal_L.SetActive(false);
                    }
                    break;
                case 2:
                    {
                        _crystal_size = Crystal_Sizes.Med;
                        _crystal_S.SetActive(false);
                        _crystal_M.SetActive(true);
                        _crystal_L.SetActive(false);
                    }
                    break;
                case 3:
                    {
                        _crystal_size = Crystal_Sizes.Large;
                        _crystal_S.SetActive(false);
                        _crystal_M.SetActive(false);
                        _crystal_L.SetActive(true);
                    }
                    break;
            }
        }
    }

    void OnDestroy()
    {
        Debug.Log(gameObject.name + " was picked up");
        GameManager.Instance._crystals.Remove(this);
    }
}
