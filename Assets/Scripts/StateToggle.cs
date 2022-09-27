using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StateToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
{
    public Image StateImage;
    [Space]
    public List<Sprite> States;
    public int CurrentState = 0;
    public IntEvent OnStateChange;

    public void GraphicUpdateComplete()
    {

    }

    public void LayoutComplete()
    {

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (++CurrentState >= States.Count)
            CurrentState = 0;
        StateImage.sprite = States[CurrentState];
        OnStateChange?.Invoke(CurrentState);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (++CurrentState >= States.Count)
            CurrentState = 0;
        StateImage.sprite = States[CurrentState];
        OnStateChange?.Invoke(CurrentState);
    }

    public void Rebuild(CanvasUpdate executing)
    {

    }
    protected override void Start()
    {
        StateImage.sprite = States[CurrentState];
    }
}
[System.Serializable]
public class IntEvent : UnityEvent<int> { }
