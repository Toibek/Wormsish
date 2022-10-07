using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    internal Unit Unit;

    [SerializeField] LayerMask _layerMask;
    [SerializeField] int _jumpHeight;
    [SerializeField] float _liftHeight;
    [SerializeField] float _moveTime;

    Coroutine _moveRoutine;

    /// <summary>
    /// Check if movement is avaliable and return wether or not it could start, start MoveIE if as long as the moveroutine is not started
    /// </summary>
    /// <param name="direction">Direction of movement</param>
    /// <returns></returns>
    public bool Move(Vector2 direction)
    {
        bool ret = false;
        if (_moveRoutine == null)
        {
            Vector3 movement = Vector3.zero;
            Vector3 forward = transform.position - Unit.Camera.position;
            forward.y = 0;
            movement += forward * direction.y;

            Vector3 right = new(forward.z, forward.y, -forward.x);
            right.y = 0;
            movement += right * direction.x;
            movement.Normalize();

            movement.x = Mathf.Round(movement.x);
            movement.z = Mathf.Round(movement.z);

            Vector3 endPosition = transform.position;
            Ray heightCheckRay = new(transform.position + new Vector3(0, _jumpHeight, 0), movement);
            if (!Physics.Raycast(heightCheckRay, 1f, _layerMask, QueryTriggerInteraction.Ignore))
            {
                Ray positioningRay = new(transform.position + movement + Vector3.up * 3, Vector3.down);
                Debug.DrawRay(positioningRay.origin, positioningRay.direction + new Vector3(0, -1, 0) * 20);
                if (Physics.Raycast(positioningRay, out RaycastHit hit, 20, _layerMask, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
                {
                    endPosition = hit.point + new Vector3(0, .5f, 0);
                    ret = true;
                }
            }
            _moveRoutine = StartCoroutine(MoveIE(transform.position, endPosition));
        }
        return ret;
    }
    /// <summary>
    /// Update the rotation of the unit relative to the camera
    /// </summary>
    public void Rotation(float rot)
    {
        transform.rotation = Quaternion.Euler(0, rot, 0);
    }
    /// <summary>
    /// run movement between start and end position through a Bézier curve
    /// </summary>
    /// <param name="startPosition">the origin of the object</param>
    /// <param name="endPosition">the final position</param>
    IEnumerator MoveIE(Vector3 startPosition, Vector3 endPosition)
    {
        float maxHeight = Mathf.Max(startPosition.y, endPosition.y);
        for (float f = 0; f < _moveTime; f += Time.deltaTime)
        {
            Vector3 pos =
                Mathf.Pow(1 - (f / _moveTime), 2) * startPosition +
                2 * Mathf.Pow(1 - (f / _moveTime), 2) * (f / _moveTime) * (new Vector3(startPosition.x, maxHeight + _liftHeight, startPosition.z)) +
                2 * (1 - (f / _moveTime)) * Mathf.Pow((f / _moveTime), 2) * (new Vector3(endPosition.x, maxHeight + _liftHeight, endPosition.z)) +
                Mathf.Pow((f / _moveTime), 2) * endPosition;

            transform.position = pos;
            yield return new WaitForFixedUpdate();
        }
        transform.position = endPosition;
        _moveRoutine = null;
    }
}
