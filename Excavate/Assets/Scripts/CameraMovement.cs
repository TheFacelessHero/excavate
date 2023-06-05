using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float zoomSpeed = 1;
    [SerializeField] float minZoom = 1;
    [SerializeField] float maxZoom = 20;
    Vector3 panOrgin;
    float currentZoom = 5;
    

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            panOrgin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
        if (Input.GetMouseButton(2))
        {
            Vector3 difference = panOrgin - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.position += difference;
        }

        currentZoom = Mathf.Clamp(currentZoom + (-Input.mouseScrollDelta.y * zoomSpeed), minZoom, maxZoom);
        Camera.main.orthographicSize = currentZoom;

    }
}
