using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestCameraRay : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Camera ViewPoint;
    public RawImage Image;
    public Canvas canvas;
    public bool isPressed = false;
    public RectTransform Point;

    public float dpi;

    public Vector3 PositionImage;
    public Vector2 ImageSize;

    private void OnValidate()
    {
        dpi = Screen.dpi;
    }

    private void Update()
    {
        if (ViewPoint)
        {
            var scaleFactor = canvas.scaleFactor;
            var imageExtends = new Vector3(Image.rectTransform.sizeDelta.x, Image.rectTransform.sizeDelta.y, 0.0f) * scaleFactor * 0.5f;
            ImageSize = imageExtends * 2.0f;
            var mousePosition = Input.mousePosition - (transform.position - imageExtends);
            var uvMousePosition = new Vector3(mousePosition.x / (Image.rectTransform.sizeDelta.x * scaleFactor),
                                              mousePosition.y / (Image.rectTransform.sizeDelta.y * scaleFactor), 
                                              0.0f);
            PositionImage = uvMousePosition;
            var ray = ViewPoint.ViewportPointToRay(uvMousePosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.yellow);

            if (Point)
            {
                Point.position = transform.position - imageExtends;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}
