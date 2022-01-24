using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardDrive : MonoBehaviour
{
    public GameObject owner;
    public int harvested; //usedSpace
    public int maxHarvest; //capacity
    private float percentage;

    public int transferRate; //transferRate
    public float nextTransferTime; //nextTransferTime

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if ((float)harvested / maxHarvest != percentage)
        //{
        //    percentage = (float)harvested / maxHarvest;
        //    transform.GetChild(0).GetComponent<Light>().intensity = (percentage) * 10;
        //}
    }

    public int gather(int resourceAmount)
    {
        return 0;
        //if (Time.time >= nextHarvest)
        //{
        //    int harvest = resourceAmount > harvestAmount ? harvestAmount : resourceAmount;
        //    if (harvested >= maxHarvest)
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        int harvestForesight = harvested + harvest;
        //        if (harvestForesight >= maxHarvest)
        //        {
        //            transform.GetChild(0).GetComponent<Light>().color = new Color(255, 0, 255, 255);
        //            harvested = maxHarvest;
        //            resetHarvestTimer();
        //            return harvest - (harvestForesight % maxHarvest);
        //        }
        //        harvested += harvest;
        //        resetHarvestTimer();
        //        return harvest;

        //    }
        //}
        //return 0;
    }

    public int transfer(int rate, int resourceAmount)
    {
        return 0;
        //if (Time.time >= nextHarvest)
        //{

        //    int harvest = resourceAmount > harvested ? harvested : resourceAmount;
        //    if (harvested <= 0)
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        int harvestForesight = harvested - harvest; // 2 - 7
        //        if (harvestForesight <= 0)
        //        {
        //            harvested = 0;
        //            nextHarvest = Time.time + 1f / rate;
        //            print("Harvest foresight: " + harvest + harvestForesight);
        //            return harvest + harvestForesight; //7 - ? = 2
        //        }
        //        harvested -= harvest;
        //        nextHarvest = Time.time + 1f / rate;
        //        print("Harvest: " + harvest);
        //        return harvest;

        //    }
        //}
        //return 0;
    }
}
