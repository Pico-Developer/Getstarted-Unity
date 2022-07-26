using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class UITouchEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Action<PointerEventData> onTouchEnter;
    public Action<PointerEventData> onTouchExit;
    public Action<PointerEventData> onTouchDown;
    public Action<PointerEventData> onTouchUp;

    public Action<PointerEventData> onBeginDrag;
    public Action<PointerEventData> onEndDrag;
    public Action<PointerEventData> onDrag;

    public Action<PointerEventData> onPointClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onTouchDown != null && eventData.dragging == false)
        {
            onTouchDown(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onTouchUp != null && eventData.dragging == false)
        {
            onTouchUp(eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onTouchEnter?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onTouchExit?.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onPointClick?.Invoke(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(eventData);
    }
}