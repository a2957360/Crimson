using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTrigger : FieldEvent
{
    public Event _toTriggerEvent;

    // how much of mission phase does this trigger change
    //public int _phaseChange = 0;

    public bool TriggerCheck()
    {
        bool ret = false;
        //ChrController chr = null;

        if (_active)
        {
            if (!_toTriggerEvent._hasPhaseLimit || _toTriggerEvent._phaseLimit > MissionManager.Instance._curPhase)
            {
                ret = true;
                //MissionManager.Instance._curPhase += _phaseChange;
                //EventManager.Instance.PlaySpecialEvent(_toTriggerEvent);
                MissionManager.Instance._triggerEvent = _toTriggerEvent;
            }
            //if (_playerSide)
            //{
            //    chr = GameManager.Instance.CheckUnit(_locationX, _locationY, 1);
            //}
            //else
            //{
            //    chr = GameManager.Instance.CheckUnit(_locationX, _locationY, 2);
            //}
            //if (chr != null)
            //{
                
            //}
        }

        return ret;
    }

}
