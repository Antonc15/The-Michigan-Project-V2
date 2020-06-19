using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DayCycle : MonoBehaviour
{
    [Header("Values")]
    [Range(0,24)]
    public float timeOfDay;
    public float minutesPerCycle = 60f;

    [Header("Objects")]
    public AnimationCurve startsCurve;
    public Light sun;
    public Light moon;
    public Volume skyVolume;

    [Header("Audio")]
    public float maxVolume = 0.3f;
    public float audioFadeSpeed;
    public AudioSource nightAmbience;
    public AudioSource dayAmbience;

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
        AmbientSounds();
    }


    void UpdateTime()
    {
        float alpha = timeOfDay / 24.0f;
        float sunRotation = Mathf.Lerp(-90, 270, alpha);

        float moonRotation = sunRotation - 180f;

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

        dayAmbience.Play();
        nightAmbience.Stop();

        sun.shadows = LightShadows.Soft;
        moon.shadows = LightShadows.None;
    }

    void StartNight()
    {
        isNight = true;

        dayAmbience.Stop();
        nightAmbience.Play();

        sun.shadows = LightShadows.None;
        moon.shadows = LightShadows.Soft;
    }

    void AmbientSounds()
    {
        if (isNight)
        {

            //ENABLES ---> night ambient sound
            //DISABLES ---> day ambient sound

            if (nightAmbience.volume < maxVolume)
            {
                nightAmbience.volume += Time.deltaTime * (audioFadeSpeed / 100);
            }

            if (dayAmbience.volume > 0)
            {
                dayAmbience.volume -= Time.deltaTime * (audioFadeSpeed / 100);
            }
        }
        else
        {

            //ENABLES ---> day ambient sound
            //DISABLES ---> night ambient sound

            if (dayAmbience.volume < maxVolume)
            {
                dayAmbience.volume += Time.deltaTime * (audioFadeSpeed / 100);
            }

            if (nightAmbience.volume > 0)
            {
                nightAmbience.volume -= Time.deltaTime * (audioFadeSpeed / 100);
            }
        }
    }
}
