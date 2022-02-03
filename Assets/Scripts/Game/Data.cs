using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour, Interactable, Displayable
{
    private float MinDistance = 10f;
    private float speed = 5f;
    public float maxLifeSpan;
    public float birthTime;
    public string resourceName;
    public string UID;
    // Start is called before the first frame update
    void Start()
    {
        birthTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Time.time > birthTime + maxLifeSpan) Destroy(gameObject);
        //Transform target = GetClosest();
        //transform.LookAt(target);
        //if (Vector3.Distance(transform.position, target.position) <= MinDistance)
        //{
        //    Vector3 follow = target.position;
        //    follow.y = this.transform.position.y;
        //    this.transform.position = Vector3.MoveTowards(this.transform.position, follow, speed * Time.deltaTime);
        //}
    }

    private Transform GetClosest()
    {
        PlayerController[] gos = GameObject.FindObjectsOfType<PlayerController>();
        PlayerController closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (PlayerController go in gos)
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

    public void Interact(PlayerController player)
    {
        if (NetworkMain.Username == player.name)
        {
            if (player.pickupItem(this))
            {
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload["UID"] = UID;
                payload["Action"] = "Pickup Item";
                NetworkMain.broadcastAction(payload);
                //                Dictionary<string, string> payload = new Dictionary<string, string>();
                //                payload["UID"] = UID;
                //                payload["Action"] = "Pickup Item";
                //print(StringUtils.convertPayloadToJson(payload));
                //                NetworkMain.messageServer(payload);
//                Destroy(gameObject);
            }
        }
    }

    public void pickupItem()
    {
        Destroy(gameObject);
        EntityManager.loot.Remove(UID);
    }
    public string display()
    {
        return resourceName;
    }
}
