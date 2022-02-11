using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusBody : MonoBehaviour
{
    public float health;
    public float speed;
    public float jumpSpeed;
    public float gravity = 20.0f;
    public float infectionRate = 5f;
    public float lookSensativity;
    public bool marked = false;
    public GameObject currentList;

    // Start is called before the first frame update
    void Start()
    {
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

    //public GameObject isDamage(bool network, float getValue, GameObject attacker)
    //{
    //    if (NetworkMain.local)
    //    {
    //        health += getValue;
    //        damageCheck();
    //    }
    //    else
    //    {
    //        if (!network)
    //        {
    //            health = getValue;
    //            damageCheck();
    //        }
    //        else
    //        {
    //            Dictionary<string, string> payload = new Dictionary<string, string>();
    //            payload["Target"] = gameObject.name;
    //            payload["damage"] = StringUtils.convertFloatToString(getValue);
    //            payload["Action"] = "Damage Living";
    //            //NetworkMain.messageServer(payload);
    //        }
    //    }

    //    health += getValue;
    //    return null;
    //    //        print(this);
    //}

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
                Destroy(this.gameObject);
            }
        }

    }
}
