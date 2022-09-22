using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGenerator : MonoBehaviour
{
    [SerializeField] IslandManager _im;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(_im.GenerationRoutine());
    }
}
