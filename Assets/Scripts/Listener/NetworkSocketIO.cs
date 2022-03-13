using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Socket.Newtonsoft.Json;
using Socket.Newtonsoft.Json.Linq;

public class NetworkSocketIO : MonoBehaviour
{

    public static Dictionary<string, Stack<string>> playerUpdates;
    public EntityManager em;
    public bool gameBegin = false;
    public float timeChecker = 0;
    public Survivors lv_survivor;
    [SerializeField] TimeSystem currentTime;
    public PlayerCanvas lv_canvas;

    // Start is called before the first frame update
    void Start()
    {

        if (!NetworkMain.local)
        {
            playerUpdates = new Dictionary<string, Stack<string>>();
            lv_canvas.initLoadingScreen("Loading... Please Wait");
            foreach(KeyValuePair<string, PlayerLobbyStatus> it_player in LobbyListener.virusPlayers)
            {
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload.Add("Username", it_player.Value.playerName.text);
                payload.Add("UserID", it_player.Key);
                payload.Add("Team", it_player.Value.team);
                payload.Add("health", "100");
                em.spawnPlayer(payload);
            }
            foreach (KeyValuePair<string, PlayerLobbyStatus> it_player in LobbyListener.survivorPlayers)
            {
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload.Add("Username", it_player.Value.playerName.text);
                payload.Add("UserID", it_player.Key);
                payload.Add("Team", it_player.Value.team);
                payload.Add("health", "100");
                em.spawnPlayer(payload);
            }

            Dictionary<string, string> localPlayer = StringUtils.getPayload();
            localPlayer["lobbyID"] = NetworkMain.LobbyID;
            localPlayer["Username"] = NetworkMain.Username;
            localPlayer["Team"] = NetworkMain.Team;
            localPlayer["Action"] = "Enter Game";
            NetworkMain.serverAction(localPlayer);
        }
    }

    void OnApplicationQuit()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Type"] = "Player Action";
        payload["Action"] = "Exit Game Session";
        NetworkMain.broadcastAction(payload);
        NetworkMain.disconnect();
    }

    // Update is called once per frame
    void Update()
    {
        if (!NetworkMain.local)
        {
            if (EntityManager.survivors.Count == 0 && lv_canvas.countDownTimer == 10 && gameBegin)
            {
//                lv_canvas.initLoadingScreen("Virus has eliminated all players. Virus Wins");
//                lv_canvas.gameOver();
            }
            if (NetworkMain.serverResponse.Count > 0)
            {
                Payload getPayload = NetworkMain.serverResponse.Dequeue();
                if (getPayload.data.TryGetValue("Action", out string out_action))
                {
                    
                    switch (out_action)
                    {
                        case "Player HUD Menu":
                            EntityManager.players[getPayload.source].listen(getPayload);
                            break;
                        case "Disinfect Ship":
                            Debug.Log("disinfecting");
                            disinfectShip();
                            break;
                        case "Spawn Resource":
                            ResourceEntity lv_tmp_resource = new ResourceEntity(float.Parse(getPayload.data["xPos"]), float.Parse(getPayload.data["yPos"]), getPayload.data["Resource"], getPayload.data["UID"]);
                            //                            em.loadResources(lv_tmp_resource);
                            EntityManager.resourcesLoad.Add(getPayload.data["UID"], lv_tmp_resource);
//                            Debug.Log(getPayload.data["UID"]);
                            //                    Debug.Log($"xPos: {lv_tmp_resource.xPos} yPos: {lv_tmp_resource.yPos} UID: {lv_tmp_resource.UID}");
                            break;
                        case "Resource Loaded":
                            em.loadResources();
                            lv_canvas.deinitLoadingScreen();
                            gameBegin = true;
                            break;
                        case "Update Infection Monitor":
                            EntityManager.infectionMonitor.text = getPayload.data["Message"];
                            break;
                        case "Pickup Item":
                            EntityManager.loot.TryGetValue(getPayload.data["UID"], out Data lv_item);
                            if (lv_item != null)
                            {
                                if (EntityManager.players[getPayload.source].pickupItem(lv_item.resourceName))
                                    EntityManager.loot[getPayload.data["UID"]].pickupItem();                            
                            }
                            break;
                        case "Damage Resource":
                            if (EntityManager.resources.ContainsKey(getPayload.data["UID"]))
                            {
                                EntityManager.resources[getPayload.data["UID"]].damage(getPayload.source, float.Parse(getPayload.data["Damage"]));
                            }
                            break;
                        case "Join Acknowledge":
                            if (!EntityManager.players.ContainsKey(getPayload.source))
                            {
 //                               Debug.Log("Does not exists");
                                em.spawnPlayer(getPayload.data);
                            }
                            break;
                        case "Join Game":
//                            Debug.Log(getPayload.source + " is joining");
                            if (!EntityManager.players.ContainsKey(getPayload.source))
                            {
                                em.spawnPlayer(getPayload.data);
                                Dictionary<string, string> payload = new Dictionary<string, string>();
                                payload.Add("Username", NetworkMain.Username);
                                payload.Add("UserID", NetworkMain.UserID);
                                payload.Add("Team", NetworkMain.Team);
                                payload.Add("health", EntityManager.players[NetworkMain.UserID].getHealth().ToString());
                                payload.Add("Action", "Join Acknowledge");
                                NetworkMain.broadcastAction(payload, getPayload.source);
                            } else
                            {
                                Debug.Log("Exists");
                            }
                            break;
                    }
                }
            }
        }
    }

    private void disinfectShip()
    {
        lv_survivor.disinfectShip();
    }
}