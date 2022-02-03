using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Socket.Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class LobbyListener : MonoBehaviour
{
    public Transform survivorList;
    public Transform virusList;
    public Text chatField;
    public InputField messageField;
    public static Dictionary<string, PlayerLobbyStatus> survivorPlayers = new Dictionary<string, PlayerLobbyStatus>();
    public static Dictionary<string, PlayerLobbyStatus> virusPlayers = new Dictionary<string, PlayerLobbyStatus>();
    private int countDown = 5;
    IEnumerator countDownTimer;

    //void OnDestroy()
    //{
    //    NetworkMain.disconnect();
    //}

    // Start is called before the first frame update
    void Start()
    {
        messageField.Select();
        countDownTimer = countdown();
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Username", NetworkMain.Username);
        payload.Add("UserID", NetworkMain.UserID);
        payload.Add("Team", "Survivor");
        payload.Add("Action", "Get Lobby Users");
        NetworkMain.broadcastAction(payload);

        addNewPlayer(NetworkMain.UserID, NetworkMain.Username, "Survivor");
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkMain.serverResponse.Count > 0)
        {
            Payload getPayload = NetworkMain.serverResponse.Dequeue();
            Debug.Log(getPayload.data["Action"]);
            switch (getPayload.data["Action"])
            {
                case "Exit":
                    removePlayer(getPayload.source);
                    managePlayerList();
                    break;
                case "Get Lobby Users":
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
            }
        }
    }

    public void addNewPlayer(string in_UID, string in_username, string in_team)
    {

        GameObject lv_player = Instantiate(Resources.Load<GameObject>("Player Lobby Status"));
        lv_player.TryGetComponent<PlayerLobbyStatus>(out PlayerLobbyStatus lv_playerLobbyStatus);
        lv_playerLobbyStatus.init(in_username, in_team);
        switch (in_team)
        {
            case "Survivor":
                lv_player.transform.SetParent(survivorList);
                lv_player.transform.localPosition = new Vector3(-155f, 230f - (survivorPlayers.Count * 35f), 0f);
                survivorPlayers.Add(in_UID, lv_playerLobbyStatus);
                break;
            case "Virus":
                lv_player.transform.SetParent(virusList);
                lv_player.transform.localPosition = new Vector3(-155f, 230f - (virusPlayers.Count * 35f), 0f);
                virusPlayers.Add(in_UID, lv_playerLobbyStatus);
                break;
        }
        if (NetworkMain.UserID.Equals(in_UID))
        {
            NetworkMain.Team = in_team;
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
        foreach(KeyValuePair<string, PlayerLobbyStatus> it_player in survivorPlayers)
        {
            it_player.Value.transform.localPosition = new Vector3(-35f, 275f - (counter * 35f), 0f);
            counter += 1;
        }
        counter = 0;
        foreach (KeyValuePair<string, PlayerLobbyStatus> it_player in virusPlayers)
        {
            it_player.Value.transform.localPosition = new Vector3(-35f, 275f - (counter * 35f), 0f);
            counter += 1;
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
            if (countDown == 0) SceneManager.LoadScene("Loading");
            chatField.text += $"Game is starting in {countDown}...\n";
            countDown -= 1;
            yield return new WaitForSeconds(1);
        }
    }
}