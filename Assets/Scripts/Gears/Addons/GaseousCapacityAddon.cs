using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaseousCapacityAddon : MonoBehaviour, IAddon
{
    //Update this
    private string addonName = "Basic Gaseous Capacity";
    private int addonLevel = 1;
    private float resilienceRate = 0f;
    private Dictionary<string, int> requirements;

    public int getLevel()
    {
        return addonLevel;
    }

    public string getName()
    {
        return addonName;
    }


    //Update this (Level requirement for each) 5 cases
    public Dictionary<string, int> getRequirements()
    {
        requirements = new Dictionary<string, int>();
        switch (addonLevel)
        {
            case 1:
                requirements.Add("Stone", 2);
                break;
            case 2:
                requirements.Add("Stone", 8);
                break;
            case 3:
                requirements.Add("Log", 10);
                requirements.Add("Stone", 10);
                break;
            case 4:
                requirements.Add("Log", 25);
                requirements.Add("Stone", 15);
                break;
        }
        return requirements;
    }

    public void updateLevel(int getVal)
    {
        resilienceRate += .05f * getVal;
        addonLevel += getVal;
    }

    public void setLevel(int in_val)
    {
        resilienceRate = (.05f * in_val);
        addonLevel = in_val;
    }

    public string getInfo()
    {
        return "Increase your resiliency to the virus' infection, slowing the virus' infection rate.";
    }

    public string getUpgradeInfo()
    {

        return $"Resiliency: {resilienceRate * 100}%";
    }
}