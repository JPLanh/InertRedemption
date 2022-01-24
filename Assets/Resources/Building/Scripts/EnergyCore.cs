using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCore : MonoBehaviour, Damagable, Displayable, IBuilding, Interactable
{
    public Node currentNode;
    public string harvestTarget = "Electricity";
    public int amount = 2;
    public Vector3 targetPosition;
    public buidingPlacer placer;

    public float speed = 15f;
    private bool deployed;
    public float durability;
    [SerializeField]
    private Collider mainCollider;
    [SerializeField]
    private Collider blockCollider;
    [SerializeField]
    private Building emitter;
    private string requesting;
    private Dictionary<string, int> inventory;
    public float convertTime;
    public float nextAction = 0f;
    [SerializeField]
    private int buildingCurrent;

    // Start is called before the first frame update
    void Start()
    {
        requesting = null;
        inventory = new Dictionary<string, int>();
        if (placer)
            placer.building = this;
    }

    public GameObject damage(bool network, float getValue, GameObject attacker)
    {
        durability += getValue;
        if (durability < 0)
        {
            emitter.addCurrent(-buildingCurrent);
            Destroy(this.gameObject);
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (emitter != null)
        {

            if (emitter.active)
            {
                if (Time.time > nextAction + convertTime)
                {
                    if (!inventory.ContainsKey("Energy"))
                    {
                        inventory.Add("Energy", 0);
                    }
                    if (inventory["Energy"] < 100)
                    {
                        inventory["Energy"] += 2;
                    } else
                    {
                        inventory["Energy"] = 100;
                    }
                        nextAction = Time.time;
                    //                    requesting = "Stone";
                }
            }
        }
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        //        moving = true;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        //        phase++;
        //       moving = false;
    }

    public string display()
    {
        string disp = "Energy Core \n";
        disp += "Durability: " + durability + " / 100\n";

        if (emitter != null)
        {
            disp += "Powered on \n";
        }
        else
        {
            disp += "Not powered\n";
        }

        if (placer != null)
        {
            if (placer.startTimer != 0)
            {
                disp += "Constructing\n";
                disp += "Progress: " + (int)(((Time.time - placer.startTimer) / placer.completion) * 100) + "%\n";
            }
            else
            {
                disp += "Placing\n";
            }
        }
        else
        {
            if (nextAction != 0)
            {
                disp += "Generating Energy\n";
                disp += "Progress: " + (int)(((Time.time - nextAction) / convertTime) * 100) + "%\n";
            }
            disp += StringUtils.printDictionary(inventory);
        }

        return disp;
    }

    public void buildingComplete()
    {
        Destroy(placer);
        placer = null;
        emitter.addCurrent(buildingCurrent);

    }

    public void buildingInProgress()
    {
        durability = (int)(((Time.time - placer.startTimer) / placer.completion) * 100);
    }

    public void placeBuilding(PlayerController player)
    {
        setCollisions(true);
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 50);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<Building>(out Building getBuilding))
            {
                if (getBuilding.placer == null)
                    toggleBuildable(true, getBuilding);

            }
        }
    }

    public void startBuilding()
    {
    }

    public void relocateBuilding(PlayerController player)
    {
        setCollisions(false);
        player.buildPlacement = this.gameObject;
    }

    public void setCollisions(bool getBool)
    {
        mainCollider.enabled = getBool;
        blockCollider.enabled = getBool;
    }

    public int toggleBuildable(bool getBool, Building getEmitter)
    {
        if (placer != null)
            placer.buildable = getBool;
        else return buildingCurrent;
        emitter = getEmitter;
        return 0;
    }

    public void Interact(PlayerController player)
    {
        if (emitter != null)
        {
            if (player.GetComponent<LivingBeing>().mainHand.GetChild(0) != null)
            {

                UsableItemInterface mainHand = player.GetComponent<LivingBeing>().mainHand.GetChild(0).GetComponent<UsableItemInterface>();
                if (mainHand != null)
                {
                    if (mainHand.getEnergy() < 100 && inventory["Energy"] > 0)
                    {
                        inventory["Energy"] -= mainHand.rechargeDurability(inventory["Energy"]);                    }
                }
            }
        }
    }

    public Building getEmitter()
    {
        return emitter;
    }

    public string getResourceRequest()
    {
        return requesting;
    }

    public void fufillRequest()
    {
        nextAction = Time.time;
        convertTime = 10f;
        requesting = null;
    }

    public int getBuildingCurrent()
    {
        return buildingCurrent;
    }
    public List<InventoryMapping> getRequirement()
    {
        return BuildingRequirement();
    }
    public static List<InventoryMapping> BuildingRequirement()
    {
        List<InventoryMapping> tmpRequirement = new List<InventoryMapping>();
        tmpRequirement.Add(new InventoryMapping("Log", 3));
        return tmpRequirement;
    }
}
