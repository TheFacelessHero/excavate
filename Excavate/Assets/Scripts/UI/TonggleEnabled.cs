using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TonggleEnabled : MonoBehaviour
{
    public GameObject toToggle;
    public KeyCode toggleKey;
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleObject(toToggle);
        }
    }

    public void ToggleObject(GameObject toggle)
    {
        toggle.SetActive(!toggle.activeSelf);
    }
}
