using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderListener : MonoBehaviour
{
    public Slider sliderObj;
    public string item;
    public string action;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void sliderListener()
    {
        sliderObj.value = (int)sliderObj.value;
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Type"] = "Player Update";
        payload["Action"] = "Player HUD Menu";
        payload["Name"] = name;
        payload["Item"] = item;
        payload["Menu"] = "Update Slider";
        payload["Value"] = sliderObj.value.ToString();
        NetworkMain.broadcastAction(payload);
    }

    public void configSlider(int in_min, int in_max)
    {
        sliderObj.minValue = in_min;
        sliderObj.maxValue = in_max;
    }
}
