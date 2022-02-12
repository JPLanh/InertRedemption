using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Socket.Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class LobbyListener : MonoBehaviour
{
    public GameObject survivorTab;
    public Transform survivorList;
    public GameObject virusTab;
    public Transform virusList;
    public GameObject mainLobbyTab;
    public Transform mainLobbyList;
    public Text chatField;
    public GameObject buttonList;
    public InputField messageField;
    public ToastNotifications lv_toast;
    public static Dictionary<string, PlayerLobbyStatus> survivorPlayers = new Dictionary<string, PlayerLobbyStatus>();
    public static Dictionary<string, PlayerLobbyStatus> virusPlayers = new Dictionary<string, PlayerLobbyStatus>();
    public static Dictionary<string, PlayerLobbyStatus> allPlayers = new Dictionary<string, PlayerLobbyStatus>();
    private int countDown = 5;
    IEnumerator countDownTimer;

    //void OnDestroy()
    //{
    //    NetworkMain.disconnect();
    //}

    // Start is called before the first frame update
    void Start()
    {
        NetworkMain.isBroadcastable = true;
        messageField.Select();
        countDownTimer = countdown();

//        addNewPlayer(NetworkMain.UserID, NetworkMain.Username, "Main Lobby");
        updateLobbyFilter();
    }

    void OnApplicationQuit()
    {
        leaveLobby();
    }

    private void updateLobbyFilter()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Username", NetworkMain.Username);
        payload.Add("UserID", NetworkMain.UserID);
        payload.Add("Type", "Action");

        switch (NetworkMain.LobbyID)
        {
            case "Lobby-Main":
                survivorPlayers.Clear();
                virusPlayers.Clear();
                buttonList.SetActive(false);
                survivorList.parent.gameObject.SetActive(false);
                foreach(Transform it_child in survivorList)
                {
                    Destroy(it_child.gameObject);
                }
                virusList.parent.gameObject.SetActive(false);
                foreach (Transform it_child in virusList)
                {
                    Destroy(it_child.gameObject);
                }
                payload.Add("Team", "Main Lobby");
                payload.Add("Action", "Get Main Lobby Users");
                addNewPlayer(NetworkMain.UserID, NetworkMain.Username, "Main Lobby");
                mainLobbyList.parent.gameObject.SetActive(true);
                break;
            default:
                allPlayers.Clear();
                buttonList.SetActive(true);
                survivorList.parent.gameObject.SetActive(true);
                virusList.parent.gameObject.SetActive(true);
                mainLobbyList.parent.gameObject.SetActive(false);
                foreach (Transform it_child in mainLobbyList)
                {
                    Destroy(it_child.gameObject);
                }
                allPlayers.Clear();
                addNewPlayer(NetworkMain.UserID, NetworkMain.Username, "Survivor");
                payload.Add("Team", NetworkMain.Team);
                payload.Add("Action", "Get Lobby Users");
                break;
        }
        NetworkMain.broadcastAction(payload);
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkMain.serverResponse.Count > 0)
        {
            Payload getPayload = NetworkMain.serverResponse.Dequeue();
            Dictionary<string, string> payload = new Dictionary<string, string>();

            switch (getPayload.data["Action"])
            {
                case "Get All Lobbies":
                    List<string> lv_room_list = new List<string>();
                    foreach(KeyValuePair<string, string> it_room in getPayload.data)
                    {
                        if (it_room.Key.Contains("Lobby"))
                        {
                            lv_room_list.Add(it_room.Key.Replace("Lobby-", ""));
                        }
                    }
                    chatField.text += "Available Rooms: " + string.Join(", ", lv_room_list.ToArray()) + "\n";
                    break;
                case "Exit":
                    removePlayer(getPayload.source);
                    managePlayerList();
                    break;
                case "Get Main Lobby Users":
                    if (!getPayload.source.Equals(NetworkMain.UserID))
                    {
//                        Debug.Log("Adding: " + getPayload.data["UserID"]);
                        payload.Add("Username", NetworkMain.Username);
                        payload.Add("UserID", NetworkMain.UserID);
                        addNewPlayer(getPayload.data["UserID"], getPayload.data["Username"], "Main Lobby");
                        payload.Add("Action", "Main Lobby");
                        NetworkMain.replyAction(payload, getPayload.data["UserID"]);
//                        StartCoroutine(refreshList());
                        //                        serverResponse.Enqueue(lv_payload);
                    }
                    break;

                case "Get Lobby Users":
                    if (!getPayload.source.Equals(NetworkMain.UserID))
                    {
                        payload.Add("Username", NetworkMain.Username);
                        payload.Add("UserID", NetworkMain.UserID);
                        payload.Add("Team", NetworkMain.Team);
                        addNewPlayer(getPayload.data["UserID"], getPayload.data["Username"], getPayload.data["Team"]);
                        payload.Add("Action", "In Lobby");
                        NetworkMain.replyAction(payload, getPayload.data["UserID"]);
  //                      StartCoroutine(refreshList());
                        //                        serverResponse.Enqueue(lv_payload);
                    }
                    break;
                case "Leave Lobby User":
                    if (!getPayload.source.Equals(NetworkMain.UserID))
                    {
//                        Debug.Log($"{getPayload.source} wants to leave {NetworkMain.LobbyID} in team {getPayload.data["Team"]}");
                        switch (NetworkMain.LobbyID)
                        {
                            case "Lobby-Main":
                                Destroy(allPlayers[getPayload.source].gameObject);
                                allPlayers.Remove(getPayload.source);
                                break;
                            default:
                                switch (getPayload.data["Team"])
                                {
                                    case "Survivor":
                                        Destroy(survivorPlayers[getPayload.source].gameObject);
                                        survivorPlayers.Remove(getPayload.source);
                                        break;
                                    case "Virus":
                                        Destroy(virusPlayers[getPayload.source].gameObject);
                                        virusPlayers.Remove(getPayload.source);
                                        break;
                                }
                                //Destroy(allPlayers[getPayload.source].gameObject);
                                //allPlayers.Remove(getPayload.source);
                                break;

                        }
    //                    StartCoroutine(refreshList());
                    }
                    break;
                case "Main Lobby":
                    addNewPlayer(getPayload.data["UserID"], getPayload.data["Username"], "Main Lobby");
                    managePlayerList();
                    break;
                case "In Lobby":
                    addNewPlayer(getPayload.data["UserID"], getPayload.data["Username"], getPayload.data["Team"]);
                    managePlayerList();
                    break;
                case "Message":
                    chatField.text += getPayload.data["Username"] + ": " + getPayload.data["Message"] + "\n";
                    break;
                case "Ready":
                    toggleReadyCheck(getPayload.source);
                    readyCheck();
                    break;
                case "Swap Team":
                    swapTeam(getPayload.source);
                    break;
                case "Denied Lobby Creation":
                    lv_toast.newNotification(getPayload.data["Reason"]);
                    break;
                case "Lobby Created":
                    changeLobby(getPayload.data["LobbyID"]);
                    //NetworkMain.LobbyID = getPayload.data["LobbyID"];
                    //chatField.text += $"Now Creating {getPayload.data["LobbyID"].Replace("Lobby-", "")}...\n";
                    //enterLobby();
                    //updateLobbyFilter();
                    break;
                case "Lobby Left":
                    changeLobby("Lobby-Main");
                    //NetworkMain.LobbyID = "Lobby-Main";
                    //chatField.text += $"Now leaving the Main lobby...\n";
                    //enterLobby();
                    //updateLobbyFilter();
                    break;
                case "Denied Lobby Join":
                    lv_toast.newNotification(getPayload.data["Reason"]);
                    break;
                case "Lobby Join":
                    changeLobby(getPayload.data["LobbyID"]);
                    //NetworkMain.LobbyID = getPayload.data["LobbyID"];
                    //chatField.text += $"Now Joining {getPayload.data["LobbyID"].Replace("Lobby-", "")}...\n";
                    //enterLobby();
                    //updateLobbyFilter();
                    break;
            }
        }
    }


    //IEnumerator refreshList()
    //{
    //    switch (NetworkMain.LobbyID)
    //    {
    //        case "Lobby-Main":
    //            int counter = 0;
    //            foreach (KeyValuePair<string, PlayerLobbyStatus> it_player in allPlayers)
    //            {
    //                it_player.Value.transform
    //            }
    //            break;
    //        default:
    //            foreach (Transform it_child in survivorList)
    //            {
    //                Destroy(it_child.gameObject);
    //            }

    //            foreach (Transform it_child in virusList)
    //            {
    //                Destroy(it_child.gameObject);
    //            }
    //            while (survivorList.childCount != 0 && virusList.childCount != 0)
    //            {
    //                yield return null;
    //            }

    //            foreach (KeyValuePair<string, PlayerLobbyStatus> it_player in survivorList)
    //            {
    //                addNewPlayer(it_player.Value);
    //            }
    //            foreach (KeyValuePair<string, PlayerLobbyStatus> it_player in virusList)
    //            {
    //                addNewPlayer(it_player.Value);
    //            }
    //            break;
    //    }
    //}

    private void changeLobby(string in_lobby_name)
    {
        leaveLobby();
        switch (NetworkMain.LobbyID)
        {
            case "Lobby-Main":
                NetworkMain.Team = "Main Lobby";
                break;
            default:
                NetworkMain.Team = "Survivor";
                break;
        }
        NetworkMain.LobbyID = in_lobby_name;
        chatField.text += $"Now entering {in_lobby_name.Replace("Lobby-", "")}...\n";
        updateLobbyFilter();
    }

    private void leaveLobby()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Username", NetworkMain.Username);
        payload.Add("UserID", NetworkMain.UserID);
        payload.Add("Team", NetworkMain.Team);
        payload.Add("Type", "Action");
        payload.Add("Action", "Leave Lobby User");
        NetworkMain.broadcastAction(payload);
    }

    public void addNewPlayer(string in_UID, string in_username, string in_team)
    {

        GameObject lv_player = Instantiate(Resources.Load<GameObject>("Player Lobby Status"));
        lv_player.TryGetComponent<PlayerLobbyStatus>(out PlayerLobbyStatus lv_playerLobbyStatus);
        lv_playerLobbyStatus.init(in_username, in_team);
        switch (in_team)
        {
            case "Main Lobby":
                lv_player.transform.SetParent(mainLobbyList);
                lv_player.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f - ((mainLobbyList.childCount-1) * 35f), 0f);
                allPlayers.Add(in_UID, lv_playerLobbyStatus);
                break;
            case "Survivor":
                lv_player.transform.SetParent(survivorList);
                lv_player.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f - ((survivorList.childCount-1) * 35f), 0f);
                survivorPlayers.Add(in_UID, lv_playerLobbyStatus);
                break;
            case "Virus":
                lv_player.transform.SetParent(virusList);
                lv_player.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f - ((virusList.childCount-1) * 35f), 0f);
                virusPlayers.Add(in_UID, lv_playerLobbyStatus);
                break;
        }
        if (NetworkMain.UserID.Equals(in_UID))
        {
            NetworkMain.Team = in_team;
        }
    }

    private void addNewPlayer(PlayerLobbyStatus in_player)
    {
        GameObject lv_player = Instantiate(Resources.Load<GameObject>("Player Lobby Status"));
        lv_player.TryGetComponent<PlayerLobbyStatus>(out PlayerLobbyStatus lv_playerLobbyStatus);
        Debug.Log(in_player.team);
        lv_playerLobbyStatus.init(in_player);
        switch (in_player.team)
        {
            case "Main Lobby":
                lv_player.transform.SetParent(mainLobbyList);
                lv_player.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f - (allPlayers.Count * 35f), 0f);
                break;
            case "Survivor":
                lv_player.transform.SetParent(survivorList);
                lv_player.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f - (survivorPlayers.Count * 35f), 0f);
                break;
            case "Virus":
                lv_player.transform.SetParent(virusList);
                lv_player.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f - (virusPlayers.Count * 35f), 0f);
                break;
        }
    }

    public void swapTeam(string in_UID)
    {
        string lv_name;
        if (survivorPlayers.ContainsKey(in_UID))
        {
            lv_name = survivorPlayers[in_UID].playerName.text;
            Destroy(survivorPlayers[in_UID].gameObject);
            survivorPlayers.Remove(in_UID);
            addNewPlayer(in_UID, lv_name, "Virus");
        }
        else if (virusPlayers.ContainsKey(in_UID))
        {
            lv_name = virusPlayers[in_UID].playerName.text;
            Destroy(virusPlayers[in_UID].gameObject);
            virusPlayers.Remove(in_UID);
            addNewPlayer(in_UID, lv_name, "Survivor");
        }

    }

    public void removePlayer(string in_UID)
    {
        if (survivorPlayers.ContainsKey(in_UID))
        {
            Destroy(survivorPlayers[in_UID].gameObject);
            survivorPlayers.Remove(in_UID);
        } else if (virusPlayers.ContainsKey(in_UID))
        {
            Destroy(virusPlayers[in_UID].gameObject);
            virusPlayers.Remove(in_UID);
        }
    }

    public void toggleReadyCheck(string in_UID)
    {
        if (survivorPlayers.ContainsKey(in_UID))
        {
            survivorPlayers[in_UID].readyCheckToggle();
        }
        else if (virusPlayers.ContainsKey(in_UID))
        {
            virusPlayers[in_UID].readyCheckToggle();
        }
    }
      
    public void managePlayerList()
    {

        int counter = 0;
        if (NetworkMain.LobbyID.Equals("Lobby-Main"))
        {
            counter = 0;
            foreach (KeyValuePair<string, PlayerLobbyStatus> it_player in allPlayers)
            {
                it_player.Value.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f - (counter * 35f), 0f);
                counter += 1;
            }
        } else
        {
            foreach (KeyValuePair<string, PlayerLobbyStatus> it_player in survivorPlayers)
            {
                it_player.Value.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f - (counter * 35f), 0f);
                counter += 1;
            }
            counter = 0;
            foreach (KeyValuePair<string, PlayerLobbyStatus> it_player in virusPlayers)
            {
                it_player.Value.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f - (counter * 35f), 0f);
                counter += 1;
            }
        }
    }

    public void readyCheck()
    {
        bool isReady = true;
        foreach (KeyValuePair<string, PlayerLobbyStatus> it_player in survivorPlayers)
        {
            isReady = (isReady && it_player.Value.ready);
        }
        foreach (KeyValuePair<string, PlayerLobbyStatus> it_player in virusPlayers)
        {
            isReady = (isReady && it_player.Value.ready);
        }
        if (isReady)
            StartCoroutine(countDownTimer);
        else
        {
            countDown = 5;
            StopCoroutine(countDownTimer);
        }
    }

    IEnumerator countdown()
    {
        while (true)
        {
            if (countDown == 0)
            {
                SceneManager.LoadScene("mainScene");
                NetworkMain.isBroadcastable = false;
            }
            chatField.text += $"Game is starting in {countDown}...\n";
            countDown -= 1;
            yield return new WaitForSeconds(1);
        }
    }
}