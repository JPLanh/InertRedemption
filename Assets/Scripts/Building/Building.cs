using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, Damagable, Displayable, IBuilding, IPublisher, ISubscribers
{
    public Node currentNode;
    public string harvestTarget = "Electricity";
    public int amount = 2;
    public Vector3 targetPosition;
    public buidingPlacer placer;

    public List<IBuilding> poweredBuildings;
    public float speed = 15f;
    public bool active;
    public float durability;
    [SerializeField]
    private GameObject emissionField;
    [SerializeField]
    private Collider mainCollider;
    [SerializeField]
    private Collider blockCollider;
    [SerializeField]
    private GameObject energyGridRange;
    private int currentAmount = 0;
    [SerializeField]
    private int maxCurrent;

    public List<ISubscribers> resourceSubscribers;
    public List<IPublisher> resourceProviders;

    private int phase = 0;
    //    private float nextHarvestTime = 0;
    public Vector3 positionToMoveTo;
    //    private bool moving = false;

    // Start is called before the first frame update
    void Start()
    {
        resourceSubscribers = new List<ISubscribers>();
        resourceProviders = new List<IPublisher>();
        placer.building = this;
        poweredBuildings = new List<IBuilding>();
    }

    public GameObject isDamage(bool network, float getValue, GameObject attacker)
    {
        durability += getValue;
        if (durability < 0)
        {

            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 50);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent<IBuilding>(out IBuilding getBuilding))
                {
                    getBuilding.toggleBuildable(false, this);
                }
            }
            Destroy(this.gameObject);
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ISubscribers getSubscriber in resourceSubscribers)
        {
            if (getSubscriber.getResourceRequest() != null)
            {
                foreach (IPublisher getProvider in resourceProviders)
                {

                    if (getProvider.requestResourceFrom(getSubscriber.getResourceRequest()))
                    {
                        getSubscriber.fufillRequest();
                    }
                }
            }
        }
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
        phase++;
        //       moving = false;
    }

    public string display()
    {
        string disp = "Energy Field \n";
        disp = "Durability: " + durability + " / 100\n";
        if (placer.startTimer != 0)
        {
            if (placer != null)
            {
                disp += "Constructing\n";
                disp += "Progress: " + (int)(((Time.time - placer.startTimer) / placer.completion) * 100) + "%\n";
            }
            else
            {
                disp += "Energy Usage: " + currentAmount + " / " + maxCurrent + "\n";
            }
        }
        else
        {
            disp += "Placing\n";

        }

        return disp;
    }


    public void buildingComplete()
    {
        Destroy(placer);
        energyGridRange.SetActive(true);
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 50);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<IBuilding>(out IBuilding getBuilding))
            {
                currentAmount += getBuilding.toggleBuildable(true, this);
            }
        }
    }

    public void buildingInProgress()
    {
        durability = (int)(((Time.time - placer.startTimer) / placer.completion) * 100);
    }

    public void placeBuilding(PlayerController player)
    {
        setCollisions(true);
    }

    public void startBuilding()
    {
        if (emissionField.activeInHierarchy)
        {
            emissionField.SetActive(false);
        }
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
        return 0;
    }


    public string getName()
    {
        throw new System.NotImplementedException();
    }

    public Building getEmitter()
    {
        return null;
    }


    public bool requestResourceFrom(string getResource)
    {
        throw new System.NotImplementedException();
    }

    public string getResourceRequest()
    {
        throw new System.NotImplementedException();
    }

    public void fufillRequest()
    {
        throw new System.NotImplementedException();
    }

    public void addSubscriber(ISubscribers getSubscriber)
    {
        resourceSubscribers.Add(getSubscriber);
    }

    public void addPublisher(IPublisher getPublisher)
    {
        resourceProviders.Add(getPublisher);
    }

    public int getBuildingCurrent()
    {
        return 0;
    }

    public bool addCurrent(int getAmount)
    {
        currentAmount += getAmount;
        if (currentAmount <= maxCurrent) active = true;
        else active = false;
        return active;
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