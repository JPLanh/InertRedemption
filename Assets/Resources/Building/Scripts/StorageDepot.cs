using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageDepot : MonoBehaviour, IDamagable, Displayable, IBuilding, Interactable, IPublisher
{
    public Node currentNode;
    public string harvestTarget = "Electricity";
    public int amount = 2;
    public Dictionary<string, int> inventory;
    public Vector3 targetPosition;
    public buidingPlacer placer;

    public float speed = 15f;
    private bool deployed;
    public float durability;
    [SerializeField]
    private Collider mainCollider;
    [SerializeField]
    private Collider blockCollider;
    private Building emitter;
    public float convertTime;
    public float nextAction = 0f;
    [SerializeField]
    private int buildingCurrent;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new Dictionary<string, int>();
        placer.building = this;
    }

    public bool isDamage(bool network, float getValue, GameObject attacker)
    {
        return false;
        //durability += getValue;
        //if (durability < 0)
        //{
        //    emitter.addCurrent(-buildingCurrent);
        //    Destroy(this.gameObject);
        //}

        //return null;
    }

    // Update is called once per frame
    void Update()
    {
        //if (phase == 0)
        //{
        //    positionToMoveTo = transform.position + new Vector3(0f, 250f, 0f);
        //    if (!moving) StartCoroutine(LerpPosition(positionToMoveTo, 15));
        //} else if (phase == 1)
        //{
        //    positionToMoveTo = new Vector3(targetPosition.x, 250f, targetPosition.z);
        //    if (!moving) StartCoroutine(LerpPosition(positionToMoveTo, 10));
        //} else if (phase == 2)
        //{
        //    positionToMoveTo = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
        //    if (!moving) StartCoroutine(LerpPosition(positionToMoveTo, 10));
        //} else if (phase == 3)
        //{
        //    if (currentNode != null)
        //    {
        //        if (Time.time >= nextHarvestTime)
        //        {
        //        currentNode.harvest(harvestTarget, amount);
        //            nextHarvestTime = Time.time + 1f;
        //        }
        //    }

        //}
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
        string disp = "Storage Depot \n";
        disp += "Durability: " + durability + " / 100\n";
        if (emitter != null && emitter.active)
            disp += "Powered on \n";
        else
            disp += "Not powered\n";
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
                disp += "Converting Ammo\n";
                disp += "Progress: " + (int)(((Time.time - nextAction) / convertTime) * 100) + "%\n";
            }
            disp += StringUtils.printDictionary(inventory);
        }

        return disp;
    }

    public void buildingComplete()
    {
        Destroy(placer);
        emitter.addPublisher(this);
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
        if (player.livingBeing.inventory.ContainsKey("Stone"))
        {
            if (player.livingBeing.inventory["Stone"] > 0)
            {
                player.livingBeing.inventory["Stone"] -= 1;
                if (!inventory.ContainsKey("Stone"))
                {
                    inventory.Add("Stone", 0);
                }
                inventory["Stone"] += 1;
            }
        }
    }

    public Building getEmitter()
    {
        return emitter;
    }

    public bool requestResourceFrom(string getResource)
    {
        if (inventory.ContainsKey(getResource)) {
         
        if (inventory[getResource] > 0)
            {
            print("Resources given");
            inventory[getResource] -= 1;
            return true;
            }
        }
            return false;
    }

    public string getResourceRequest()
    {
        throw new System.NotImplementedException();
    }

    public void fufillRequest()
    {
        throw new System.NotImplementedException();
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
