using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{

    public float health = 100;

    private string state = "patrol";
    public GameObject[] wayPointsOne;
    public GameObject[] wayPointsTwo;
    private GameObject[] waypoints;
    private int pathMinion;
    public int currentWayPoint = 0;
    public float rotSpeed = 2f;
    public float speed = 10f;
    float accuracyWayPoint = 15.0f;
    public int attackRate = 15;
    public float projectileRange = 5f;
    public float projectileSpeed = 180f;
    public float projectileDamage = 5f;
    public float nextTimeToAttack = 0f;
    Vector3 direction;
    public float count;

    public GameObject loot;
    public Base team;


    //public Transform player;
    public Transform head;

    [SerializeField]
    private LivingBeing livingBeing;


    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.Find("Player").transform;
    }

    public void setMinion(int getNum, Base getTeam, float getCount)
    {
        pathMinion = getNum;
        team = getTeam;
        count = getCount;
        if (pathMinion == 1) waypoints = wayPointsOne;
        else if (pathMinion == 2) waypoints = wayPointsTwo;
        switch (team.GetTeam())
        {
            case "Blue":
                currentWayPoint = 0;
                setTeamColor(new Color(0f / 255f, 191f / 255f, 188f / 255f, .81f));
                break;
            case "Red":
                currentWayPoint = 4;
                setTeamColor(new Color(255f / 255f, 0f / 255f, 10f / 255f, .4f));
                break;
        }
    }



    // Update is called once per frame
    void Update()
    {
       // NetworkMain.minionPositionalUpdate(gameObject.name, team.GetTeam(), pathMinion, count, transform.position, direction);


        if (state == "patrol" && waypoints.Length > 0)
        {
            if (Vector3.Distance(waypoints[currentWayPoint].transform.position, transform.position) < accuracyWayPoint)
            {
                if (team.GetTeam() == "Blue") currentWayPoint++;
                else currentWayPoint--;
                if (currentWayPoint >= waypoints.Length || currentWayPoint < waypoints.Length)
                {
                    state = "stop";
                }
            }

            direction = waypoints[currentWayPoint].transform.position - transform.position;
            this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);
            this.transform.Translate(0, 0, Time.deltaTime * speed);
            livingBeing.legsAnimator.SetBool("Walking", true);

        }
        Transform closest = GetClosestEnemy();
        if (closest != null)
        {
            direction = closest.position - this.transform.position;
            float angle = Vector3.Angle(direction, head.forward);
            if (Vector3.Distance(closest.position, this.transform.position) < projectileRange * 1.5 && (angle < 80 || state == "pursuing"))
            {
                state = "pursuing";
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);
                //Does not aim correctly
                if (direction.magnitude > projectileRange && !Physics.Raycast(transform.position, head.forward, out var hit, 20f))
                {
                    this.transform.Translate(0, 0, Time.deltaTime * speed);
                    livingBeing.legsAnimator.SetBool("Walking", true);
                }
                else
                {
                    if (Time.time >= nextTimeToAttack)
                    {
                        livingBeing.legsAnimator.SetBool("Walking", false);
                        //livingBeing.rightHand.LookAt(closest);

                        nextTimeToAttack = Time.time + 1f / attackRate;
                        Attack(closest);
                    }
                }
            }
            else
            {
                state = "patrol";
            }
        }
        else
        {
            state = "patrol";
        }
    }

    public void serverControl(Dictionary<string, string> payload)
    {
        this.transform.position = StringUtils.getVectorFromJson(payload, "Pos");// new Vector3(float.Parse(payload["xPos"]), float.Parse(payload["yPos"]), float.Parse(payload["zPos"]));
                                                                                //        hand.transform.localRotation = Quaternion.Euler(float.Parse(payload["xRot"]), 0, 0);
        transform.eulerAngles = new Vector2(0, float.Parse(payload["yRot"]));
    }

    public void setTeamColor(Color getColor)
    {
        livingBeing.headMesh.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", getColor);
        livingBeing.bodyMesh.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", getColor);
        livingBeing.legsMesh.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", getColor);
    }

    void Attack(Transform closestEnemy)
    {
        if (livingBeing.currentWeapon.TryGetComponent<UsableItemInterface>(out UsableItemInterface getUsable))
        {
            if (getUsable.isUsable())
            {
                getUsable.fireOne();
            } else 
            {
                getUsable.reload();
            }
        }
        //        holding.GetComponent<Gun>().shoot(transform, true);
        //GameObject lazurBeem = Instantiate(Resources.Load<GameObject>("Laser Beam"), transform.position + transform.forward * 3f, transform.rotation);
        //lazurBeem.GetComponent<Projectile>().setProjectile(projectileSpeed, projectileRange, projectileDamage);
    }

    private Transform GetClosestEnemy()
    {
        GameObject[] gos = null;
        if (transform.gameObject.CompareTag("Blue")) gos = GameObject.FindGameObjectsWithTag("Red");
        if (transform.gameObject.CompareTag("Red")) gos = GameObject.FindGameObjectsWithTag("Blue");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest != null ? closest.transform : null;
    }

}
