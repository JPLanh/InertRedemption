using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Socket.Newtonsoft.Json;
using Socket.Newtonsoft.Json.Linq;

public class NetworkListenerInterpolation : MonoBehaviour
{

    public static Dictionary<string, Stack<string>> playerUpdates;
    public EntityManager em;
    public bool ready = false;
    public float timeChecker = 0;
    // Start is called before the first frame update
    void Start()
    {

        if (!NetworkMain.local)
        {
            //Dictionary<string, string> localPlayer = StringUtils.getPayload();
            //localPlayer["Username"] = NetworkMain.Username;
            //localPlayer["UserID"] = NetworkMain.UserID;
            //em.spawnPlayer(localPlayer);

            //Dictionary<string, string> payload = StringUtils.getPayload();
            //payload["Action"] = "Begin Game";
            //NetworkMain.sendString(payload);

            playerUpdates = new Dictionary<string, Stack<string>>();
            //NetworkMain.socket.Emit("Enter Game");
        }
    }

    void FixedUpdate()
    {

    }

    void OnDestroy()
    {
        if (!NetworkMain.local)
            NetworkMain.disconnect();
    }

    // Update is called once per frame
    void Update()
    {
        if (!NetworkMain.local)
        // Not QSocket
        {

            //    if (NetworkMain.updateRecResponse.Count > 0)
            //    {
            //        Dictionary<string, string> getUpdatePayload = NetworkMain.updateRecResponse.Pop();
            //        NetworkMain.updateRecResponse = new Stack<Dictionary<string, string>>();
            //        Dictionary<string, string> updatePayload = new Dictionary<string, string>();
            //        GameObject updateFindGO = GameObject.Find(getUpdatePayload["Username"]);
            //        //if (NetworkMain.clientType == "Server")
            //        //{
            //        //    NetworkMain.broadcastClients(getUpdatePayload);
            //        //}

            //        if (getUpdatePayload["Username"] != NetworkMain.Username)
            //        {
            //            print("Not this persoin");
            //            if (updateFindGO == null)
            //            {
            //                em.spawnPlayer(getUpdatePayload);
            //            }
            //            else
            //            {
            //                updateFindGO.GetComponent<PlayerController>().serverControl(getUpdatePayload);

            //            }
            //        } else
            //        {

            //            print("this persoin");
            //        }
            //    }

        //    if (NetworkMain.serverResponse.Count > 0)
        //    {
        //        Dictionary<string, string> getPayload = NetworkMain.serverResponse.Dequeue();
        //        NetworkMain.serverResponse = new Queue<Dictionary<string, string>> ();
        //        Dictionary<string, string> payload = new Dictionary<string, string>();
        //        print(StringUtils.convertPayloadToJson(payload));
        //        GameObject findGO = GameObject.Find(getPayload["Username"]);
        //        switch (getPayload["Action"])
        //        {
        //            case "Fire1":
        //                findGO.GetComponent<PlayerController>().fireOne();
        //                break;
        //            case "Fire2":
        //                //findGO.GetComponent<PlayerController>().aim(false);
        //                break;
        //            case "Fire1Up":
        //                findGO.GetComponent<PlayerController>().singleHandUse = false;
        //                break;
        //            case "Reload":
        //                findGO.GetComponent<PlayerController>().reload(false);
        //                break;
        //            case "Swap holding":
        //                findGO.GetComponent<PlayerController>().swapGun(false);
        //                break;
        //            case "Swap Gun":
        //                findGO.GetComponent<PlayerController>().swapGun(false);
        //                break;
        //            case "Menu":
        //                findGO.GetComponent<PlayerController>().accessMenu(false);
        //                break;
        //            case "Build":
        //                findGO.GetComponent<PlayerController>().buildModeSwitch();
        //                break;
        //        }

        //    }
        }

        //Dictionary<string, string> getPayload = NetworkMain.serverRecResponse.Dequeue();
        //switch (getPayload["Action"])
        //{
        //    case "Ready Check":

        //}
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

        //if (!NetworkMain.local)
        //{
        if (NetworkMain.updateResponseItnterpolation.Count > 0)
        {
            string message = NetworkMain.updateResponseItnterpolation.Pop();
            NetworkMain.updateResponseItnterpolation = new Stack<string>();
            //Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
            List<Dictionary<string, string>> massPayload = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(message);

            foreach (Dictionary<string, string> payload in massPayload)
            {
                //if (payload.ContainsKey("Minion"))
                //{
                //    GameObject getMinion = GameObject.Find(payload["Minion"]);
                //    if (getMinion == null)
                //    {
                //        GameObject.Find(payload["Team"] + " Base").transform.GetChild(3).transform.GetComponent<PassiveScript>().spawnMinionSpecific(payload);

                //    }
                //    else
                //    {
                //        GameObject.Find(payload["Minion"]).GetComponent<Entity>().serverControl(payload);
                //    }
                //}
                //else
                //{
                //    GameObject getPlayer = GameObject.Find(payload["Username"]);
                //    if (getPlayer == null)
                //    {
                //        //Fix this to allow new joinings
                //        //if (payload["State"] == "Alive")
                //        //existingPlayer(payload);
                //    }
                //    else GameObject.Find(payload["Username"]).GetComponent<PlayerController>().serverControl(payload);
                //}
            }
        }



        //    if (NetworkMain.serverResponse.Count > 0)
        //    {
        //        string message = NetworkMain.serverResponse.Dequeue();
        //        print(message);
        //        //            print(message);
        //        Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

        //        switch (payload["Action"])
        //        {
        //            case "Enter":
        //                hostChecker();
        //                em.spawnPlayer(payload);
        //                //Dictionary<string, string> newPayload = loadPlayer(payload["Username"], payload["UserID"]);
        //                //if (payload["UserID"] == NetworkMain.UserID) GameObject.Find(payload["Username"]).GetComponent<PlayerController>().setActivePlayer(payload["UserID"], payload["Username"], newPayload["Team"]);
        //                //else GameObject.Find(payload["Username"]).GetComponent<PlayerController>().setOtherPlayer(payload["UserID"], payload["Username"], newPayload["Team"]);
        //                break;
        //            //case "Join":
        //            //    loadPlayer(payload["Username"], payload["UserID"]);
        //            //    GameObject.Find(payload["UserID"]).GetComponent<PlayerController>().setOtherPlayer(payload["UserID"], payload["Username"], payload["Team"]);
        //            //playerUpdates.Add(payload["userID"], new Stack<string>());
        //            //                    NetworkMain.addNewUser(payload["userID"]);
        //            //                    break;
        //            case "Spot shot":
        //                GameObject.Find(payload["UserID"]).GetComponent<PlayerController>().gunHolding.GetComponent<Gun>().muzzleFlash.Play();
        //                GameObject objHit = Instantiate(Resources.Load<GameObject>("Hit Spark"),
        //                    new Vector3(float.Parse(payload["xPos"]), float.Parse(payload["yPos"]), float.Parse(payload["zPos"])),
        //                    Quaternion.LookRotation(new Vector3(float.Parse(payload["xRot"]), float.Parse(payload["yRot"]), float.Parse(payload["zRot"])))
        //                    );
        //                Destroy(objHit, 1f);
        //                break;
        //            case "Damage":
        //                GameObject.Find(payload["Target"]).GetComponent<LivingBeing>().damage(int.Parse(payload["Value"]));
        //                break;
        //            case "Spawn Minion":
        //                GameObject.Find(payload["Team"] + " Base").transform.GetChild(3).transform.GetComponent<PassiveScript>().spawnMinion(payload["Count"]);
        //                break;
        //            case "Spawn Node":
        //                em.spawnNode(payload);
        //                //GameObject newNode = Instantiate(Resources.Load<GameObject>("Node"), StringUtils.getVectorFromJson(payload, "Pos"), StringUtils.getQuaternionFromJson(payload, "Rot"));
        //                //newNode.name = "Node " + payload["Node Counter"];
        //                //newNode.transform.SetParent(GameObject.Find("Node List").transform);
        //                break;
        //            default:
        //                //print(message);
        //                GameObject.Find(payload["userID"]).GetComponent<PlayerController>().serverResponse(payload);
        //                break;
        //        }

        //    }
        //}
    }

    //public void hostChecker()
    //{
    //    int nodeCounter = Random.Range(10, 20);
    //    GameObject blueBase = GameObject.Find("Blue Base");
    //    GameObject redBase = GameObject.Find("Red Base");
    //    if (blueBase.transform.GetChild(4).childCount == redBase.transform.GetChild(4).childCount && blueBase.transform.GetChild(4).childCount == 0)
    //    {
    //        while (nodeCounter > 0)
    //        {
    //            var position = new Vector3(Random.Range(-500.0f, 500.0f), 0, Random.Range(-500.0f, 500.0f));
    //            if (!(position.x > 350f && position.z > 350f) && !(position.x < -350f && position.z < -350f) && EntityManager.GetClosestResource(position) > 15)
    //            {
    //                Dictionary<string, string> payload = StringUtils.getPositionAndRotation(position, Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0));
    //                payload["Node Counter"] = StringUtils.convertIntToString(nodeCounter);
    //                payload["Action"] = "Spawn Node";
    //                NetworkMain.broadcast(payload);
    //                nodeCounter--;
    //            }
    //        }
    //    }
    //}


}
