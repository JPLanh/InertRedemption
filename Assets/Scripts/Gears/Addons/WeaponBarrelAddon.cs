using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBarrelAddon : MonoBehaviour, IAddon
{
    private string addonName = "Basic Barrel";
    private int addonLevel = 1;
    public float projectileSpeed;
    public float projectileRange;
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
        projectileSpeed += 5f;
        projectileRange += 15f;
        addonLevel += getVal;
    }

    public string getInfo()
    {
        return "Increase the efficiency of your bullet.";
    }

    public string getUpgradeInfo()
    {

        return "Projectile speed: " + projectileSpeed + " + 5 \n " +
        "Projectile Range: " + projectileRange + " + 15 \n";
    }

    public void setLevel(int in_level)
    {
        projectileSpeed = 5f * in_level;
        projectileRange = 15f * in_level;
        addonLevel = in_level;
    }
}
