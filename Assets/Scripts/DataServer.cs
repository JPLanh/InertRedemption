using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataServer : MonoBehaviour, IDamagable
{
    public int data;
    public int rate;
    public int amount;
    public float health;
    public Transform hardDrive;
    private HardDrive hardDriveScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hardDrive != null)
        {
            if (data > 0)
            {
                data += hardDrive.GetComponent<HardDrive>().transfer(rate, amount);
            }
            if (data <= 0) Destroy(this);
        }
    }

    public void Interact(Transform getPlayer)
    {
        if (!transform.Find("Hard Drive"))
        {
            if (getPlayer.Find("Hard Drive"))
            {
                getPlayer.Find("Hard Drive").SetParent(transform);
                transform.Find("Hard Drive").position = transform.position;
            }
        }
        else
        {
            if (!getPlayer.Find("Hard Drive"))
            {
                transform.Find("Hard Drive").SetParent(getPlayer);
                getPlayer.Find("Hard Drive").position = getPlayer.position;
            }
        }
        hardDrive = transform.Find("Hard Drive");
        hardDriveScript = (hardDrive == null ? null : hardDrive.GetComponent<HardDrive>());
    }


    public GameObject isDamage(bool network, float getValue, GameObject attacker)
    {
        health += getValue;
        if (health <= 0)
        {
            print(gameObject.tag + " loses");
        }
        return null;
    }
}
