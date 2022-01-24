using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Socket.Newtonsoft.Json;
using Socket.Newtonsoft.Json.Linq;

public class NetworkListener : MonoBehaviour
{
    public static Dictionary<string, Stack<string>> playerUpdates;
    // Start is called before the first frame update
    void Start()
    {
        playerUpdates = new Dictionary<string, Stack<string>>();
        NetworkMain.socket.Emit("WhoAmI");
    }

    //void OnDestroy()
    //{
    //    NetworkMain.disconnect();
    //}

    // Update is called once per frame
    void Update()
    {

        //foreach (KeyValuePair<string, Stack<string>> getpay in playerUpdates)
        //{
        //    //getString += getpay.Key + " = " + getpay.Value + "\n";
        //if (getpay.Value.Count > 0){
        //        print("Update " + getpay.Key);
        //    string message = getpay.Value.Pop();
        //    playerUpdates[getpay.Key] = new Stack<string>();
        //    MSG getJson = JsonConvert.DeserializeObject<MSG>(message);

        //    if (getJson.Payload.ContainsKey("Minion"))
        //    {
        //        GameObject.Find(getJson.Payload["Minion"]).GetComponent<Entity>().serverControl(getJson.Payload);
        //    } else
        //    {
        //        GameObject getPlayer = GameObject.Find(getJson.Payload["userID"]);
        //    if (getPlayer == null) NewPlayerLogic.existingPlayer(getJson.Payload);
        //    else GameObject.Find(getJson.Payload["userID"]).GetComponent<PlayerController>().serverControl(getJson.Payload);
        //    }
        //}
        //}


        if (NetworkMain.updateResponse.Count > 0)
        {
            string message = NetworkMain.updateResponse.Pop();
            NetworkMain.updateResponse = new Stack<string>();
            Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
            /*            List<Dictionary<string, string>> massPayload = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(message);

                        foreach (Dictionary<string, string> payload in massPayload)
                        {*/
            if (payload.ContainsKey("Minion"))
            {
                GameObject getMinion = GameObject.Find(payload["Minion"]);
                if (getMinion == null)
                {
                    GameObject.Find(payload["Team"] + " Base").transform.GetChild(3).transform.GetComponent<PassiveScript>().spawnMinionSpecific(payload);

                }
                else
                {
                    GameObject.Find(payload["Minion"]).GetComponent<Entity>().serverControl(payload);
                }
            }
            else
            {
                GameObject getPlayer = GameObject.Find(payload["userID"]);
                if (getPlayer == null) { 
                    //if (payload.ContainsKey("Team"))
                        //NewPlayerLogic.existingPlayer(payload); 
                }
                else GameObject.Find(payload["userID"]).GetComponent<PlayerController>().serverControl(payload);
            }
            //}
        }


//        if (NetworkMain.serverResponse.Count > 0)
//        {
////            string message = NetworkMain.serverResponse.Dequeue();
//            //            MSG getJson = JsonConvert.DeserializeObject<MSG>(message);
////            print(message);
////            Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

//            switch (payload["Action"])
//            {
//                case "Enter":
//                    Dictionary<string, string> newPayload = loadPlayer(payload["userID"]);
//                    NetworkMain.socket.Emit("Player", JsonConvert.SerializeObject(newPayload));
//                    //GameObject.Find(payload["userID"]).GetComponent<PlayerController>().setActivePlayer(payload["userID"], newPayload["Team"]);
//                    break;
//                case "Join":
//                    //GameObject.Find("Scripts").GetComponent<NewPlayerLogic>().loadPlayer(payload["userID"]);
//                    //GameObject.Find(payload["userID"]).GetComponent<PlayerController>().setOtherPlayer(payload["userID"], payload["Team"]);
//                    playerUpdates.Add(payload["userID"], new Stack<string>());
//                    //                    NetworkMain.addNewUser(payload["userID"]);
//                    break;
//                case "Spot shot":
//                    //GameObject.Find(payload["userID"]).GetComponent<PlayerController>().gunHolding.GetComponent<Gun>().muzzleFlash.Play();
//                    GameObject objHit = Instantiate(Resources.Load<GameObject>("Hit Spark"),
//                        new Vector3(float.Parse(payload["xPos"]), float.Parse(payload["yPos"]), float.Parse(payload["zPos"])),
//                        Quaternion.LookRotation(new Vector3(float.Parse(payload["xRot"]), float.Parse(payload["yRot"]), float.Parse(payload["zRot"])))
//                        );
//                    Destroy(objHit, 1f);
//                    break;
//                case "Damage":
//                    GameObject.Find(payload["Target"]).GetComponent<LivingBeing>().damage(int.Parse(payload["Value"]));
//                    break;
//                case "Spawn Minion":
//                    GameObject.Find(payload["Team"] + " Base").transform.GetChild(3).transform.GetComponent<PassiveScript>().spawnMinion(payload["Count"]);
//                    break;
//                default:
//                    //print(message);
//                    GameObject.Find(payload["userID"]).GetComponent<PlayerController>().serverResponse(payload);
//                    break;
//            }

//        }
    }

    public Dictionary<string, string> loadPlayer(string getName)
    {

        Dictionary<string, string> payload = new Dictionary<string, string>();
        GameObject blueBase = GameObject.Find("Blue Base");
        GameObject redBase = GameObject.Find("Red Base");
        GameObject newPlayer = Instantiate(Resources.Load<GameObject>("Player"), transform.position, Quaternion.identity);
        if (blueBase.transform.GetChild(4).childCount == redBase.transform.GetChild(4).childCount && blueBase.transform.GetChild(4).childCount == 0)
        {
            blueBase.GetComponent<Base>().isHost = true;
            redBase.GetComponent<Base>().isHost = true;
        }

        if (blueBase.transform.GetChild(4).childCount <= redBase.transform.GetChild(4).childCount)
        {
            newPlayer.transform.SetParent(blueBase.transform.GetChild(4));
            payload["Action"] = "Join";
            payload["Team"] = "Blue";
        }
        else
        {
            newPlayer.transform.SetParent(redBase.transform.GetChild(4));
            payload["Action"] = "Join";
            payload["Team"] = "Red";
        }
        newPlayer.transform.name = getName;

        return payload;
    }
}
