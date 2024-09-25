using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditToggle : MonoBehaviour
{
    public enum EditType
    {
        DigitClock,
        AnalogClock
    }

    public EditType Type;
    public Toggle Toggle;
}
