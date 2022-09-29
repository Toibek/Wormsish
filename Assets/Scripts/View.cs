using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class View : MonoBehaviour
{
    public StartBehaviour OnStart = StartBehaviour.Hide;
    public TransitionType Transition = TransitionType.Move;
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
        switch (OnStart)
        {
            case StartBehaviour.Show:
                _rect.anchoredPosition = ShowPosition;
                break;
            case StartBehaviour.ShowAnimation:
                Show();
                break;
            case StartBehaviour.Hide:
                gameObject.SetActive(false);
                break;
        }
    }
    public void ToggleView()
    {
        if (gameObject.activeInHierarchy) Hide();
        else Show();
    }
    public void Show()
    {
        if (_activeRoutine == null)
        {
            gameObject.SetActive(true);
            _cg = GetComponent<CanvasGroup>();
            _rect = GetComponent<RectTransform>();
            switch (Transition)
            {
                case TransitionType.Fade:
                    _activeRoutine = StartCoroutine(ShowFadeRoutine());
                    break;
                case TransitionType.Move:
                    _activeRoutine = StartCoroutine(ShowMoveRoutine());
                    break;
            }
        }
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
            _rect.anchoredPosition = pos;
            yield return new WaitForEndOfFrame();
        }
        _rect.anchoredPosition = target;
        _activeRoutine = null;
    }
    IEnumerator ShowFadeRoutine()
    {
        gameObject.SetActive(true);
        _rect.anchoredPosition = ShowPosition;
        for (float f = 0; f < AnimationTime; f += Time.deltaTime)
        {
            float t = f / AnimationTime;
            _cg.alpha = Curve.Evaluate(t);
            yield return new WaitForEndOfFrame();
        }
        _cg.alpha = 1;
        _activeRoutine = null;
    }
    public void Hide()
    {
        if (_activeRoutine == null)
        {
            gameObject.SetActive(true);
            _cg = GetComponent<CanvasGroup>();
            _rect = GetComponent<RectTransform>();
            switch (Transition)
            {
                case TransitionType.Fade:
                    _activeRoutine = StartCoroutine(HideFadeRoutine());
                    break;
                case TransitionType.Move:
                    _activeRoutine = StartCoroutine(HideMoveRoutine());
                    break;
            }
        }
    }
    IEnumerator HideMoveRoutine()
    {
        Vector3 target;
        switch (MoveDirection)
        {
            case Direction.Right:
                target = 2 * Screen.width * Vector3.right;
                break;
            case Direction.Left:
                target = 2 * Screen.width * Vector3.left;
                break;
            case Direction.Up:
                target = 2 * Screen.height * Vector3.up;
                break;
            case Direction.Down:
                target = 2 * Screen.height * Vector3.down;
                break;
            case Direction.Custom:
                target = CustomHidePosition;
                break;
            default:
                target = 2 * Screen.height * Vector3.up;
                break;
        }
        Vector3 origin = ShowPosition;
        for (float f = 0; f < AnimationTime; f += Time.deltaTime)
        {
            float t = f / AnimationTime;
            float dis = Vector3.Distance(origin, target);
            Vector3 pos = Vector3.MoveTowards(origin, target, dis * Curve.Evaluate(t));
            _rect.anchoredPosition = pos;
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
    public enum TransitionType { Fade, Move }
    public enum Direction { Right, Left, Up, Down, Custom }
    public enum StartBehaviour { Show, ShowAnimation, Hide }
}
