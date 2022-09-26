using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGenerator : MonoBehaviour
{
    [SerializeField] IslandManager _im;
    [SerializeField] ObjectManager _og;
    [SerializeField] [TextArea] string IslandGenString;
    [SerializeField] [TextArea] string ObjectGenString;
    Coroutine routine;
    private void Start()
    {
        Application.runInBackground = true;
    }
    public void Generate()
    {
        if (routine == null)
            routine = StartCoroutine(TestGen());
    }
    IEnumerator TestGen()
    {
        _og.ClearObjects();
        _im.ClearMap();


        if (IslandGenString == "")
            Debug.Log(_im.GenerateIsland().Replace(':', '\n'));
        else
            Debug.Log(_im.GenerateIsland(IslandGenString).Replace(':', '\n'));


        if (ObjectGenString == "")
        {
            bool[,] placeable = _im.PlaceableArea;
            Debug.Log(_og.SpawnObjects(placeable).Replace(':', '\n'));
        }
        else
            Debug.Log(_og.SpawnObjects(ObjectGenString).Replace(':', '\n'));

        yield return new WaitForEndOfFrame();
        routine = null;
    }
}
