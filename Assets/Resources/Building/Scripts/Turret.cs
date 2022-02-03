using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour, Damagable, Displayable, IBuilding, Interactable
{
    [SerializeField]
    private ParticleSystem muzzleFlash;

    public float attackRate;
    public float projectileRange;
    public float projectileSpeed;
    public float projectileDamage;
    public float nextTimeToAttack;
    public float attackDistance;
    public float visionDistance;
    public float visionDistanceReal;

    [SerializeField]
    private Transform turretHead;

    [SerializeField]
    private float health;
    [SerializeField]
    private float maxHealth;


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
        visionDistanceReal = visionDistance;
        visionDistance *= visionDistance;
        placer.building = this;
    }

    // Update is called once per frame
    void Update()
    {
        //if (health < maxHealth)
        //{
        //    if (team.transferCenter.resources.TryGetValue("Scrap", out int scrapAmount))
        //    {
        //        if (scrapAmount > 0)
        //        {

        //            health += 2;
        //            if (health > maxHealth) health = maxHealth;
        //            team.transferCenter.resources["Scrap"] -= 1;
        //        }
        //    }
        //}
        if (placer == null && emitter.active)
        {

        Transform enemyDetected;
        GetClosestEnemy(out enemyDetected);
        if (enemyDetected != null)
        {
            turretHead.transform.LookAt(enemyDetected);

            Debug.DrawRay(muzzleFlash.transform.position, muzzleFlash.transform.forward * visionDistanceReal, Color.blue);
            if (Physics.Raycast(muzzleFlash.transform.position, muzzleFlash.transform.forward, out var hit, visionDistanceReal))
            {

                if (Time.time >= nextTimeToAttack)
                {
                    nextTimeToAttack = Time.time + 1f / attackRate;

                    if (hit.transform.gameObject.tag == "Viruses")
                    {
                        GameObject lazurBeem = Instantiate(Resources.Load<GameObject>("Laser Beam"), muzzleFlash.transform.position, muzzleFlash.transform.rotation);
                        lazurBeem.GetComponent<Projectile>().setProjectile(projectileSpeed, projectileRange, projectileDamage);
                    }
                }
            }
        }
        }
    }

    private void GetClosestEnemy(out Transform detection)
    {
        GameObject[] gos = null;
        gos = GameObject.FindGameObjectsWithTag("Viruses");
        //if (turretHead.transform.gameObject.CompareTag("Blue")) gos = GameObject.FindGameObjectsWithTag("Red");
        //if (turretHead.transform.gameObject.CompareTag("Red")) gos = GameObject.FindGameObjectsWithTag("Blue");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = turretHead.transform.position;
        if (gos != null)
        {
            foreach (GameObject go in gos)
            {
                Vector3 diff = go.transform.position - turretHead.position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance && curDistance < visionDistance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }

        }
        detection = closest != null ? closest.transform : null;
    }


    public GameObject isDamage(bool network, float getValue, GameObject attacker)
    {
        durability += getValue;

        if (durability < 0)
        {
            //           Instantiate(loot, this.transform.position + new Vector3(0, 2f, 0), Quaternion.identity);

            emitter.addCurrent(-buildingCurrent);
            Destroy(this.gameObject);
        }

        return null;
    }

    //public string display()
    //{

    //    string disp = gameObject.name + " \n\n";

    //    return disp;
    //}

    public string display()
    {
        string disp = "Turret \n";
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
            disp += "Attack Rate: " + attackRate + "\n";
            disp += "Laser Range: " + projectileRange + "\n";
            disp += "Attack Speed: " + projectileSpeed + "\n";
            disp += "Attack Damage: " + projectileDamage + "\n";
            disp += "Attack Distance: " + attackDistance + "\n";
            disp += "Vision Distance: " + visionDistance + "\n";
        }
        return disp;
    }

    public void buildingComplete()
    {
        Destroy(placer);
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

    public Building getEmitter()
    {
        return emitter;
    }

    public string getResourceRequest()
    {
        throw new System.NotImplementedException();
    }

    public void fufillRequest()
    {
        throw new System.NotImplementedException();
    }

    public void Interact(PlayerController player)
    {
        if (placer != null) placer.Interact(player);
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
        tmpRequirement.Add(new InventoryMapping("Stone", 3));
        return tmpRequirement;
    }
}
