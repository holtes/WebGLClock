using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseGrabRotate : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public bool IsRotatable = false;
    [SerializeField] private Transform _rotateObj;
    [SerializeField] private float _rotationSpeed = 1f;
    private bool _isMouseOver;

    private void Update()
    {
        if (IsRotatable)
        {
            if (_isMouseOver && Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float rotationZ = mouseX * _rotationSpeed;
                _rotateObj.Rotate(0, 0, -rotationZ);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isMouseOver = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isMouseOver = false; 
    }
}
