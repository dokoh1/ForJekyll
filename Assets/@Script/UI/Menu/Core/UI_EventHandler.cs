using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler
{
    public event Action<PointerEventData> OnClickHandler;
    public event Action<PointerEventData> OnPointerDownHandler;
    public event Action<PointerEventData> OnPointerUpHandler;
    public event Action<PointerEventData> OnDragHandler;
    public event Action<PointerEventData> OnBeginDragHandler;
    public event Action<PointerEventData> OnEndDragHandler;
    public event Action<PointerEventData> OnLongPressHandler; // 롱프레스 이벤트 핸들러
    public event Action<PointerEventData> OnEnterHandler;

    private bool _isDragging = false;
    private bool _isLongPressTriggered = false;
    private bool _isClickAllowed = true;
    private PointerEventData _currentEventData;
    private float _pressStartTime;

    private void Update()
    {
        if (_isDragging)
        {
            OnDragHandler?.Invoke(_currentEventData);
        }

        // 롱프레스 체크
        if (!_isLongPressTriggered && _currentEventData != null && (Time.time - _pressStartTime) >= 0.15f)
        {
            _isLongPressTriggered = true;
            _isClickAllowed = false; // 롱프레스가 발생하면 클릭을 허용하지 않음
            OnLongPressHandler?.Invoke(_currentEventData);
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isClickAllowed)
        {
            OnClickHandler?.Invoke(eventData);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressStartTime = Time.time;
        _isLongPressTriggered = false;
        _isClickAllowed = true; // 초기화하여 클릭을 허용하도록 설정
        _currentEventData = eventData;
        OnPointerDownHandler?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _currentEventData = null;
        OnPointerUpHandler?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중 포인터를 가만히 있으면 이벤트가 발생하지 않기 때문에 Update에서 따로 이벤트 발생시킴
        _currentEventData = eventData;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _currentEventData = eventData;
        _isClickAllowed = false;
        OnBeginDragHandler?.Invoke(eventData);
        _isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
        _isLongPressTriggered = false;
        OnEndDragHandler?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnterHandler?.Invoke(eventData);
    }
}
