using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    private BaseTool _tool;
    private void Start()
    {
        _tool = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().RandomToolDrop;
        _tool.EquippedTransform = Instantiate(_tool.ToolPrefab, transform).transform;
    }
    private void Update()
    {
        float rot = rotationSpeed * Time.deltaTime;
        transform.GetChild(0).Rotate(0, rot, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Unit>().Active)
        {
            other.GetComponent<UnitTools>().AddTool(_tool);
            GetComponent<Damageable>().LethalDamage();
        }
    }
}
