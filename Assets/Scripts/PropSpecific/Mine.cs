using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private int _delay;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private float _jumpForce;
    [SerializeField] private bool _testTrigger;
    private Coroutine _activeRoutine;
    private void Update()
    {
        if (_testTrigger)
        {
            _testTrigger = false;
            _activeRoutine = StartCoroutine(Triggered());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _activeRoutine == null)
        {
            _activeRoutine = StartCoroutine(Triggered());
        }
    }
    IEnumerator Triggered()
    {
        for (int i = 0; i < _delay; i++)
        {
            //beepnoise
            yield return new WaitForSeconds(1);
            GetComponent<Rigidbody>().AddForce(new(0, _jumpForce, 0));
        }
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);

    }
}
