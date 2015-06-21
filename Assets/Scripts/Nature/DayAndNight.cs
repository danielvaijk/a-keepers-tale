using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class DayAndNight : MonoBehaviour
{
    public float cicleRadius;
    public float cicleTime;

    public float dayTemperature;
    public float nightTemperature;

    public Transform sun;
    public Transform moon;

    public Color sunriseAmbienceColor;
    public Color dayAmbienceColor;
    public Color duskAmbienceColor;
    public Color nightAmbienceColor;

    public AudioClip dayAmbienceSound;
    public AudioClip nightAmbienceSound;

    private float circleCircumference = 0f;
    private float currentDistance = 0f;
    private float baseTemperature = 0f;

    private Light sunLight;

    private PlayerStats playerStats = null;

    // Called when the script is loaded or a value is changed in the inspector (Editor only).
    private void OnValidate ()
    {
        sun.localPosition = new Vector3(sun.localPosition.x, sun.localPosition.y, cicleRadius);
    }

    // Called on the frame when a script is enabled.
    private void Start ()
    {
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        circleCircumference = Mathf.PI * cicleRadius * 2;
        sunLight = sun.GetComponent<Light>();
    }

    // Called every frame, if the MonoBehaviour is enabled.
    private void Update ()
    {
        // Attempt to equalize the Player's temperature to the current ambient temperature.
        if (baseTemperature != 0f)
        {
            if (baseTemperature == dayTemperature && playerStats.playerTemperature < dayTemperature)
                playerStats.playerTemperature += 0.05f * Time.deltaTime;

            if (baseTemperature == nightTemperature && playerStats.playerTemperature > nightTemperature)
                playerStats.playerTemperature -= 0.05f * Time.deltaTime;
        }

        // Check if the Suns is rotating outside the <cicleRadius> and set him back.
        if (sun.localPosition.z > cicleRadius)
            sun.localPosition = new Vector3(sun.localPosition.x, sun.localPosition.y, cicleRadius);

        currentDistance += sun.up.magnitude * (circleCircumference / cicleTime) * Time.deltaTime;

        float ciclePercentage = (currentDistance / circleCircumference) * 100;

        if (ciclePercentage >= 100f)
        {
            currentDistance = 0f;
            ciclePercentage = 0f;
        }

        sun.position += sun.up * (circleCircumference / cicleTime) * Time.deltaTime;
        sun.rotation = Quaternion.LookRotation(transform.position - sun.position, sun.up);

        if (ciclePercentage < 20f)
        {
            // Sunrise -> Day.
            float transitionRate = ciclePercentage / 10;

            sunLight.intensity = Mathf.Lerp(sunLight.intensity, 0.8f, transitionRate);

            RenderSettings.ambientLight = Color.Lerp
                                          (
                                                sunriseAmbienceColor,
                                                dayAmbienceColor,
                                                transitionRate
                                          );

            RenderSettings.skybox.SetColor
                                  (
                                      "_Tint",
                                      Color.Lerp
                                      (
                                           sunriseAmbienceColor,
                                           dayAmbienceColor,
                                           transitionRate
                                      )
                                  );

            RenderSettings.fogColor = Color.Lerp
                                      (
                                          sunriseAmbienceColor,
                                          dayAmbienceColor,
                                          transitionRate
                                      );

            if (!this.GetComponent<AudioSource>().isPlaying)
            {
                this.GetComponent<AudioSource>().clip = dayAmbienceSound;
                this.GetComponent<AudioSource>().Play();
            }
        }
        else if (ciclePercentage >= 35f && ciclePercentage < 45f)
        {
            // Day -> Dusk.
            float transitionRate = (ciclePercentage - 35f) / 5;

            sunLight.intensity = Mathf.Lerp(sunLight.intensity, 0.5f, transitionRate);

            RenderSettings.ambientLight = Color.Lerp
                                          (
                                                dayAmbienceColor,
                                                duskAmbienceColor,
                                                transitionRate
                                          );

            RenderSettings.skybox.SetColor
                                  (
                                      "_Tint",
                                      Color.Lerp
                                      (
                                           dayAmbienceColor,
                                           duskAmbienceColor,
                                           transitionRate
                                      )
                                  );

            RenderSettings.fogColor = Color.Lerp
                                      (
                                           dayAmbienceColor,
                                           duskAmbienceColor,
                                           transitionRate
                                      );

            if (!this.GetComponent<AudioSource>().isPlaying)
            {
                this.GetComponent<AudioSource>().clip = dayAmbienceSound;
                this.GetComponent<AudioSource>().Play();
            }
        }
        else if (ciclePercentage >= 45f && ciclePercentage < 90f)
        {
            // Dusk -> Night.
            float transitionRate = (ciclePercentage - 45f) / 10;

            sunLight.intensity = Mathf.Lerp(sunLight.intensity, 0f, transitionRate);

            RenderSettings.ambientLight = Color.Lerp
                                          (
                                                duskAmbienceColor,
                                                nightAmbienceColor,
                                                transitionRate
                                          );

            RenderSettings.skybox.SetColor
                                  (
                                      "_Tint",
                                      Color.Lerp
                                      (
                                           duskAmbienceColor,
                                           nightAmbienceColor,
                                           transitionRate
                                      )
                                  );

            RenderSettings.fogColor = Color.Lerp
                                      (
                                           duskAmbienceColor,
                                           nightAmbienceColor,
                                           transitionRate
                                      );

            RenderSettings.fogStartDistance = Mathf.Lerp(200f, -200f, transitionRate);

            baseTemperature = Mathf.Lerp(dayTemperature, nightTemperature, transitionRate);

            FadeAudioClip(dayAmbienceSound, nightAmbienceSound, transitionRate);
        }
        else if (ciclePercentage >= 95f)
        {
            // Night -> Sunrise.
            float transitionRate = (ciclePercentage - 95f) / 10;

            Light sunLight = sun.GetComponent<Light>();

            sunLight.intensity = Mathf.Lerp(sunLight.intensity, 0.5f, transitionRate);

            RenderSettings.ambientLight = Color.Lerp
                                          (
                                                nightAmbienceColor,
                                                sunriseAmbienceColor,
                                                transitionRate
                                          );

            RenderSettings.skybox.SetColor
                                  (
                                      "_Tint",
                                      Color.Lerp
                                      (
                                           nightAmbienceColor,
                                           sunriseAmbienceColor,
                                           transitionRate
                                      )
                                  );

            RenderSettings.fogColor = Color.Lerp
                                      (
                                           nightAmbienceColor,
                                           sunriseAmbienceColor,
                                           transitionRate
                                      );

            RenderSettings.fogStartDistance = Mathf.Lerp(-200f, 200f, transitionRate);

            baseTemperature = Mathf.Lerp(nightTemperature, dayTemperature, transitionRate);

            FadeAudioClip(nightAmbienceSound, dayAmbienceSound, transitionRate);
        }
    }

    public void FadeAudioClip (AudioClip from, AudioClip to, float t)
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource.isPlaying && audioSource.clip != to)
        {
            if (audioSource.volume > 0.1f)
            {
                // Fade the volume down so we can change the Audio Clips.
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0.1f, t);
            }
            else
            {
                audioSource.clip = to;
                audioSource.Play();

                audioSource.volume = Mathf.Lerp(audioSource.volume, 0.6f, t);
            }
        }
    }
}