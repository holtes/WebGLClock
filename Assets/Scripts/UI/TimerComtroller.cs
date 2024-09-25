using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using System;
using Cysharp.Threading.Tasks;

public class TimerComtroller : TimeBase
{
    [SerializeField] private TMP_InputField _timerTxt;
    private DateTime _currentTime = DateTime.Now;
    private bool _isTimeUpdated = true;
    private bool _isStart = true;
    void Start()
    {
        _timerTxt.enabled = false;
    }

    public override void SetTime(DateTime newTime)
    {
        _currentTime = newTime;
        _timerTxt.text = newTime.ToString("HH:mm:ss");
        if (_isStart)
        {
            _isStart = false;
            UpdateTime();
        }
    }

    private async UniTaskVoid UpdateTime()
    {
        while (_isTimeUpdated)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: false);
            if (_isTimeUpdated) _currentTime = _currentTime.Add(TimeSpan.FromSeconds(1));
            _timerTxt.text = _currentTime.ToString("HH:mm:ss");
        }
    }

    private void ValidateInput(string newTimeString)
    {
        string pattern = @"^(2[0-3]|[01]?[0-9]):([0-5]?[0-9]):([0-5]?[0-9])$";
        if (!Regex.IsMatch(newTimeString, pattern))
        {
            _timerTxt.text = _currentTime.ToString("HH:mm:ss");
        }
    }

    public override void Edit(bool isEdit, EditToggle.EditType editType)
    {
        if (editType == EditToggle.EditType.DigitClock)
        {
            _isTimeUpdated = !isEdit;
            _timerTxt.enabled = isEdit;
            if (!isEdit)
            {
                if (DateTime.TryParse(_timerTxt.text, out DateTime parsedTime))
                {
                    _currentTime = parsedTime;
                }
                _timerTxt.onValueChanged.RemoveListener(ValidateInput);
                OnEditTimeChanged?.Invoke(_currentTime);
                UpdateTime();
            }
            else _timerTxt.onValueChanged.AddListener(ValidateInput);
        }
    }
}
