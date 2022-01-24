using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigBoss : MonoBehaviour, Damagable
{
    public Vector3 walkPoint;
    bool walkPointSet = false;
    public float walkPointRange;
    public NavMeshAgent agent;
    public Transform attackPoint;
    public Vector3 focusDestination = Vector3.zero;
    public LayerMask whatIsGround, whatIsPlayer;

    public bool isEating = false;

    public float eatInterval = 3f;
    public float eatTimer = 0f;
    public Transform targetResource = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void eatingState(bool getState)
    {
        if (getState)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.SetDestination(transform.position);

        }
        else
        {
            isEating = false;
            targetResource = null;
            walkPointSet = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isEating)
        {
            if (Time.time > eatTimer)
            {
                eatTimer = Time.time + eatInterval;
                if (targetResource != null)
                {
                    if ((attackPoint.position - targetResource.position).sqrMagnitude < 1000)
                    {
                        print("Within attack");
                        GameObject destroyed = targetResource.GetComponent<Damagable>().damage(true, -10f, gameObject);

                        if (destroyed != null)
                        {
                            Destroy(destroyed);
                            eatingState(false);
                        }

                    }
                } else
                {
                    print("Disappeared");
                    eatingState(false);
                }
            }
        }
        else
        {
            if (targetResource == null)
            {
                targetResource = GetClosestResource(10000);
            }
            else
            {
                if (!walkPointSet)
                {
                    agent.SetDestination(targetResource.position);
                    print(targetResource.position);
                    walkPointSet = true;
                }
            }

        }
        //Patroling();
    }


    private Transform GetClosestResource(float getDistance)
    {
        GameObject[] gos = null;
        gos = GameObject.FindGameObjectsWithTag("Resource");
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

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
        {

            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //        print(distanceToWalkPoint + " , " + walkPoint);

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 15f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 200f, whatIsGround))
        {
            walkPointSet = true;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(walkPoint, walkPoint - new Vector3(0f, 200f, 0f));
        //        Gizmos.DrawLine(transform.position, transform.forward * 100f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.transform.name);
        //if (collision.transform.name.Equals("Resource Spawner"))
        //{
        //    print("Spawner hit this");
        //}
        //if (collision.transform.name.Equals("Resource"))
        //{
        //    Destroy(collision.gameObject);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Equals("Resource"))
        {
            print("Spawner hit this");
            //Destroy(other.transform);
        }

    }

    public GameObject damage(bool network, float getValue, GameObject attacker)
    {
        print("Damage");
        targetResource = attacker.transform;
        return null;
    }
}
