using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : Singleton<WeatherManager>
{
    public bool _hasWeather = false;

    public GameObject Rain;
    public GameObject Mist;

    public void OpenWeather()
    {
        if (_hasWeather)
        {
            Rain.SetActive(true);
            Mist.SetActive(true);
        }
    }

    public void CLoseWeather()
    {
        Rain.SetActive(false);
        Mist.SetActive(false);
    }

    public void EnableWeather()
    {
        _hasWeather = true;
        OpenWeather();
    }

    public void DisableWeather()
    {
        _hasWeather = false;
        CLoseWeather();
    }
}
