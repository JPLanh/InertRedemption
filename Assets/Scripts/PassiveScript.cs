using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveScript : MonoBehaviour
{
    public GameObject bit;
    public Base team;
    public float counter = 0;
    public float spawnTimer = 100f * 60f * 60f * 5f;
    public float nextSpawn = 0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkMain.local)
        {
            if (Time.time > nextSpawn)
            {
                nextSpawn = Time.time + spawnTimer;
                if (team != null)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dictionary<string, string> payload = StringUtils.getPayload();
                        payload["Action"] = "Spawn Minion";
                        payload["Team"] = team.GetTeam();
                        payload["Count"] = StringUtils.convertFloatToString(counter);
                        spawnMinion(StringUtils.convertFloatToString(counter));
                        counter = (counter + 1) % 50;
                    }
                }
            }
        }
    }

    public void spawnMinion(string getCount)
    {
        if (transform.childCount < 30)
        {
            GameObject entity1 = Instantiate(bit, transform.position + transform.forward, Quaternion.identity);
            entity1.GetComponent<Entity>().setMinion(1, team, StringUtils.convertToFloat(getCount));
            entity1.transform.SetParent(transform);
            entity1.gameObject.tag = team.GetTeam(); ;
            entity1.gameObject.name = team.GetTeam() + " " + 1 + " " + getCount;
            //NetworkListener.playerUpdates.Add(team.GetTeam() + " " + 1 + " " + getCount, new Stack<string>());

            GameObject entity2 = Instantiate(bit, transform.position, Quaternion.identity);
            entity2.GetComponent<Entity>().setMinion(2, team, StringUtils.convertToFloat(getCount));
            entity2.transform.SetParent(transform);
            entity2.gameObject.tag = team.GetTeam(); ;
            entity2.gameObject.name = team.GetTeam() + " " + 2 + " " + getCount;

            //NetworkListener.playerUpdates.Add(team.GetTeam() + " " + 2 + " " + getCount, new Stack<string>());

        }
    }

    public void spawnMinionSpecific(Dictionary<string,string> payload)
    {
        Vector3 pos = StringUtils.getVectorFromJson(payload, "Pos"); 
        GameObject entity1 = Instantiate(bit, pos, Quaternion.identity);
        entity1.GetComponent<Entity>().setMinion(StringUtils.convertToInt(payload["MinionNum"]), team, StringUtils.convertToFloat(payload["Count"]));
        entity1.transform.SetParent(transform);
        entity1.gameObject.tag = team.GetTeam(); ;
        entity1.gameObject.name = team.GetTeam() + " " + payload["MinionNum"] + " " + payload["Count"];
        //NetworkListener.playerUpdates.Add(team.GetTeam() + " " + payload["MinionNum"] + " " + payload["Count"], new Stack<string>());

    }
}
