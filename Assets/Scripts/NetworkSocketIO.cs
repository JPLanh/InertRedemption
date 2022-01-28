using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Socket.Newtonsoft.Json;
using Socket.Newtonsoft.Json.Linq;

public class NetworkSocketIO : MonoBehaviour
{

    public static Dictionary<string, Stack<string>> playerUpdates;
    public EntityManager em;
    public bool ready = false;
    public float timeChecker = 0;
    public Survivors lv_survivor;
    [SerializeField]
    TimeSystem currentTime;
    // Start is called before the first frame update
    void Start()
    {

        if (!NetworkMain.local)
        {
            playerUpdates = new Dictionary<string, Stack<string>>();
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
        {
            if (NetworkMain.serverResponse.Count > 0)
            {
                Payload getPayload = NetworkMain.serverResponse.Dequeue();
                if (getPayload.data.TryGetValue("Action", out string out_action))
                {

                    switch (out_action)
                    {
                        case "Join Game":
                            Debug.Log(getPayload.source + " is joining");
                            if (!EntityManager.players.ContainsKey(getPayload.source))
                            {
                                Debug.Log("Does not exists");
                                em.spawnPlayer(getPayload.data);
                                NetworkMain.isPlaying = true;
                                //NetworkMain.LobbyID = getPayload["lobbyID"];
                                //NetworkMain.UserID = getPayload["UserID"];
                                //currentTime.setTime(StringUtils.convertToFloat(getPayload["Time"]));
                                //em.resourceCounter = StringUtils.convertToInt(getPayload["resourceLimit"]);
                                //payload["lobbyID"] = NetworkMain.LobbyID;
                                //payload["Action"] = "Server Update";
                                //NetworkMain.getUpdates(payload);
                                    Dictionary<string, string> payload = new Dictionary<string, string>();
                                    payload.Add("Username", NetworkMain.Username);
                                    payload.Add("UserID", NetworkMain.UserID);
                                    payload.Add("Team", NetworkMain.Team);
                                    payload.Add("health", EntityManager.players[NetworkMain.Username].getHealth().ToString());
                                    payload.Add("Action", "Join Game");
                                    NetworkMain.broadcastAction(payload, getPayload.source);
                                    Debug.Log("Synchronizing");
                            } else
                            {
                                Debug.Log("Exists");
                            }
                            break;
                    }
                }

//                Dictionary<string, string> payload = new Dictionary<string, string>();
//                if (getPayload != null)
//                {
////                    Debug.Log(StringUtils.convertPayloadToJson(getPayload));
//                    if (getPayload.TryGetValue("name", out var username))
//                    {
//                        //                        GameObject findGO = GameObject.Find(username);
////                        EntityManager.survivors.TryGetValue(getPayload["name"], out PlayerController getPlayer);
//                        EntityManager.players.TryGetValue(getPayload["name"], out IPlayerController getExistance);

//                        string[] parsedAction = getPayload["Action"].Split(' ');
//                        switch (parsedAction[0])
//                        {
//                            case "Update":
//                                if (getPayload["name"] != NetworkMain.Username)
//                                {
//                                    if (getExistance == null)
//                                    {
//                                        if (getPayload["State"] == "Alive")
//                                        {
//                                            em.spawnPlayer(getPayload);
//                                        }
//                                    }
//                                    else
//                                    {
//                                        if (getPayload["State"] == "Alive")
//                                            getExistance.serverControl(getPayload);
//                                        else if (getPayload["State"] == "Dead")
//                                            Destroy(getExistance.getGameObject());
//                                    }
//                                }
//                                else
//                                {
//                                    if (getExistance != null)
//                                    {
//                                        if (!getExistance.isMovable())
//                                        {
//                                            getExistance.serverControl(getPayload);
//                                        }
//                                        if (NetworkMain.isHost != bool.Parse(getPayload["host"]))
//                                        {
//                                            NetworkMain.isHost = bool.Parse(getPayload["host"]);
//                                            if (bool.Parse(getPayload["host"]))
//                                            {
//                                                em.newHost();
//                                            }
//                                        }
//                                    }
//                                }
//                                break;
//                            case "Infect":
//                                foreach(KeyValuePair<string, VirusController> it_virus in EntityManager.virus)
//                                {
//                                    //                                    it_virus.Value.infe
//                                    getExistance.getInfectionScript().infect(it_virus.Value);
//                                }
//                                break;
//                            case "Spaceship":
//                                lv_survivor.spaceship.addResource(parsedAction[3], int.Parse(parsedAction[2]));
//                                break;
//                            default:
//                                switch (getPayload["Action"])
//                                {
//                                    case "Join Game":
//                                        PlayerController playerSpawn = em.spawnPlayer(getPayload);
//                                        //NetworkMain.LobbyID = getPayload["lobbyID"];
//                                        //NetworkMain.UserID = getPayload["UserID"];
//                                        currentTime.setTime(StringUtils.convertToFloat(getPayload["Time"]));
//                                        em.resourceCounter = StringUtils.convertToInt(getPayload["resourceLimit"]);
//                                        //payload["lobbyID"] = NetworkMain.LobbyID;
//                                        //payload["Action"] = "Server Update";
//                                        //NetworkMain.getUpdates(payload);
//                                        break;
//                                    case "Fire1":
//                                        getExistance.fireOne();
//                                        break;
//                                    case "Fire2":
//                                        getExistance.fireTwo();
//                                        //                                findGO.GetComponent<PlayerController>().aim(false);
//                                        break;
//                                    case "Fire1Up":
//                                        getExistance.setSingleHandUse(false);
//                                        break;
//                                    case "Reload":
//                                        getExistance.reload(false);
//                                        break;
//                                    case "Swap holding":
//                                        getExistance.swapGun(false);
//                                        break;
//                                    case "Swap Gun":
//                                        getExistance.swapGun(false);
//                                        break;
//                                    case "Menu":
//                                        getExistance.accessMenu(false);
//                                        break;
//                                    case "Interact":
//                                        getExistance.Interact();
//                                        break;
//                                    case "Build":
//                                        getExistance.buildModeSwitch();
//                                        break;
//                                    case "Spawn Resource":
//                                        em.createResource(getPayload);
//                                        break;
//                                    case "Flash Light":
//                                        getExistance.toggleFlashLight();
//                                        break;
//                                    case "Skill 1":
//                                    case "Skill 2":
//                                    case "Skill 3":
//                                    case "Skill 4":
//                                    case "Skill 5":
//                                        getExistance.useAbility(int.Parse(parsedAction[1]));
//                                        break;
//                                    case "Debug Time":
//                                        float timeCalc = (StringUtils.convertToFloat(getPayload["Time"]) % 600) / 600;
//                                        print("Server: " + StringUtils.convertFloatToString(timeCalc));
//                                        print("Client: " + currentTime.time);
//                                        print("Difference: " + (currentTime.time - timeCalc));
//                                        break;
//                                    //case "Pull":
//                                    //    if (NetworkMain.Username == getPayload["Username"]) getExistance.canMove = false;
//                                    //    break;
//                                    //case "Pull Finish":
//                                    //    if (NetworkMain.Username == getPayload["Username"])
//                                    //        getExistance.canMove = true;
//                                    //    break;
//                                }
//                                break;

//                        }


                    //}
                    //else if (getPayload.TryGetValue("UID", out var getResource))
                    //{
                    //    switch (getPayload["Action"])
                    //    {
                    //        case "Spawn Item":
                    //            em.spawnItem(getPayload);
                    //            break;
                    //        case "Spawn Resource":
                    //            em.createResource(getPayload);
                    //            break;
                    //        case "Update Resource":
                    //            GameObject findGO = GameObject.Find(getResource);
                    //            findGO.GetComponent<Resource>().damage(false, StringUtils.convertToFloat(getPayload["durability"]), null);
                    //            break;
                    //    }
                    //}
                }

            }

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
                    //GameObject getPlayer = GameObject.Find(payload["name"]);
                    Debug.Log(StringUtils.convertPayloadToJson(payload));
                    EntityManager.players.TryGetValue(payload["name"], out IPlayerController getPlayer);
                    //if (payload["name"] != NetworkMain.Username)
                    //{
                    //    if (getPlayer == null)
                    //    {
                    //        if (payload["State"] == "Alive")
                    //        {
                    //            em.spawnPlayer(payload);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (payload["State"] == "Alive")
                    //            getPlayer.serverControl(payload);
                    //        else if (payload["State"] == "Dead")
                    //            Destroy(getPlayer.getGameObject());
                    //    }
                    //}
                    //else
                    //{
                    //    if (getPlayer != null)
                    //    {
                    //        if (!getPlayer.isMovable())
                    //        {
                    //            getPlayer.serverControl(payload);
                    //        }
                    //        if (NetworkMain.isHost != bool.Parse(payload["host"]))
                    //        {
                    //            NetworkMain.isHost = bool.Parse(payload["host"]);
                    //            if (bool.Parse(payload["host"]))
                    //            {
                    //                em.newHost();
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
        }
    }