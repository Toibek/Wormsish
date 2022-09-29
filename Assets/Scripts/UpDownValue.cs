using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpDownValue : MonoBehaviour
{
    public string ValueName
    {
        get { return _titleText.text; }
        set { _titleText.text = value; }
    }
    public int Increments;
    public int CurrentValue
    {
        get { return _currentValue; }
        set
        {
            _currentValue = value;
            _valueText.text = _currentValue.ToString();
        }
    }
    [SerializeField] private int _currentValue;
    private TMP_Text _titleText;
    private Button _leftButton;
    private Button _rightButton;
    private TMP_Text _valueText;
    private void Awake()
    {
        TMP_Text[] texts = transform.GetComponentsInChildren<TMP_Text>();
        _titleText = texts[0];
        _valueText = texts[2];
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        _leftButton = buttons[0];
        _rightButton = buttons[1];
    }
    private void Start()
    {
        _titleText.text = ValueName;
        _valueText.text = CurrentValue.ToString();
        _leftButton.onClick.AddListener(Decrease);
        _rightButton.onClick.AddListener(Increase);
    }
    public void Increase() { CurrentValue += Increments; }
    public void Decrease() { CurrentValue -= Increments; }
}
