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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateLevel(int getVal)
    {
        damage += 5;
        addonLevel += getVal;
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