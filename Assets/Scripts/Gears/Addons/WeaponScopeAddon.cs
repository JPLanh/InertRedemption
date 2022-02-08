using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScopeAddon : MonoBehaviour, IAddon
{
    private string addonName = "Basic Scope";
    private int addonLevel = 1;
    public float range = 250f;
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
                requirements.Add("Stone", 5);
                requirements.Add("Crystal", 5);
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
        range = 25f;
        addonLevel += getVal;
    }
    public string getInfo()
    {
        return "Enhance your vision while you are looking down the scope";
    }

    public string getUpgradeInfo()
    {

        return "Range: " + range + " + 25 \n ";
    }
}
