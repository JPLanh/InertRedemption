using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, IInventory, IEquipment
{
    [SerializeField]
    private string partName;
    //private int maxSize;
    //private int amount;
    public int amountOnHold;
    //public Dictionary<string, int> inventory;
    public float intervalTime;

    public StorageCapacityAddon capacityAddon;

    public string getName()
    {
        return partName;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Dictionary<string, int> getInventory()
    {
        return capacityAddon.getInventory();
    }

    public Dictionary<string, int> getAmmo()
    {
        return capacityAddon.getAmmo();
    }

    public bool recieveAmmo(string getName, int getAmt)
    {
        if (capacityAddon.ammoAmount + getAmt <= capacityAddon.maxAmmoSize)
        {
            if (!capacityAddon.getAmmo().TryGetValue(getName, out int ComponentsAmount))
            {
                capacityAddon.getAmmo()[getName] = 0;
            }
            capacityAddon.getAmmo()[getName] += getAmt;

            capacityAddon.ammoAmount += getAmt;
            return true;
        }
        else return false;
    }

    public bool recieveItem(string getName, int getAmt)
    {
        if (capacityAddon.amount + getAmt <= capacityAddon.maxSize)
        {
            if (!capacityAddon.inventory.TryGetValue(getName, out int ComponentsAmount))
            {
                capacityAddon.inventory[getName] = 0;
            }
            capacityAddon.inventory[getName] += getAmt;

            capacityAddon.amount += getAmt;
            return true;
        }
        else return false;
    }

    public void getHoldAmt(string getName, int getAmt)
    {
        amountOnHold += getAmt;
    }

    public void convertAllHold()
    {
        capacityAddon.amount -= amountOnHold;
        amountOnHold = 0;
    }

    public List<IAddon> getAllAddons()
    {
        List<IAddon> listOfAddons = new List<IAddon>();
        listOfAddons.Add(capacityAddon);
        return listOfAddons;
    }

    public void intervalAmmoIncrease(string getName, int getAmt)
    {
        if (capacityAddon.ammoAmount + getAmt <= capacityAddon.maxAmmoSize)
        {
            if (Time.time > intervalTime)
            {
                intervalTime = Time.time + 1f;
                if (!capacityAddon.getAmmo().TryGetValue(getName, out int ComponentsAmount))
                {
                    capacityAddon.getAmmo()[getName] = 0;
                }
                capacityAddon.getAmmo()[getName] += getAmt;

                capacityAddon.ammoAmount += getAmt;
            }
        }
    }

    public void modifyAmount(int getAmt)
    {
        capacityAddon.amount += getAmt;
    }
}

