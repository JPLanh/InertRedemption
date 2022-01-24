using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour, Damagable
{

    public float durability;
    public GameObject loot;
    public string resourceName;
    public int amount;
    public string UID;
    // Start is called before the first frame update
    void Start()
    {
        loot.GetComponent<Data>().resourceName = resourceName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject damage(bool network, float getValue, GameObject attacker)
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
            if (!network)
            {
                durability = getValue;
                if (durability < 0)
                {
                    Dictionary<string, string> payload = StringUtils.getPositionAndRotation(transform.position, transform.rotation);
                    Destroy(this.gameObject);
                    payload["UID"] = UID;
                    payload["lobbyID"] = NetworkMain.LobbyID;
                    payload["resource"] = resourceName;
                    payload["Action"] = "Spawn Item";
                    NetworkMain.messageServer(payload);
                }
            } else
            {
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload["UID"] = UID;
                payload["damage"] = StringUtils.convertFloatToString(getValue);
                payload["Action"] = "Damage Resource";
                NetworkMain.messageServer(payload);
            }
        }
        return null;
    }

    private GameObject createLoot()
    {
        GameObject GO = Instantiate(Resources.Load<GameObject>("Resource Loot"), this.transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        GO.transform.GetComponent<Data>().resourceName = resourceName;
        GO.transform.GetComponent<Data>().UID = UID;
        Destroy(this.gameObject);
        return GO;


    }
}
