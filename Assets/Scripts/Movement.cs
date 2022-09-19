using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    internal Transform CamTransform;
    internal PlayerManager PlayerManager;
    internal Team Team;

    [SerializeField] LayerMask _layerMask;
    [SerializeField] int _jumpHeight;
    [SerializeField] float _liftHeight;
    [SerializeField] float _moveTime;
    Vector3 _startPosition = Vector3.zero;
    Vector3 _endPosition = Vector3.forward;
    Coroutine _moveRoutine;
    internal Unit Unit;

    public bool Move(Vector2 direction)
    {
        bool ret = false;

        Vector3 movement = Vector3.zero;
        Vector3 camForward = CamTransform.forward;
        camForward.y = 0;
        movement += camForward * direction.y;

        Vector3 camRight = CamTransform.right;
        camRight.y = 0;
        movement += camRight * direction.x;
        movement.Normalize();
        movement.x = Mathf.Round(movement.x);
        movement.z = Mathf.Round(movement.z);

        if (_moveRoutine == null)
        {
            _startPosition = transform.position;
            _endPosition = transform.position;
            Ray heightCheckRay = new(transform.position + new Vector3(0, _jumpHeight, 0), movement);
            if (!Physics.Raycast(heightCheckRay, 1f, _layerMask))
            {
                Ray positioningRay = new(transform.position + movement + Vector3.up * 3, Vector3.down);
                Debug.DrawRay(positioningRay.origin, positioningRay.direction + new Vector3(0, -1, 0) * 20);
                if (Physics.Raycast(positioningRay, out RaycastHit hit, 20, _layerMask, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
                {
                    _endPosition = hit.point + new Vector3(0, .5f, 0);
                    ret = true;
                }
            }
            _moveRoutine = StartCoroutine(MoveIE());
        }
        return ret;
    }
    public void Rotation()
    {
        Vector3 dif = (transform.position - CamTransform.position).normalized;
        float rot = Mathf.Atan2(dif.x, dif.z) * Mathf.Rad2Deg;
        rot = Mathf.Round(rot / 45) * 45;
        transform.rotation = Quaternion.Euler(0, rot, 0);
    }
    IEnumerator MoveIE()
    {
        for (float f = 0; f < _moveTime; f += Time.deltaTime)
        {
            Vector3 pos =
                Mathf.Pow(1 - (f / _moveTime), 2) * _startPosition +
                2 * Mathf.Pow(1 - (f / _moveTime), 2) * (f / _moveTime) * (_startPosition + new Vector3(0, _liftHeight)) +
                2 * (1 - (f / _moveTime)) * Mathf.Pow((f / _moveTime), 2) * (_endPosition + new Vector3(0, _liftHeight)) +
                Mathf.Pow((f / _moveTime), 2) * _endPosition;

            transform.position = pos;
            yield return new WaitForFixedUpdate();
        }
        transform.position = _endPosition;
        _moveRoutine = null;
    }
    private void OnDrawGizmosSelected()
    {
        for (float f = 0; f < 1; f += 0.05f)
        {
            Vector3 pos =
                Mathf.Pow(1 - f, 2) * _startPosition +
                2 * Mathf.Pow(1 - f, 2) * f * (_startPosition + new Vector3(0, _liftHeight)) +
                2 * (1 - f) * Mathf.Pow(f, 2) * (_endPosition + new Vector3(0, _liftHeight)) +
                Mathf.Pow(f, 2) * _endPosition;
            Gizmos.DrawSphere(pos, 0.25f);
        }
    }
}
