using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSystem : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLength;
    [Range(0.0f, 1.0f)]
    public float startTime;
    private float timeRate;
    public Vector3 noon;

    public Text timeText;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;


    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Settings")]
    public AnimationCurve lightingIntensityMultipler;
    public AnimationCurve reflectionIntensityMultipler;

    public EntityManager em;

    private bool timeFreeze = true;
    public AudioSource footstepAuto;

    // Start is called before the first frame update
    void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    public int getMinute()
    {
        return (int)(time * 1440);
    }

    public void setTime(float getServerTime)
    {
        time = (getServerTime % 600) / 600;
    }

    // Update is called once per frame
    void Update()
    {
        if (!timeFreeze)
            time += timeRate * Time.deltaTime;
        if (time >= 1.0f)
        {
            time = 0f;
//            em.npcCounter = 6;
        }

        int minute = getMinute();

        timeText.text = (minute/60).ToString("D2") + " : " + (minute % 60).ToString("D2");

        sun.transform.eulerAngles = (time) * noon * 4.0f;
        moon.transform.eulerAngles = (time - .5f) * noon * 4.0f;

        sun.intensity = sunIntensity.Evaluate(time);
        moon.intensity = moonIntensity.Evaluate(time);

        sun.color = sunColor.Evaluate(time);
        moon.color = moonColor.Evaluate(time);

        if (sun.intensity == 0 && sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(false);
        else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(true);

        if (moon.intensity == 0 && moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(false);
        else if (moon.intensity > 0 && !moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(true);

        RenderSettings.ambientIntensity = lightingIntensityMultipler.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultipler.Evaluate(time);
    }
}
