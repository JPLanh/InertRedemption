using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaponBladeAddon : MonoBehaviour, IAddon
{
    private string addonName = "Basic Blade";
    private int addonLevel = 1;
    public float damage;
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
        damage -= 5 * getVal;
        addonLevel += getVal;
    }

    public void setLevel(int in_val)
    {
        damage = -20 - (5 * in_val);
        addonLevel = in_val;
    }

    public string getInfo()
    {
        return "Enhance your blade to deal more damage.";
    }

    public string getUpgradeInfo()
    {

        return "Damage: " + damage + " + 5";
    }
}