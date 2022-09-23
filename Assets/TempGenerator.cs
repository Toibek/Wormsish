using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGenerator : MonoBehaviour
{
    [SerializeField] IslandManager _im;
    [SerializeField] ObjectManager _og;
    Coroutine routine;
    private void Start()
    {
        Generate();
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
        yield return StartCoroutine(_im.GenerationRoutine());
        bool[,] placeable = _im.PlaceableArea;
        yield return StartCoroutine(_og.GenerateObjects(placeable));

        routine = null;
    }
}
