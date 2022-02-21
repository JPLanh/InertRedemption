using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using Socket.Newtonsoft.Json;
//using Socket.Newtonsoft.Json.Linq;

public class LivingBeing : MonoBehaviour, IDamagable
{
    public float health;
    public float infectionRate = 0;
    public float speed;
    public float jumpSpeed;
    public float gravity = 20.0f;
    public float lookSensativity;
    public GameObject loot;
    public bool marked = false;
    public GameObject survivorList;

    public Node currentNode;

    public Transform currentWeapon;
    public Transform offWeapon;
    public Transform mainHand;
    public Transform offHand;
    public Transform weaponHarness;
    public Transform hip;
    public Transform upperBody;

    //public Transform holding;
    //public Transform stowed;

    public Animator headAnimator;
    public Animator handAnimator;
    public Animator legsAnimator;

    public Transform headMesh;
    public Transform bodyMesh;
    public Transform leftHandMesh;
    public Transform rightHandMesh;
    public Transform legsMesh;

    public Dictionary<string, int> inventory;

    // Start is called before the first frame update
    void Start()
    {

        inventory = new Dictionary<string, int>();
        //inventory.Add("Scrap", 5);
        //inventory.Add("Wire", 2);
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position.y < -1000)
        {
            transform.position = new Vector3(transform.position.x, 20, transform.position.z);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

    }

    public void setTeamColor(Color getColor)
    {
        if(headMesh != null)
            headMesh.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", getColor);
        if (leftHandMesh != null)
            leftHandMesh.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", getColor);
        if (rightHandMesh != null)
            rightHandMesh.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", getColor);
        if (bodyMesh != null)
            bodyMesh.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", getColor);
        if (legsMesh != null)
            legsMesh.GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", getColor);
    }

    public GameObject isDamage(bool network, float getValue, GameObject attacker)
    {
        if (NetworkMain.local)
        {
            health += getValue;
            damageCheck();
        }
        else
        {
            if (!network)
            {
                health = getValue;
                damageCheck();
            }
            else
            {
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload["Target"] = gameObject.name;
                payload["damage"] = StringUtils.convertFloatToString(getValue);
                payload["Action"] = "Damage Living";
                //NetworkMain.messageServer(payload);
            }
        }

        health += getValue;
        return null;
        //        print(this);
    }

    private void damageCheck()
    {
        if (GetComponent<PlayerController>() != null)
        {
            GetComponent<PlayerController>().getDamage = true;
            if (health < 0)
            {
                GetComponent<PlayerController>().died();
            }
        }
        else
        {
            if (health < 0)
            {
                survivorList.GetComponent<Survivors>().removeMarkedTarget(this.gameObject);
                Destroy(this.gameObject);
                Instantiate(loot, this.transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
            }
        }

    }

    public void setAnimation(string getAnimation, bool getBool)
    {
        handAnimator.SetBool(getAnimation, getBool);
        headAnimator.SetBool(getAnimation, getBool);
    }

}
