using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSettings : MonoBehaviour
{
    public string Name
    {
        get { return _nameField.text; }
        set { _nameField.text = value; }
    }
    public Color Color
    {
        get { return _colorPicker.CurrentColor; }
    }
    TMP_InputField _nameField;
    UpDownColor _colorPicker;
    private void Awake()
    {
        _nameField = GetComponentInChildren<TMP_InputField>();
        _colorPicker = GetComponentInChildren<UpDownColor>();
    }
}
