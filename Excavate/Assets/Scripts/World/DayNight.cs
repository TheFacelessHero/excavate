using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DayNight : MonoBehaviour
{
    public float dayLength = 600;
    public Gradient daySkyColors;
    public float dayIntensity = 0.9f;
    public float nightLength = 600;
    public Gradient nightSkyColors;
    public float nightIntensity = 0.3f;
    public float currentTime;
    public bool isDayTime = true;
    public float timeSpeedController = 1f;
    public float depthChange = 30;



    public UnityEngine.Rendering.Universal.Light2D sunMoonObj;
    public UnityEngine.Rendering.Universal.Light2D generalLightObj;

    void Update()
    {
        currentTime += timeSpeedController * Time.deltaTime;

        if (isDayTime) 
        {
            Camera.main.backgroundColor = daySkyColors.Evaluate((currentTime / dayLength)*1);
            sunMoonObj.color = daySkyColors.Evaluate((currentTime / dayLength) * 1);
            generalLightObj.color = daySkyColors.Evaluate((currentTime / dayLength) * 1);
            sunMoonObj.intensity = dayIntensity - Mathf.Clamp((Mathf.Abs((transform.position.y / depthChange))), 0, 10);
            generalLightObj.intensity = dayIntensity - Mathf.Clamp((Mathf.Abs((transform.position.y / depthChange))),0,10);
            transform.eulerAngles = new Vector3(0, 0, (currentTime / dayLength) * 180 - 90);

            if (currentTime >= dayLength)
            {
                currentTime = 0;
                isDayTime = false;
            }
        }
        else
        {
            Camera.main.backgroundColor = nightSkyColors.Evaluate((currentTime / nightLength) * 1);
            sunMoonObj.color = nightSkyColors.Evaluate((currentTime / nightLength) * 1);
            sunMoonObj.intensity = nightIntensity - Mathf.Clamp((Mathf.Abs((transform.position.y / depthChange))), 0, 10);
            generalLightObj.color = nightSkyColors.Evaluate((currentTime / nightLength) * 1);
            generalLightObj.intensity = nightIntensity - Mathf.Clamp((Mathf.Abs((transform.position.y / depthChange))), 0, 10);
            transform.eulerAngles = new Vector3(0, 0, (currentTime / nightLength) * 180 -90);

            if (currentTime >= nightLength)
            {
                currentTime = 0;
                isDayTime = true;
            }
        }

    }
}
