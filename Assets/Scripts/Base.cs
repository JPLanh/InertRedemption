using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public string team;
    [SerializeField]
    private Color teamColor;
    public bool isHost;
    public float nextReplenishment = 0f;

    [SerializeField]
    private GameObject Buildings;
    [SerializeField]
    private DataServer database;

    public TransferCenter transferCenter;
    public GameObject playerLists;



    // Start is called before the first frame update
    void Start()
    {
        createResource("Log", 50);
        createResource("Wooden Plank", 50);
        createResource("Fiber", 50);
        createResource("Stone", 50);
        createResource("Oil", 200);
        createResource("Water", 200);
        database.gameObject.tag = team;
    }

    public GameObject getBuildings()
    {
        return Buildings;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextReplenishment)
        {
            nextReplenishment = Time.time + 25;
            if (transferCenter.resources["Stone"] > 0)
            {
                createResource("Ammo", 15);
                transferCenter.resources["Stone"] -= 1; 
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

    public string GetTeam()
    {
        return team;
    }

    public Color getTeamColor()
    {
        return teamColor;
    }
}
