using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Resource : MonoBehaviour, Damagable
{
    public string _id;
    public string __v;
    public float xPos;
    public float yPos;
    public float zPos;
    public float xRot;
    public float yRot;
    public float zRot;
    public float durability;
    [NonSerialized] public GameObject loot;
    public string resource;
    public int amount;
    public string lobbyID;
    public string UID;
    // Start is called before the first frame update
    void Start()
    {
//        loot.GetComponent<Data>().resourceName = resource;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject isDamage(bool network, float getValue, GameObject attacker)
    {
        if (NetworkMain.local)
        {
            durability += getValue;
            if (durability < 0)
            {
                return createLoot();
                
            }
        } else
        {
            Debug.Log("Netwokring damage");
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload["UID"] = UID;
                payload["Damage"] = StringUtils.convertFloatToString(getValue);
                payload["Action"] = "Damage Resource";
                NetworkMain.broadcastAction(payload);
//                NetworkMain.messageServer(payload);
        }
        return null;
    }

    public void damage(float getValue)
    {
        durability += getValue;
        if (durability <= 0)
        {
            createLoot();
            //Dictionary<string, string> payload = new Dictionary<string, string>();
            //payload["UID"] = UID;
            //payload["Action"] = "Destroy Resource";
            //NetworkMain.broadcastAction(payload);
            //
        }
    }

    private GameObject createLoot()
    {
        GameObject GO = Instantiate(Resources.Load<GameObject>("Resource Loot"), this.transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        GO.TryGetComponent<Data>(out Data lv_data);
        EntityManager.loot.Add(UID, lv_data);
        lv_data.resourceName = resource;
        lv_data.UID = UID;
        Destroy(this.gameObject);
        EntityManager.resources.Remove(UID);
        return GO;


    }
}
