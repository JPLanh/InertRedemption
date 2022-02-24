using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Flash light component of a gear
 */

public class BodyFlashLightAddon : MonoBehaviour, IAddon
{

    private string addonName = "Basic Flash Light";
    private int addonLevel = 1;
    public float lightDistance;
    public float lightIntensity;
    public float chargeCapacity;
    public float chargeUsageRate;
    public float chargeUseageAmt;
    public float rechageRate;
    public float rechargeAmt;
    public float charge;
    public float actionTimer;

    [SerializeField]
    private Light lightsource;
    private Dictionary<string, int> requirements;

    public AudioSource lightOnAudio;
    public AudioSource lightOffAudio;

    public int getLevel()
    {
        return addonLevel;
    }

    public string getName()
    {
        return addonName;
    }

    public Dictionary<string, int> getRequirements()
    {
        requirements = new Dictionary<string, int>();
        switch (addonLevel)
        {
            case 1:
                requirements.Add("Log", 5);
                break;
            case 2:
                requirements.Add("Log", 5);
                requirements.Add("Stone", 5);
                break;
            case 3:
                requirements.Add("Log", 10);
                requirements.Add("Stone", 10);
                break;
        }
        return requirements;
    }

    public void updateLevel(int getVal)
    {
        chargeCapacity += 25f;
        rechageRate += .5f;
        rechargeAmt += 1;
        addonLevel += getVal;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lightsource.enabled)
        {
            if (Time.time > actionTimer)
            {
                actionTimer = Time.time + chargeUsageRate;
                charge -= chargeUseageAmt;
                if (charge < chargeUseageAmt) lightsource.enabled = false;
            }
        } else
        {

            if (Time.time > actionTimer)
            {
                if (charge < chargeCapacity)
                {
                    charge += rechargeAmt;
                    actionTimer = Time.time + rechageRate;
                }
            }
        }
    }

    public void toggle()
    {
        if (lightsource.enabled)
        {
            lightsource.enabled = false;
            lightOffAudio.Play();
        }
        else
        {
            lightsource.enabled = true;
            lightOnAudio.Play();
        }
    }

    public string getInfo()
    {
        return "Emits a light on front of where you are looking.";
    }

    public string getUpgradeInfo()
    {
        return "Charge Capacity: " + chargeCapacity + " + 25 \n " +
        "Rechage Rate: " + rechageRate + " + .5 \n" +
        "Recharge Amount: " + rechargeAmt + "+ 1"; 
    }

    public void setLevel(int in_level)
    {
        chargeCapacity = 25f * in_level;
        rechageRate = .5f * in_level;
        rechargeAmt = 1 * in_level;
        addonLevel = in_level;
    }
}
