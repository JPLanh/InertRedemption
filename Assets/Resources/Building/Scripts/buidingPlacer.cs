using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buidingPlacer : MonoBehaviour, Interactable
{
    public bool buildable;
    public bool inBuildingZone;

    public PlayerController accessor;
    public float startTimer = 0f;
    public float completion = 50f;

    [SerializeField]
    bool requireEmitter;
    public IBuilding building;
    public List<InventoryMapping> resourceHold;


    void Start()
    {
    }

    public void Interact(PlayerController player)
    {
        if (startTimer == 0f)
        {
            startTimer = Time.time;
            building.startBuilding();
        }

    }

    public void pickupBuildPlacer(PlayerController player)
    {
        building.relocateBuilding(player);

    }
    
public void placeBuilding(PlayerController player)
    {
        //        if (buildable || !requireEmitter)
        building.placeBuilding(player);

    }

    void Update()
    {
        if (requireEmitter && !buildable)
            startTimer = 0f;
        else
        {
            if (startTimer != 0)
                building.buildingInProgress();
        }
        if (Time.time > (startTimer + completion) && startTimer != 0)
        {
            building.buildingComplete();
            if (requireEmitter)
            {
            building.getEmitter().poweredBuildings.Add(this.GetComponent<IBuilding>());
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (requireEmitter)
        {
            if (col.TryGetComponent<EnergyGridRange>(out EnergyGridRange egr))
            {
                building.toggleBuildable(true, egr.getDataEmitter());
            }
        }
    }


    void OnTriggerExit(Collider col)
    {
        if (requireEmitter)
        {
            if (col.TryGetComponent<EnergyGridRange>(out EnergyGridRange egr))
            {
                building.toggleBuildable(false, null);
            }
        }
    }
    /*
    void OnTriggerEnter(Collider col)
    {
        if (accessor.livingBeing.currentNode != null)
        {
            if (accessor.livingBeing.currentNode.owner != null)
            {
                if (accessor.livingBeing.currentNode.owner.GetTeam() == accessor.team.GetTeam())
                {
                    if (col.tag != "Ground")
                    {
                        if (col.name == "Detection Radius")
                        {
                            inBuildingZone = true;
                            transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(43f / 255f, 255f / 255f, 43f / 255f, 43f / 255f);
                        }
                        else
                        {
                            buildable = false;
                            transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(255f / 255f, 43f / 255f, 43f / 255f, 43f / 255f);
                        }
                    }
                }
                else
                {
                    buildable = false;
                    inBuildingZone = false;
                    transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(255f / 255f, 43f / 255f, 43f / 255f, 43f / 255f);
                }
            }
            else
            {
                buildable = false;
                inBuildingZone = false;
                transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(255f / 255f, 43f / 255f, 43f / 255f, 43f / 255f);
            }
        }
        else
        {
            buildable = false;
            inBuildingZone = false;
            transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(255f / 255f, 43f / 255f, 43f / 255f, 43f / 255f);

        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag != "Ground")
        {
            if (col.name == "Detection Radius")
            {
                inBuildingZone = false;
                buildable = false;
                transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(255f / 255f, 43f / 255f, 43f / 255f, 43f / 255f);
            }
            else
            {
                if (inBuildingZone)
                {
                    buildable = true;
                    transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(43f / 255f, 255f / 255f, 43f / 255f, 43f / 255f);
                }
                else
                {
                    buildable = false;
                    transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(255f / 255f, 43f / 255f, 43f / 255f, 43f / 255f);

                }
            }
        }
    }
    */

}

[System.Serializable]
public struct InventoryMapping
{
    public string key;
    public int value;

    public InventoryMapping(string getKey, int getValue)
    {
        key = getKey;
        value = getValue;
    }
}