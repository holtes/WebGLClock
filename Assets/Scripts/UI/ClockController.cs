using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Tools;
using System;

public class ClockController : TimeBase
{
    [SerializeField] private SerializableDictionary<string, ClockHand> _clockHandsDict;
    private const float BaseOffset = 89.3f;

    public override void SetTime(DateTime newTime)
    {
        Debug.Log(newTime);
        float hourAngle = (newTime.Hour % 12 + newTime.Minute / 60f) * 30f;
        _clockHandsDict["HourHand"].HandParent.eulerAngles = new Vector3(0f, 0f, BaseOffset - hourAngle);
        float minuteAngle = (newTime.Minute + newTime.Second / 60f) * 6f;
        _clockHandsDict["MinHand"].HandParent.eulerAngles = new Vector3(0f, 0f, BaseOffset - minuteAngle);
        float secondAngle = newTime.Second * 6f;
        _clockHandsDict["SecHand"].HandParent.eulerAngles = new Vector3(0f, 0f, BaseOffset - secondAngle);
        ActivateClockAnim();

    }

    private void ActivateClockAnim()
    {
        foreach (var animator in _clockHandsDict.values)
        {
            animator.HandAnimator.enabled = true;
            animator.HandAnimator.Rebind();
        }
    }

    public void StopClock()
    {
        foreach (var animator in _clockHandsDict.values)
        {
            animator.HandAnimator.enabled = false;
        }
    }

    public override void Edit(bool isEdit, EditToggle.EditType editType)
    {
         
        if (editType == EditToggle.EditType.AnalogClock)
        {
            foreach (var hand in _clockHandsDict.values) hand.GrabRotate.IsRotatable = isEdit;
            if (isEdit)
            {
                StopClock();
            }
            else
            {
                OnEditTimeChanged?.Invoke(GetSelectedTime());
            }
        }
    }

    private DateTime GetSelectedTime()
    {
        float secondsAngle = GetHandAngle("SecHand");

        int seconds = (int)Math.Round(secondsAngle / 6f);
        float minutesAngle = GetHandAngle("MinHand");

        int minutes = (int)Math.Round(minutesAngle / 6f);
        float hoursAngle = GetHandAngle("HourHand");

        int hours = (int)Math.Round(hoursAngle / 30f) % 12;
        if (minutes >= 60)
        {
            minutes = 0;
        }

        DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, minutes, seconds);

        return time;
    }

    private float GetHandAngle(string handName)
    {
        float angleOffset = Math.Abs(_clockHandsDict[handName].HandAnimator.transform.localEulerAngles.z - 360);
        return (Math.Abs(_clockHandsDict[handName].HandParent.eulerAngles.z - 360) + angleOffset + BaseOffset) % 360;
    }
}
