using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class View : MonoBehaviour
{
    public Transition Transition = Transition.Move;
    public Direction MoveDirection = Direction.Up;
    public Vector3 ShowPosition;
    public Vector3 CustomHidePosition;
    public float AnimationTime;
    public AnimationCurve Curve;

    CanvasGroup _cg;
    RectTransform _rect;
    Coroutine _activeRoutine;
    private void Start()
    {
        _cg = GetComponent<CanvasGroup>();
        _rect = GetComponent<RectTransform>();
    }
    public bool Show()
    {
        if (_activeRoutine == null)
        {
            gameObject.SetActive(true);
            _cg = GetComponent<CanvasGroup>();
            _rect = GetComponent<RectTransform>();
            switch (Transition)
            {
                case Transition.Fade:
                    _activeRoutine = StartCoroutine(ShowFadeRoutine());
                    return true;
                case Transition.Move:
                    _activeRoutine = StartCoroutine(ShowMoveRoutine());
                    return true;
            }
        }
        return false;
    }
    IEnumerator ShowMoveRoutine()
    {
        Vector3 origin;
        switch (MoveDirection)
        {
            case Direction.Right:
                origin = Screen.width * Vector3.right;
                break;
            case Direction.Left:
                origin = Screen.width * Vector3.left;
                break;
            case Direction.Up:
                origin = Screen.height * Vector3.up;
                break;
            case Direction.Down:
                origin = Screen.height * Vector3.down;
                break;
            case Direction.Custom:
                origin = CustomHidePosition;
                break;
            default:
                origin = Screen.height * Vector3.up;
                break;
        }
        Vector3 target = ShowPosition;
        for (float f = 0; f < AnimationTime; f += Time.deltaTime)
        {
            float t = f / AnimationTime;
            float dis = Vector3.Distance(origin, target);
            Vector3 pos = Vector3.MoveTowards(origin, target, dis * Curve.Evaluate(t));
            _rect.localPosition = pos;
            yield return new WaitForEndOfFrame();
        }
        _rect.localPosition = target;
        _activeRoutine = null;
    }
    IEnumerator ShowFadeRoutine()
    {
        gameObject.SetActive(true);
        for (float f = 0; f < AnimationTime; f += Time.deltaTime)
        {
            float t = f / AnimationTime;
            _cg.alpha = Curve.Evaluate(t);
            yield return new WaitForEndOfFrame();
        }
        _cg.alpha = 1;
        _activeRoutine = null;
    }
    public bool Hide()
    {
        if (_activeRoutine == null)
        {
            gameObject.SetActive(true);
            _cg = GetComponent<CanvasGroup>();
            _rect = GetComponent<RectTransform>();
            switch (Transition)
            {
                case Transition.Fade:
                    _activeRoutine = StartCoroutine(HideFadeRoutine());
                    return true;
                case Transition.Move:
                    _activeRoutine = StartCoroutine(HideMoveRoutine());
                    return true;
            }
        }
        return false;
    }
    IEnumerator HideMoveRoutine()
    {
        Vector3 target;
        switch (MoveDirection)
        {
            case Direction.Right:
                target = Screen.width * Vector3.right;
                break;
            case Direction.Left:
                target = Screen.width * Vector3.left;
                break;
            case Direction.Up:
                target = Screen.height * Vector3.up;
                break;
            case Direction.Down:
                target = Screen.height * Vector3.down;
                break;
            case Direction.Custom:
                target = CustomHidePosition;
                break;
            default:
                target = Screen.height * Vector3.up;
                break;
        }
        Vector3 origin = ShowPosition;
        for (float f = 0; f < AnimationTime; f += Time.deltaTime)
        {
            float t = f / AnimationTime;
            float dis = Vector3.Distance(origin, target);
            Vector3 pos = Vector3.MoveTowards(origin, target, dis * Curve.Evaluate(t));
            _rect.localPosition = pos;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
        _activeRoutine = null;
    }
    IEnumerator HideFadeRoutine()
    {
        for (float f = 0; f < AnimationTime; f += Time.deltaTime)
        {
            float t = 1 - (f / AnimationTime);
            _cg.alpha = Curve.Evaluate(t);
            yield return new WaitForEndOfFrame();
        }
        _cg.alpha = 0;
        gameObject.SetActive(false);
        _activeRoutine = null;
    }
}
public enum Transition { Fade, Move }
public enum Direction { Right, Left, Up, Down, Custom }
