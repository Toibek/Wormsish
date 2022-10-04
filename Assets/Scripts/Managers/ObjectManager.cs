using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public float AirDropPosition;
    public float GroundDropCheck;
    private List<GameObject> _activeObjects = new();
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }
    public void ClearObjects()
    {
        for (int i = _activeObjects.Count - 1; i >= 0; i--)
            _activeObjects[i].GetComponent<DamageableObject>().LethalDamage();
    }
    public void SpawnOnGround(SpawnableObject obj, int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            Ray ray = new(_gameManager.ValidPosition + Vector3.up * GroundDropCheck, Vector2.down);
            if (Physics.Raycast(ray, out var hit))
            {
                GameObject go = obj.GetObject(hit.point + Vector3.up * 0.5f, transform);
                go.name = obj.Name;
                _activeObjects.Add(go);
            }
        }
    }
    //public void SpawnInAir(SpawnableObject obj, int amount = 1)
    //{
    //    for (int i = 0; i < amount; i++)
    //    {
    //        GameObject go = obj.GetObject(_gameManager.ValidPosition + Vector3.up * AirDropPosition, transform);
    //        go.GetComponent<DamageableObject>().ReturnTo = obj;
    //        go.name = obj.Name;
    //        _activeObjects.Add(go);
    //    }
    //}
    //public void Decomission(GameObject go)
    //{
    //    _activeObjects.Remove(go);
    //    SpawnableObject obj = new();
    //    for (int i = 0; i < _gameManager.Spawnables.Length; i++)
    //    {
    //        if (go.name == _gameManager.Spawnables[i].Name)
    //        {
    //            obj = _gameManager.Spawnables[i];
    //            break;
    //        }
    //    }
    //    obj.DeactivateObject(go);
    //    if (string.IsNullOrEmpty(obj.Name)) Debug.LogWarning("Unexpected object trying to pool");
    //}
}
