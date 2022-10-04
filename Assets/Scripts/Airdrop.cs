using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airdrop : MonoBehaviour
{
    [SerializeField] private float AnimationTime;
    [SerializeField] private GameObject ConfettiPrefab;
    internal GameObject Drop;
    GameManager _gameManager;
    Transform _crate;
    float _scaleRef;
    private void OnCollisionEnter(Collision collision)
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        Drop = _gameManager.RandomPickup;
        _crate = transform.GetChild(0);
        StartCoroutine(OpenPackage());
    }
    IEnumerator OpenPackage()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        for (float f = 0; f < AnimationTime; f += Time.deltaTime)
        {
            _crate.Rotate(0, 2 * Time.deltaTime, 0);
            float scale = _scaleRef * (1 - (f / AnimationTime));
            yield return new WaitForEndOfFrame();
        }
        if (ConfettiPrefab != null) Instantiate(ConfettiPrefab, transform.position, Quaternion.identity);
        Drop.transform.position = transform.position;
        Drop.SetActive(true);
        _gameManager.Airdrop = gameObject;
    }
}
