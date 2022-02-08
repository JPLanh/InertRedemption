using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour, Damagable, Displayable, IBuilding
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
    private Building emitter;
    [SerializeField]
    private int buildingCurrent;

    // Start is called before the first frame update
    void Start()
    {
        placer.building = this;
    }

    public GameObject isDamage(bool network, float getValue, GameObject attacker)
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
        string disp = "Barricade \n";
        disp = "Durability: " + durability + " / 100\n";
        if (placer.buildable)
            disp += "Powered on \n";
        else
            disp += "Not powered\n";
        if (placer.startTimer != 0)
        {
            if (placer != null)
            {
                disp += "Constructing\n";
                disp += "Progress: " + (int)(((Time.time - placer.startTimer) / placer.completion) * 100) + "%\n";
            }
            else
            {
                disp += "Completed";
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

    public Building getEmitter()
    {
        return emitter;
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
        tmpRequirement.Add(new InventoryMapping("Stone", 1));
        tmpRequirement.Add(new InventoryMapping("Log", 1));
        return tmpRequirement;
    }
}
