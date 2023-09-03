using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool allowToMove;
    private Transform parent;
    private Vector3 startParentPosition;
    private Vector3 startMousePosition;

    private void Awake()
    {
        allowToMove = false;
        parent = transform.parent;
    }

    private void Update()
    {
        if (allowToMove)
        {
            parent.position = startParentPosition + (Input.mousePosition - startMousePosition);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        startParentPosition = parent.position;
        startMousePosition = Input.mousePosition;
        allowToMove = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        allowToMove = false;
    }
}
