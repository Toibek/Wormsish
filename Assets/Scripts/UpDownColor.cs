using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpDownColor : MonoBehaviour
{
    public List<Color> _avaliableColors;
    public string ValueName;
    public Color CurrentColor
    {
        get { return _currentColor; }
        set
        {
            _currentColor = value;
            _colorImage.color = _currentColor;
        }
    }
    [SerializeField] private Color _currentColor;
    private Button _leftButton;
    private Button _rightButton;
    private Image _colorImage;
    private int _currentValue;
    private void Start()
    {
        Image[] allImages = transform.GetComponentsInChildren<Image>();
        _colorImage = allImages[1];
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        _leftButton = buttons[0];
        _rightButton = buttons[1];

        _currentValue = Random.Range(0, _avaliableColors.Count);
        CurrentColor = _avaliableColors[_currentValue];

        _leftButton.onClick.AddListener(Decrease);
        _rightButton.onClick.AddListener(Increase);
    }
    public void Increase()
    {
        if (++_currentValue >= _avaliableColors.Count)
            _currentValue = 0;
        CurrentColor = _avaliableColors[_currentValue];
    }
    public void Decrease()
    {
        if (--_currentValue < 0)
            _currentValue = _avaliableColors.Count - 1;
        CurrentColor = _avaliableColors[_currentValue];
    }
}
