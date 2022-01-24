using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenCapacityAddon : MonoBehaviour, IAddon
{

    private string addonName = "Basic Oxygen";
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

        rechargeRate += 1f;
        rechargeAmount += 1;
        maxCharge += 25;

        addonLevel += getVal;
    }

    public string getInfo()
    {
        return "Increase the oxygen capacity that your tank may carry.";
    }

    public string getUpgradeInfo()
    {

        return "Max Capacity: " + maxCharge + " + 25 \n " +
        "Recharge rate: " + rechargeRate + " + 1 charge per second \n" +
        "Recharge amount: " + rechargeAmount + " + 1";
    }
}
