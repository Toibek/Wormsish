using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] float _rotationSpeed;
    void Update()
    {
        transform.RotateAround(Vector2.zero, Vector3.up, _rotationSpeed * Time.deltaTime);
    }
}
