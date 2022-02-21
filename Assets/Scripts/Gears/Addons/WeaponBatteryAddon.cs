using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBatteryAddon : MonoBehaviour, IAddon
{
    private string addonName = "Basic Weapon Battery";
    private int addonLevel = 1;
    private Dictionary<string, int> requirements;

    //How many tick per second;
    public float rechargeRate;
    public float rechargeTimer;
    //Amount of charge per tick
    public int rechargeAmount;
    public float maxCharge;
    public float charge;

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

    public int recharge(int getVal)
    {
        if (Time.time > rechargeTimer)
        {
            int rechargingAmt = 0;
            if (charge < maxCharge)
            {
                rechargingAmt = (getVal > rechargeAmount ? rechargeAmount : getVal);

                rechargeTimer = Time.time + (1f / rechargeRate);
                charge += rechargingAmt;
            }
            return rechargingAmt;

        }
        return 0;
    }
    public void updateLevel(int getVal)
    {

        rechargeRate += 1f * getVal;
        rechargeAmount += 1 * getVal;
        maxCharge += 25 * getVal;

    addonLevel += getVal;
    }

    public string getInfo()
    {
        return "Increase the efficiency of your weapon's battery.";
    }

    public string getUpgradeInfo()
    {
        return "Max Charge: " + maxCharge + " + 25 \n " +
        "Recharge rate: " + rechargeRate + " + 1 charge per second \n" +
        "Recharge amount: " + rechargeAmount + " + 1";
    }

    public void setLevel(int in_level)
    {
        rechargeRate = 1f * in_level;
        rechargeAmount = 1 * in_level;
        maxCharge = 25 * in_level;
        addonLevel = in_level;
    }
}