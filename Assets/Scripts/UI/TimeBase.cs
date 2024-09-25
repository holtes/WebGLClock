using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class TimeBase : MonoBehaviour
{
    public Action<DateTime> OnEditTimeChanged;
    void Awake()
    {
        TimeReciever.OnRecievedTime += SetTime;
    }

    public abstract void SetTime(DateTime newTime);

    public abstract void Edit(bool isEdit, EditToggle.EditType editType);

    private void OnDestroy()
    {
        TimeReciever.OnRecievedTime -= SetTime;
    }


}
