using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    public List<Event> _tutorials = new List<Event>();

    public void PlayTutorialEvent(int index)
    {
        if (index < _tutorials.Count)
        {
            EventManager.Instance.PlaySpecialEvent(_tutorials[index]);
        }
    }
}
