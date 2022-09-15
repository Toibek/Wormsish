using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingController : MonoBehaviour
{
    [SerializeField] private IslandGen _islandGen;
    [SerializeField] private ObjectGen _objectGen;
    [SerializeField] private GameObject _explosionPrefab;
    private void Start()
    {
        StartCoroutine(MapSpawn());
    }
    IEnumerator MapSpawn()
    {
        yield return StartCoroutine(_islandGen.GenerationRoutine());
        bool[,] placeable = _islandGen.PlaceableArea;
        Debug.Log(placeable);
        yield return StartCoroutine(_objectGen.GenerateObjects(placeable));

        Debug.Log("Generation complete");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                //Instantiate(_explosionPrefab, hit.point, Quaternion.identity);
                if (hit.transform.GetComponent<Damageable>())
                {
                    Damageable d = hit.transform.GetComponent<Damageable>();
                    d.Damage(100);
                }
            }
        }
    }
}
