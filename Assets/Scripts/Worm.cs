using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worm : MonoBehaviour, WeaponBaseInterface
{

    public float health = 100;

    private string state = "active";
    public GameObject[] wayPointsOne;
    public GameObject[] wayPointsTwo;
    private GameObject[] waypoints;
    public int pathMinion;
    public int currentWayPoint;
    public float rotSpeed;
    public float speed;
    float accuracyWayPoint = 15.0f;
    public float attackRate;
    public float projectileRange;
    public float projectileSpeed;
    public float projectileDamage;
    public float nextTimeToAttack = 0f;
    public float attackDistance;
    public float dayVisionDistance;
    public float nightVisionDistance;
    private float dayVisionDistanceSqrt;
    private float nightVisionDistanceSqrt;
    Vector3 direction;
    public float count;
    [SerializeField]
    private Light frontalLight;
    [SerializeField]
    private NPCSensors visionSensor;
    public TimeSystem gameTime;

    public GameObject loot;
    public Base team;

    //public Transform player;
    public Transform head;

    [SerializeField]
    private LivingBeing livingBeing;
    private Rigidbody rb;


    //AI


    public NavMeshAgent agent;

    public Transform closestTarget;

    public LayerMask whatIsGround, whatIsPlayer;


    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public Vector3 focusDestination = Vector3.zero;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;



    // Start is called before the first frame update
    void Start()
    {
        dayVisionDistanceSqrt = dayVisionDistance * dayVisionDistance;
        nightVisionDistanceSqrt = nightVisionDistance * nightVisionDistance;
        rb = GetComponent<Rigidbody>();
        //player = GameObject.Find("Player").transform;
    }

    void Awake()
    {

        agent = GetComponent<NavMeshAgent>();
    }

    public void setMinion(int getNum, Base getTeam, float getCount)
    {
        pathMinion = getNum;
        team = getTeam;
        count = getCount;
        gameObject.tag = team.GetTeam();
        if (pathMinion == 1) waypoints = wayPointsOne;
        else if (pathMinion == 2) waypoints = wayPointsTwo;
        switch (team.GetTeam())
        {
            case "Blue":
                currentWayPoint = 4;
                setTeamColor(new Color(0f / 255f, 191f / 255f, 188f / 255f, .81f));
                break;
            case "Red":
                currentWayPoint = 1;
                setTeamColor(new Color(255f / 255f, 0f / 255f, 10f / 255f, .4f));
                break;
        }
    }

    public void setMinion(TimeSystem getTime)
    {
        gameTime = getTime;
        setTeamColor(new Color(255f / 255f, 0f / 255f, 10f / 255f, .4f));
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 2f);
    }

    void FixedUpdate()
    {
//        rb.velocity = direction;
        if(agent.velocity.sqrMagnitude != 0)
        {
            livingBeing.legsAnimator.SetBool("isWalking", true);
        } else
        {
            livingBeing.legsAnimator.SetBool("isWalking", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // NetworkMain.minionPositionalUpdate(gameObject.name, team.GetTeam(), pathMinion, count, transform.position, direction);

        //if (state == "patrol" && waypoints.Length >= 0)
        //{
        //    if (currentWayPoint <= waypoints.Length && currentWayPoint > 0)
        //    {
        //        if (Vector3.Distance(waypoints[currentWayPoint].transform.position, transform.position) < accuracyWayPoint)
        //        {
        //            if (team.GetTeam() == "Blue") currentWayPoint++;
        //            else currentWayPoint--;
        //        }

        //    }
        //    direction = waypoints[currentWayPoint].transform.position - transform.position;
        //    this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);
        //    this.transform.Translate(0, 0, Time.deltaTime * speed);
        //    livingBeing.legsAnimator.SetBool("isWalking", true);

        //}
        //if ((gameTime.getMinute() >= 0 && gameTime.getMinute() <= 720))
        //{
        //    dayMode();
        //} else
        //{
        //    nightMode();
        //}

        //Check for sight and attack range
        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        //        print(playerInSightRange + " ("+sightRange +") , " + playerInAttackRange + " ("+attackRange +")");
        //        if (!playerInSightRange && !playerInAttackRange) Patroling();
        //if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        //if (playerInSightRange) ChasePlayer();
        //if (playerInAttackRange && playerInSightRange) AttackPlayer();

        //        agent.destination = GetClosestEnemy(dayVisionDistanceSqrt).transform.position;

        if (visionSensor.focusTarget != null)
        {
            focusDestination = visionSensor.focusTarget.transform.position;            
        }



        if (focusDestination != Vector3.zero)
        {
            if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
            {
                focusDestination = Vector3.zero;
            }
            if (playerInAttackRange) AttackPlayer();
            else ChasePlayer(focusDestination);
        } else
        {
            Patroling();
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 10f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer(Vector3 chaseDestination)
    {
        //print(chaseDestination);
        //Vector3 dir = (chaseDestination - transform.position).normalized;
        //Quaternion qDir = Quaternion.LookRotation(dir);
//        rb.rotation = Quaternion.Slerp(transform.rotation, qDir, Time.deltaTime * rotSpeed);

        //        closestTarget = GetClosestEnemy(dayVisionDistanceSqrt);
        agent.SetDestination(chaseDestination);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(closestTarget);

        if (!alreadyAttacked)
        {
            ///Attack code here
            //Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            targetConfirm(closestTarget);
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    //private void nightMode()
    //{
    //    pursueTarget(nightVisionDistance, nightVisionDistanceSqrt);
    //}

    //private void dayMode()
    //{
    //    pursueTarget(dayVisionDistance, dayVisionDistanceSqrt);
    //}



    //public void pursueTarget(float visionRadius, float visionRadiusSqrt)
    //{
    //    if (Physics.Raycast(transform.position, transform.forward, out var hit, attackDistance))
    //    {
    //        if (hit.collider.tag == "Viruses")
    //        {
    //            rb.MovePosition(transform.position + new Vector3(Time.deltaTime * speed, 0, 0));
    //            livingBeing.legsAnimator.SetBool("isWalking", true);
    //            //                print("Get outta mah way vbish");
    //        }
    //    }
    //    Transform closest = GetClosestEnemy(visionRadiusSqrt);
    //    if (closest != null)
    //    {
    //        direction = closest.position - this.transform.position;
    //        float angle = Vector3.Angle(direction, head.forward);
    //        if (Vector3.Distance(closest.position, this.transform.position) < visionRadius)// && (angle < 80 || state == "pursuing"))
    //        {
    //            if (frontalLight.gameObject.activeInHierarchy) frontalLight.gameObject.SetActive(true);
    //            state = "pursuing";
    //            //                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);
    //            rb.MoveRotation(Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime));

    //            //                    print(direction.magnitude);
    //            if (direction.magnitude > attackDistance)
    //            {
    //                //rb.MovePosition(transform.position + new Vector3(0, 0, Time.deltaTime * speed));
    //                //rb.MovePosition(transform.position + new Vector3(0, 0, Time.deltaTime * speed));
    //                //                    this.transform.Translate(0, 0, Time.deltaTime * speed);
    //                livingBeing.legsAnimator.SetBool("isWalking", true);
    //            }
    //            else
    //            {
    //                targetConfirm(closest);
    //            }
    //        }
    //        else
    //        {
    //            state = "idle";
    //            if (frontalLight.gameObject.activeInHierarchy) frontalLight.gameObject.SetActive(false);
    //        }
    //    }
    //    else
    //    {
    //        state = "idle";
    //        if (frontalLight.gameObject.activeInHierarchy) frontalLight.gameObject.SetActive(false);
    //    }
    //}

    public void serverControl(Dictionary<string, string> payload)
    {
        this.transform.position = StringUtils.getVectorFromJson(payload, "Pos");// new Vector3(float.Parse(payload["xPos"]), float.Parse(payload["yPos"]), float.Parse(payload["zPos"]));
                                                                                //        hand.transform.localRotation = Quaternion.Euler(float.Parse(payload["xRot"]), 0, 0);
        transform.eulerAngles = new Vector2(0, float.Parse(payload["yRot"]));
    }

    public void targetConfirm(Transform closest)
    {
//        if (Physics.Raycast(transform.position, transform.forward, out var hit, attackDistance))
        {
            if (Time.time >= nextTimeToAttack)
            {
                nextTimeToAttack = Time.time + 1f / attackRate;

//                if (transform.tag != hit.transform.gameObject.tag)
                {
                    livingBeing.legsAnimator.SetBool("isWalking", false);
                    Attack(closest);
                }
            }
        }
    }
    public void setTeamColor(Color getColor)
    {
        //livingBeing.headMesh.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", getColor);
        //livingBeing.bodyMesh.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", getColor);
        //livingBeing.legsMesh.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", getColor);
    }

    void Attack(Transform closestEnemy)
    {
        livingBeing.handAnimator.SetBool("isAttacking", true);
    }

    private Transform GetClosestEnemy(float getDistance)
    {
        GameObject[] gos = null;
        gos = GameObject.FindGameObjectsWithTag("Survivors");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance && diff.sqrMagnitude < getDistance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest != null ? closest.transform : null;
    }

    public void damageTrigger(Collider other)
    {
        if (livingBeing.handAnimator.GetBool("isAttacking"))
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            Damagable target = null;
            target = other.transform.GetComponent<Damagable>();
            if (other.transform.parent != null && target == null) target = other.transform.parent.transform.GetComponent<Damagable>();

            if (target != null)
            {
                //                print(target + " Damage ");
                target.damage(true, projectileDamage, gameObject);
            }
            if (!other.transform.GetComponent<NodeCollision>())
            {
                livingBeing.handAnimator.SetBool("isAttacking", false);
                //                durabilityDamage(-5);
            }
        }
    }

    public void getDamaged()
    {
        state = "active";
    }
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(walkPoint, walkPoint - new Vector3(0f, 100f, 0f));
        //        Gizmos.DrawLine(transform.position, transform.forward * 100f);
    }
}
