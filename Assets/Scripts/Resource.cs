using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Resource : MonoBehaviour, IDamagable, Displayable
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
    public float progressCounter = 0;
    public bool resourceTrapped = false;
    public int amount;
    public string lobbyID;
    public string UID;
    public bool isDestroyed = false;
    public GameObject resourceObj;
    public GameObject trapObj;
    public Collider resourceCollision;
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

    public bool trapping(float in_value)
    {
        float lv_progress = progressCounter - in_value * 5;
        if (lv_progress >= 100)
        {
            if (!resourceTrapped)
            {
                harvest(in_value);
                progressCounter = lv_progress;
                resourceTrapped = true;
                if (NetworkMain.Team.Equals("Virus"))
                {
                    trapObj.SetActive(true);
                }
                return true;
            }
        } else
        {
            harvest(in_value);
            progressCounter = lv_progress;
            return true;
        }
        return false;
    }

    IEnumerator breakingResource()
    {
        isDestroyed = true;
        breakingSound.Play();
        Destroy(resourceObj);
        Destroy(resourceCollision);
        createLoot();
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }

    IEnumerator exhaustingResource()
    {
        isDestroyed = true;
        breakingSound.Play();
        Destroy(resourceObj);
        Destroy(resourceCollision);
        createLoot();
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }

    public void damage(string in_user, float getValue)
    {
        durability += getValue;
        if (durability > 0)
        {
            damageSound.Play();
        } else
        {
            EntityManager.survivors.TryGetValue(in_user, out PlayerController out_player);
            if (out_player != null)
            {
                if (resourceTrapped)
                {

                    GameObject lv_new_affliction = Instantiate(Resources.Load<GameObject>("Afflictions/Fear"), out_player.affliction_lists.transform);
                    lv_new_affliction.TryGetComponent<Affliction_Fear>(out Affliction_Fear out_affliction);
                    out_affliction.lv_player = out_player;
                    out_affliction.init(9, 5);
                }
            }
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
        EntityManager.resources.Remove(UID);
        return GO;
    }

    public string display()
    {
        if (NetworkMain.Team.Equals("Survivor"))
        {
            return "This is just a resource";
        } else
        {
            String displayOut = $"Durability: {durability}\n";
            if (progressCounter > 0) displayOut += $"Trapping in Progress: {progressCounter}";
            return displayOut;
        }
    }
}
