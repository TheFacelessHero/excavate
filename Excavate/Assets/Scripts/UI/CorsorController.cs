using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorsorController : MonoBehaviour
{
    public bool onCanvas = false;
    private void Update()
    {
        Vector2 mousePos = new Vector2(0,0);
        if (onCanvas)
        {
            mousePos = Input.mousePosition;
        }
        else
        {
            mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        }
        
        //mousePos.z = 0;
        //Debug.Log("X: " + mousePos.x + " Y: " + mousePos.y + " Z: " + mousePos.z);
        transform.position = mousePos;
    }
    private void OnEnable()
    {
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }
}
