using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Socket.Newtonsoft.Json;

public class NewPlayerLogic : MonoBehaviour
{
    public bool loaded = false;
//    private int nodeCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        //while (nodeCounter < 1)
        //{
        //    var position = new Vector3(Random.Range(-500.0f, 500.0f), 0, Random.Range(-500.0f, 500.0f));
        //    if (!(position.x > 350f && position.z > 350f) && !(position.x < -350f && position.z < -350f) && GetClosestResource(position) > 3)
        //    {
        //        GameObject newNode = Instantiate(Resources.Load<GameObject>("Node"), position, 
        //            Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0));
        //        newNode.name = "Node " + nodeCounter;
        //        newNode.transform.SetParent(GameObject.Find("Node List").transform);
        //        nodeCounter++;
        //    }
        //}
    }
    private float GetClosestResource(Vector3 getPosition)
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Resource");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = getPosition;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return distance;
    }

    //public static void existingPlayer(Dictionary<string, string> payload)
    //{

    //    GameObject newPlayer = Instantiate(Resources.Load<GameObject>("Player"), new Vector3(0,0,0), Quaternion.identity);
    //    newPlayer.transform.SetParent(GameObject.Find(payload["Team"] + " Base").transform.GetChild(4));
    //    newPlayer.GetComponent<PlayerController>().setOtherPlayer(payload["userID"], payload["Team"]);
    //    newPlayer.GetComponent<PlayerController>().serverControl(payload);
    //}

    //public Dictionary<string, string> loadPlayer(string getName)
    //{

    //    Dictionary<string, string> payload = new Dictionary<string, string>();
    //    GameObject blueBase = GameObject.Find("Blue Base");
    //    GameObject redBase = GameObject.Find("Red Base");
    //    GameObject newPlayer = Instantiate(Resources.Load<GameObject>("Player"), transform.position, Quaternion.identity);
    //    if (blueBase.transform.GetChild(4).childCount == redBase.transform.GetChild(4).childCount && blueBase.transform.GetChild(4).childCount == 0)
    //    {
    //        blueBase.GetComponent<Base>().isHost = true;
    //        redBase.GetComponent<Base>().isHost = true;
    //    }

    //    if (blueBase.transform.GetChild(4).childCount <= redBase.transform.GetChild(4).childCount)
    //    {
    //        newPlayer.transform.SetParent(blueBase.transform.GetChild(4));
    //        payload["Action"] = "Join";
    //        payload["Team"] = "Blue";
    //    }
    //    else
    //    {
    //        newPlayer.transform.SetParent(redBase.transform.GetChild(4));
    //        payload["Action"] = "Join";
    //        payload["Team"] = "Red";
    //    }

    //    return payload;
    //}

    // Update is called once per frame
    void Update()
    {

        //if (!loaded && Time.time > 10)
        //{
        //    loaded = true;
        //    Dictionary<string, string> payload = loadPlayer("Local");
        //    NetworkMain.socket.Emit("Player", JsonConvert.SerializeObject(payload));
        //    GameObject.Find("Local").GetComponent<PlayerController>().setActivePlayer("Local", payload["Team"]);

        //}
    }
}
