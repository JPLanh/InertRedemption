using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Resource : MonoBehaviour, IDamagable
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
    public AudioSource damageSound;
    public AudioSource breakingSound;
    public string resource;
    public int amount;
    public string lobbyID;
    public string UID;
    public bool isDestroyed = false;
    // Start is called before the first frame update
    void Start()
    {
//        loot.GetComponent<Data>().resourceName = resource;
    }

    // Update is called once per frame
    void Update()
    {

        //if (durability <= 0 && !breakingSound.isPlaying)
        //{
        //    breakingSound.Play();
            //Dictionary<string, string> payload = new Dictionary<string, string>();
            //payload["UID"] = UID;
            //payload["Action"] = "Destroy Resource";
            //NetworkMain.broadcastAction(payload);
            //
        //}
    }

    public bool isDamage(bool network, float getValue, GameObject attacker)
    {
        //if (NetworkMain.local)
        //{
        //    durability += getValue;
        //    if (durability < 0)
        //    {
        //        breakingSound.Play();
        //        createLoot();
        //        return true;
        //    }
        //} else
        //{
        if (durability > 0)
        {
            if (NetworkMain.Username.Equals(attacker.name))
            {
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload["UID"] = UID;
                payload["Type"] = "Action";
                payload["Damage"] = StringUtils.convertFloatToString(getValue);
                payload["Action"] = "Damage Resource";
                NetworkMain.broadcastAction(payload);
            }
            return true;

        }
        return false;
    }

    IEnumerator breakingResource()
    {
        isDestroyed = true;
        breakingSound.Play();
        yield return new WaitForSeconds(4);
        createLoot();
        Destroy(gameObject);
    }

    IEnumerator exhaustingResource()
    {
        isDestroyed = true;
        breakingSound.Play();
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }

    public void damage(float getValue)
    {

        durability += getValue;
        if (durability > 0)
        {
            damageSound.Play();
        } else
        {
            StartCoroutine(breakingResource());
        }
    }

    public bool harvest(float getValue)
    {

        durability += getValue;
        if (durability > 0)
        {
            return true;
        }
        else
        {
            StartCoroutine(exhaustingResource());
            return false;
        }
    }

    private GameObject createLoot()
    {
        GameObject GO = Instantiate(Resources.Load<GameObject>("Resource Loot"), this.transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        GO.TryGetComponent<Data>(out Data lv_data);
        EntityManager.loot.Add(UID, lv_data);
        switch (resource)
        {
            case "Tree":
                lv_data.resourceName = "Log";
                break;
            case "Stone":
                lv_data.resourceName = "Stone";
                break;
        }
        lv_data.UID = UID;
        Destroy(this.gameObject);
        EntityManager.resources.Remove(UID);
        return GO;
    }
}
