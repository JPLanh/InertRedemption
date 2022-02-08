using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMagazineAddon : MonoBehaviour, IAddon
{
    private string addonName = "Basic Magazine";
    private int addonLevel = 1;
    private Dictionary<string, int> requirements;

    public int maxAmmo;
    public string ammoType;
    public string damageType;
    public int ammo;
    public float reloadSpeed;

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
        maxAmmo += 50;
        reloadSpeed -= 1f;
        addonLevel += getVal;
    }

    public string getInfo()
    {
        return "Increase the amount of ammo your gun can carry";
    }

    public string getUpgradeInfo()
    {

        return "Max Ammo: " + maxAmmo + " + 50 \n " +
        "Reload Speed: " + reloadSpeed + " - 1 \n";
    }
}