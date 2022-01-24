using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageCapacityAddon : MonoBehaviour, IAddon
{
    private string addonName = "Basic Capacity";
    private int addonLevel = 1;
    private Dictionary<string, int> requirements;

    public int maxSize;
    public int maxAmmoSize;
    public int amount;
    public int ammoAmount;
    public Dictionary<string, int> inventory;
    public Dictionary<string, int> ammo;

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
                maxSize = 10;
                break;
            case 2:
                requirements.Add("Log", 5);
                requirements.Add("Stone", 5);
                maxSize = 20;
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
        inventory = new Dictionary<string, int>();
        ammo = new Dictionary<string, int>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateLevel(int getVal)
    {
        maxSize += 5;
        maxAmmoSize += 20;
        addonLevel += getVal;
    }

    public Dictionary<string, int> getInventory()
    {
        return inventory;
    }
    public Dictionary<string, int> getAmmo()
    {
        return ammo;
    }


    public string getInfo()
    {
        return "Storage Addon where you will keep your items";
    }

    public string getUpgradeInfo()
    {

        return "Max Capacity: " + maxSize + " + 5 \n " +
        "Max Ammo Capacity: " + maxAmmoSize + " + 20 \n";
    }
}