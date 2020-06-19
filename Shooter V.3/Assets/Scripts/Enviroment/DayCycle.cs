using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DayCycle : MonoBehaviour
{
    [Range(0,24)]
    public float timeOfDay;

    public float minutesPerCycle = 60f;

    public AnimationCurve startsCurve;

    public Light sun;
    public Light moon;
    public Volume skyVolume;

    float OrbitSpeed;
    bool isNight;
    PhysicallyBasedSky sky;

    void Start()
    {
        skyVolume.profile.TryGet(out sky);
    }

    void Update()
    {
        OrbitSpeed = 24  / (minutesPerCycle * 60);

        timeOfDay += Time.deltaTime * OrbitSpeed;

        if(timeOfDay > 24)
        {
            timeOfDay = 0;
        }

        UpdateTime();
    }


    void UpdateTime()
    {
        float alpha = timeOfDay / 24.0f;
        float sunRotation = Mathf.Lerp(-90, 270, alpha);

        float moonRotation = sunRotation - 180;

        sun.transform.rotation = Quaternion.Euler(sunRotation, 45f, 90f);
        moon.transform.rotation = Quaternion.Euler(moonRotation, 45f, 90f);

        sky.spaceEmissionMultiplier.value = startsCurve.Evaluate(alpha) * 1000.0f;

        CheckNightDayTransition();
    }

    void CheckNightDayTransition()
    {
        if (isNight)
        {
            if (moon.transform.rotation.eulerAngles.x > 180)
            {
                StartDay();
            }
        }
        else
        {
            if (sun.transform.rotation.eulerAngles.x > 180)
            {
                StartNight();
            }
        }
    }

    void StartDay()
    {
        isNight = false;
        sun.shadows = LightShadows.Soft;
        moon.shadows = LightShadows.None;
    }

    void StartNight()
    {
        isNight = true;
        sun.shadows = LightShadows.None;
        moon.shadows = LightShadows.Soft;
    }
}
