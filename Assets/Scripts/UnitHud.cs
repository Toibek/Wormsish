using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitHud : MonoBehaviour
{
    [SerializeField] private Transform _unit;

    [SerializeField] private float disDiv;
    [SerializeField] private TMP_Text _nameField;
    [SerializeField] private RectTransform _healthBar;

    private Camera camRef;
    private void Start()
    {
        camRef = Camera.main;
    }

    private void Update()
    {
        if (_unit != null)
        {
            transform.position = camRef.WorldToScreenPoint(_unit.position + Vector3.up * 2);
            float dis = Vector3.Distance(camRef.transform.position, _unit.position);
            transform.localScale = new(dis / disDiv, dis / disDiv, dis / disDiv);
        }
    }
    public void SetTeamColor(Color color)
    {
        GetComponent<Image>().color = color;
    }
    public void UpdateName(string name)
    {
        _nameField.text = name;
    }
    public void UpdateHealth(float healthPercent)
    {
        healthPercent = Mathf.Clamp01(healthPercent);
        _healthBar.localScale = new(healthPercent, 1, 1);
    }
}
