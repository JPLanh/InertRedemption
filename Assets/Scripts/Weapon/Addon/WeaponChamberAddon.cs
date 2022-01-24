using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChamberAddon : MonoBehaviour, IAddon
{
    private string addonName = "Basic Chamber";
    private int addonLevel = 1;
    private Dictionary<string, int> requirements;

    public float damage;
    public float fireRate;
    public int ammoConsumption;
    public float nextTimeToFire;

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
        damage += 1;
        fireRate += 1f;
        addonLevel += getVal;
    }

    public string getInfo()
    {
        return "Enhance your gun to fire projectile much stronger.";
    }

    public string getUpgradeInfo()
    {

        return "Damage: " + damage + " + 1 \n " +
        "Fire Rate: " + fireRate + " + 1 fire per secod \n";
    }
}
