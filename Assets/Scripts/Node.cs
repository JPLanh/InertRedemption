using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour, Interactable, Displayable
{
    public Transform hardDrive;
    public HardDrive hardDriveScript;
    Vector3 position;
    public int energy = 0;
    float nextHarvest;
    public float conversionTimer = 0;
    public float conversion = 0;
    public int resourceNum;
    public float nextReplenishment = 5f;

    [SerializeField]
    private Collider detection;

    public TransferCenter transferCenter;
    public List<GameObject> buildings;


    // Start is called before the first frame update
    void Start()
    {
        buildings = new List<GameObject>();
    }

    // Update is called once per frame
    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextReplenishment)
        {

            nextReplenishment = Time.time + 25;
            if (transferCenter.resources.TryGetValue("Stone", out int scrapAmount))
            {
                if (transferCenter.resources["Stone"] > 0)
                {
                    createResource("Ammo", 15);
                    transferCenter.resources["Stone"] -= 1;
                }
            }
        }
    }

    private void createResource(string getResource)
    {
        if (!transferCenter.resources.TryGetValue(getResource, out int scrapAmount))
        {
            transferCenter.resources[getResource] = 0;
        }
        transferCenter.resources[getResource] += 1;
    }

    private void createResource(string getResource, int getAmount)
    {
        if (!transferCenter.resources.TryGetValue(getResource, out int scrapAmount))
        {
            transferCenter.resources[getResource] = 0;
        }
        transferCenter.resources[getResource] += getAmount;
    }

    public void Interact(PlayerController getPlayer)
    {
            if (getPlayer.GetComponent<LivingBeing>().mainHand.GetChild(0) != null)
            {

                UsableItemInterface mainHand = getPlayer.GetComponent<LivingBeing>().mainHand.GetChild(0).GetComponent<UsableItemInterface>();
                if (mainHand != null)
                {
                    if (mainHand.getEnergy() < 100 && energy > 0)
                    {
                        energy -= mainHand.rechargeDurability(energy);
                    }
                }
        }
        //PlayerController pc = getPlayer.GetComponent<PlayerController>();
        //if (pc != null)
        //{

        //}
        //if (!transform.Find("Hard Drive"))
        //{
        //    if (getPlayer.Find("Hard Drive"))
        //    {
        //        getPlayer.Find("Hard Drive").SetParent(transform);
        //        transform.Find("Hard Drive").position = transform.position;
        //    }
        //}
        //else
        //{
        //    if (!getPlayer.Find("Hard Drive"))
        //    {
        //        transform.Find("Hard Drive").SetParent(getPlayer);
        //        getPlayer.Find("Hard Drive").position = getPlayer.position;
        //    }
        //}
        //hardDrive = transform.Find("Hard Drive");
        //hardDriveScript = (hardDrive == null ? null : hardDrive.GetComponent<HardDrive>());
    }

    public void getNodeData()
    {
        Dictionary<string, string> payload = StringUtils.getPositionAndRotation(transform.position, transform.rotation);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 2f);
    }

    public void harvest(string getResource, int getAmount)
    {
            if (!transferCenter.resources.TryGetValue(getResource, out int ComponentsAmount))
            {
            transferCenter.resources[getResource] = 1;
            }
            else
            {
            transferCenter.resources[getResource] += getAmount;
            }
    }

    public void convert(int amount, string getTeam)
    {

        if (Time.time >= conversionTimer)
        {
            if (amount > 0 && conversion < 100)
            {
                conversion += amount;
                conversionTimer = Time.time + 1;
            }
            if (amount < 0 && conversion > 0)
            {
                conversion -= amount;
                conversionTimer = Time.time + 1;
            }

//            if (conversion > 99 && owner == null)
//            {
//                conversion = 100;
//                position = transform.position + new Vector3(0f, 2f, 0f);
////                owner = GameObject.Find(getTeam + " Base").GetComponent<Base>();
//                transferCenter.team = getTeam;
//                GetComponent<Rigidbody>().isKinematic = true;
//                conversionTimer = Time.time + 1;
//            }
        }
        //if (owner != null) {
        //    if (transform.position != position + new Vector3(0f, 2f, 0f))
        //        transform.position = Vector3.MoveTowards(transform.position, position + new Vector3(0f, 2f, 0f), 2 * Time.deltaTime);
        //    else
        //    {
        //        GetComponent<Light>().enabled = true;
        //        GetComponent<Light>().color = owner.getTeamColor();
        //    }
        //    if (conversion <= 0)
        //    {
        //        conversion = 0;
        //        GetComponent<Rigidbody>().isKinematic = false;
        //        owner = null;

        //    }
        //}
    }

    public string display()
    {

        string disp = "Energy Capsule \n";
        disp += "Energy: " + energy;

        //if (node.conversion != 0 && node.conversion != 100 && nodeDetector.group != null)
        //{
        //    disp += "Converting: \n" +
        //            " -Team: " + nodeDetector.group + "\n" +
        //            " -Process: " + StringUtils.convertFloatToString(node.conversion) + "\n";
        //}

        return disp;
    }
}
