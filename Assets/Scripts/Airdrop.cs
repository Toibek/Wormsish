using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airdrop : MonoBehaviour
{
    [SerializeField] private float _animationTime;
    [SerializeField] private AnimationCurve _scaleCurve;
    [SerializeField] private float _rotSpeed;
    [SerializeField] private GameObject _confettiPrefab;

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
        _scaleRef = _crate.localScale.x;
        transform.GetChild(1).gameObject.SetActive(false);
        for (float f = 0; f < _animationTime; f += Time.deltaTime)
        {
            _crate.Rotate(0, 0, _rotSpeed * Time.deltaTime);
            float scale = _scaleRef * _scaleCurve.Evaluate(f / _animationTime);
            _crate.localScale = new(scale, scale, scale);
            yield return new WaitForEndOfFrame();
        }
        PackageOpened();
    }
    private void PackageOpened()
    {
        if (_confettiPrefab != null) Instantiate(_confettiPrefab, transform.position, Quaternion.identity);
        Drop.transform.position = transform.position;
        Drop.SetActive(true);

        _crate.localScale = new(_scaleRef, _scaleRef, _scaleRef);
        _crate.rotation = Quaternion.Euler(-90, 0, 0);
        transform.GetChild(1).gameObject.SetActive(true);
        _gameManager.Airdrop = gameObject;
    }
}
