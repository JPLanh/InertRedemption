using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Make it harfer for a virus to infect the survivors.
 */

public class BodyResilientAddon : MonoBehaviour, IAddon
{
    private string addonName = "Basic Body Resiliency";
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

        return $"Resiliency: {resilienceRate*100}%";
    }
}
