using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIController : MonoBehaviour
{
    [SerializeField] private List<TimeBase> _timeClocks;

    private void Awake()
    {
        for (int i = 0; i < _timeClocks.Count; i++)
        {
            _timeClocks[i].OnEditTimeChanged += (DateTime newTime) =>
            {
                for (int j = 0; j < _timeClocks.Count; j++) if (i != j) _timeClocks[j].SetTime(newTime);
            };
        }
    }
}
