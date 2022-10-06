using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPickup : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    private BaseTool _tool;
    private void Start()
    {
        GetTool();
    }
    public void GetTool()
    {
        if (transform.childCount > 0)
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        if (_tool == null)
            _tool = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().RandomToolDrop;
        _tool.EquippedTransform = Instantiate(_tool.ToolPrefab, transform).transform;
    }
    private void Update()
    {
        if (_tool != null)
        {
            float rot = rotationSpeed * Time.deltaTime;
            _tool.EquippedTransform.Rotate(0, rot, 0);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Unit>().Active)
        {
            other.GetComponent<UnitTools>().AddTool(_tool);
            _tool = null;
            GetComponent<Damageable>().LethalDamage();
        }
    }
}
