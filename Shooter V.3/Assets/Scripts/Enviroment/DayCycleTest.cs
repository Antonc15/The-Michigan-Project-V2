using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DayCycleTest : MonoBehaviour
{
    [Header("Values")]
    [Range(0, 24)]
    public float timeOfDay;
    public float spaceFadeSpeed;
    public float spaceMaxFade;
    public float minutesPerCycle = 60f;

    [Header("Objects")]
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
        OrbitSpeed = 24 / (minutesPerCycle * 60);

        timeOfDay += Time.deltaTime * OrbitSpeed;

        if (timeOfDay > 24)
        {
            timeOfDay = 0;
        }

        UpdateTime();
        AmbientSounds();
    }


    void UpdateTime()
    {
        //***********************//
        //                       //
        //    Assigning floats   // 
        //          \/           //
        //***********************//

        float alpha = timeOfDay / 24.0f;
        float positionInSky;
        float moonX;
        float sunX;

        //****************************************************//
        //                                                    //
        //    Changing Sun & Moon Location depending on time. //
        //                      \/                            //
        //****************************************************//

        if (timeOfDay > 12)
        {
            // THIS IS DAY!

            positionInSky = alpha * 2.0f - 1;
            moonX = Mathf.Lerp(360, 180, positionInSky);
            sunX = 160 * positionInSky * positionInSky - 160 * positionInSky + 180;
        }
        else
        {
            // THIS IS NIGHT!

            positionInSky = alpha * 2.0f;
            moonX = 160 * positionInSky * positionInSky - 160 * positionInSky + 180;
            sunX = Mathf.Lerp(360, 180, positionInSky);
        }

        //****************************************************//
        //                                                    //
        //           Executing the sun & moon location.       //
        //                      \/                            //
        //****************************************************//

        float moonY = Mathf.Lerp(180, -180, alpha);
        float sunY = moonY - 180;

        sun.transform.rotation = Quaternion.Euler(sunX, sunY, 90f);
        moon.transform.rotation = Quaternion.Euler(moonX, moonY, 90f);

        sky.spaceRotation.value = new Vector3(0,alpha * 360,0);

        //****************************************************//
        //                                                    //
        //              Running other methods                 //
        //                      \/                            //
        //****************************************************//
        CheckNightDayTransition();
        SpaceFadeAmount();
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

        if (dayAmbience != null)
        {
            dayAmbience.Play();
        }

        if (nightAmbience != null)
        {
            nightAmbience.Stop();
        }

        sun.shadows = LightShadows.Soft;
        moon.shadows = LightShadows.None;
    }

    void StartNight()
    {
        isNight = true;

        if (dayAmbience != null)
        {
            dayAmbience.Stop();
        }

        if (nightAmbience != null)
        {
            nightAmbience.Play();
        }

        sun.shadows = LightShadows.None;
        moon.shadows = LightShadows.Soft;
    }

    void AmbientSounds()
    {
        if (isNight)
        {

            //ENABLES ---> night ambient sound
            //DISABLES ---> day ambient sound

            if (nightAmbience != null && nightAmbience.volume < maxVolume)
            {
                nightAmbience.volume += Time.deltaTime * (audioFadeSpeed / 100);
            }

            if (dayAmbience != null && dayAmbience.volume > 0)
            {
                dayAmbience.volume -= Time.deltaTime * (audioFadeSpeed / 100);
            }
        }
        else
        {

            //ENABLES ---> day ambient sound
            //DISABLES ---> night ambient sound

            if (dayAmbience != null && dayAmbience.volume < maxVolume)
            {
                dayAmbience.volume += Time.deltaTime * (audioFadeSpeed / 100);
            }

            if (nightAmbience != null && nightAmbience.volume > 0)
            {
                nightAmbience.volume -= Time.deltaTime * (audioFadeSpeed / 100);
            }
        }
    }

    void SpaceFadeAmount()
    {
        if (isNight)
        {
            if(sky.spaceEmissionMultiplier.value < spaceMaxFade)
                sky.spaceEmissionMultiplier.value += Time.deltaTime * spaceFadeSpeed;
        }
        else
        {
            if(sky.spaceEmissionMultiplier.value > 0)
                sky.spaceEmissionMultiplier.value -= Time.deltaTime * spaceFadeSpeed;
        }
    }
}
