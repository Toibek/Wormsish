using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class InputHandler : MonoBehaviour
{

    public VectorDelegate OnMovementStay;
    public VectorDelegate OnRotationStay;
    public EmptyDelegate OnPassTurn;
    public EmptyDelegate OnShowMap;


    public delegate void EmptyDelegate();
    public delegate void VectorDelegate(Vector2 vector2);

    private Vector2 _movement = Vector2.zero;
    private Coroutine _continousMoveRoutine;
    private Vector2 _rotation = Vector2.zero;
    private Coroutine _continousRotRoutine;
    public void Move(InputAction.CallbackContext context)
    {
        if (context.started && _continousMoveRoutine == null)
        {
            _movement = (Vector2)context.ReadValueAsObject();
            _continousMoveRoutine = StartCoroutine(ContinousMoving());
        }
        else if (context.performed)
            _movement = (Vector2)context.ReadValueAsObject();
        else if (context.canceled && _continousMoveRoutine != null)
        {
            StopCoroutine(_continousMoveRoutine);
            _continousMoveRoutine = null;
        }
    }
    IEnumerator ContinousMoving()
    {
        while (true)
        {
            OnMovementStay?.Invoke(_movement);
            yield return new WaitForEndOfFrame();
        }
    }
    public void Look(InputAction.CallbackContext context)
    {
        if (context.performed)
            _rotation = (Vector2)context.ReadValueAsObject();
        else if (context.canceled)
            _rotation = Vector2.zero;
    }
    public void Primary(InputAction.CallbackContext context)
    {

    }
    public void Secondary(InputAction.CallbackContext context)
    {
        if (context.started && _continousRotRoutine == null)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _continousRotRoutine = StartCoroutine(ContinousRotation());
        }
        else if (context.canceled && _continousRotRoutine != null)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            StopCoroutine(_continousRotRoutine);
            _continousRotRoutine = null;
        }
    }
    IEnumerator ContinousRotation()
    {
        while (true)
        {
            OnRotationStay?.Invoke(_rotation);
            yield return new WaitForEndOfFrame();
        }
    }
    public void ShowMap(InputAction.CallbackContext context)
    {
        if (context.started) OnShowMap?.Invoke();
    }
    public void PassTurn(InputAction.CallbackContext context)
    {
        if (context.started)
            OnPassTurn?.Invoke();
    }
}
