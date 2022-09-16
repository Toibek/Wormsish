using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    [SerializeField] LayerMask _ignorePlayerMask;
    [SerializeField] int _jumpHeight;
    [SerializeField] float _moveTime;
    [SerializeField] AnimationCurve _heightCurve;
    Vector3 _startPosition = Vector3.zero;
    float _height = 1;
    Vector3 _endPosition = Vector3.forward;
    Coroutine _moveRoutine;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Move(transform.forward);
        }
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
            Move(new Vector2(Mathf.Sign(Input.GetAxis("Horizontal")), 0));
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
            Move(new Vector2(0, Mathf.Sign(Input.GetAxis("Vertical"))));
    }
    public void Move(Vector2 direction)
    {
        if (_moveRoutine == null)
            _moveRoutine = StartCoroutine(MoveIE(direction));
    }
    IEnumerator MoveIE(Vector2 dir)
    {
        Vector3 moveDir = new(Mathf.Clamp(dir.x, -1, 1), 0, Mathf.Clamp(dir.y, -1, 1));
        _startPosition = transform.position;
        Ray heightCheckRay = new(transform.position + new Vector3(0, _jumpHeight, 0), moveDir);
        Debug.DrawRay(heightCheckRay.origin, heightCheckRay.direction);
        if (!Physics.Raycast(heightCheckRay, 1f, _ignorePlayerMask))
        {
            Ray positioningRay = new(transform.position + moveDir+Vector3.up*3, Vector3.down);
            Debug.Log(positioningRay.origin);
            Debug.DrawRay(positioningRay.origin, positioningRay.direction + new Vector3(0, -1, 0) * 20);
            if (Physics.Raycast(positioningRay, out RaycastHit hit, 20))
            {
                //Destroy(hit.transform.gameObject);
                _endPosition = hit.point + new Vector3(0, .5f, 0);
            }
        }
        _height = Mathf.Max(_startPosition.y, _endPosition.y) - 1f;
        Vector3 half = Vector3.MoveTowards(_startPosition, _endPosition, Vector3.Distance(_startPosition, _endPosition) * 0.5f);
        for (float f = 0; f < _moveTime; f += Time.deltaTime)
        {
            Vector3 pos =
                Mathf.Pow(1 - f, 2) * _startPosition +
                2 * Mathf.Pow(1 - f, 2) * f * (_startPosition + new Vector3(0, _height)) +
                2 * (1 - f) * Mathf.Pow(f, 2) * (_endPosition + new Vector3(0, _height)) +
                Mathf.Pow(f, 2) * _endPosition;

            yield return new WaitForFixedUpdate();
        }
        transform.position = _endPosition;



        yield return new WaitForSeconds(1);
        _moveRoutine = null;
    }
    private void OnDrawGizmos()
    {
        for (float f = 0; f < 1; f += 0.05f)
        {
            Vector3 pos =
                Mathf.Pow(1 - f, 2) * _startPosition +
                2 * Mathf.Pow(1 - f, 2) * f * (_startPosition + new Vector3(0, _height)) +
                2 * (1 - f) * Mathf.Pow(f, 2) * (_endPosition + new Vector3(0, _height)) +
                Mathf.Pow(f, 2) * _endPosition;
            Gizmos.DrawSphere(pos, 0.25f);
        }
    }
}
