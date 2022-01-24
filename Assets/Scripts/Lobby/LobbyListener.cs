using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Socket.Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class LobbyListener : MonoBehaviour
{
    public Text userlistField;
    public Text chatField;
    public InputField messageField;

    //void OnDestroy()
    //{
    //    NetworkMain.disconnect();
    //}

    // Start is called before the first frame update
    void Start()
    {
        messageField.Select();

        //if (NetworkMain.clientType == "Server")
        //{
        //    userlistField.text = NetworkMain.Username;
        //} else
        //{
        //    Dictionary<string, string> payload = new Dictionary<string, string>();
        //    payload["Action"] = "Get Userlist";
        //    payload["Mode"] = "Action";
        //    NetworkMain.sendString(payload);
        //}

        Dictionary<string, string> payload = StringUtils.getPayload();
        payload["Action"] = "Refresh Userlist";
        messageField.Select();
        NetworkMain.socket.Emit("Lobby", StringUtils.convertPayloadToJson(payload));
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkMain.clientType == "Client")
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["Username"] = NetworkMain.Username;
            payload["Action"] = "Update";
//            NetworkMain.sendString(payload);
        }
        //if (NetworkMain.updateResponse.Count > 0)
        //{
        //    string message = NetworkMain.updateResponse.Pop();
        //    List<Dictionary<string, string>> payload = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(message);
        //    string tmpField = "";
        //    foreach (Dictionary<string, string> getSubPayload in payload)
        //    {
        //        tmpField += getSubPayload["Username"] + "\n";
        //    }
        //    userlistField.text = tmpField;
        //}

        if (NetworkMain.serverResponse.Count > 0)
        {
            //string message = NetworkMain.serverRecResponse.Dequeue();
            //print(message);
            //Dictionary<string, string> payload = StringUtils.parsePayload(message);
            Dictionary<string, string> getPayload = NetworkMain.serverRecResponse.Dequeue();
            Dictionary<string, string> payload = new Dictionary<string, string>();
            switch (getPayload["Action"])
            {
                case "Join":
                    print("Someone wanna join");
                    payload["Action"] = "Welcome";
                    userlistField.text += "\n" + getPayload["Username"];
                    NetworkMain.sendReply(payload, getPayload["ipAddress"], getPayload["Port"]);
                    NetworkMain.listOfClients[getPayload["Username"]] = new ClientsInfo(getPayload["ipAddress"], getPayload["Port"]);
                    break;
                case "Send Message":
                    chatField.text += getPayload["Username"] + ": " + getPayload["Message"] + "\n";
                    //payload["Message"] = getPayload["Message"];
                    //payload["Username"] = getPayload["Username"];
                    //payload["Action"] = "Populate Message";
//                  NetworkMain.broadcastClients(payload);
                    break;
                case "Begin Game":
                    SceneManager.LoadScene("mainScene");
                    break;
                case "Get Userlist":
                    payload["Action"] = "Refresh Userlist";
                    payload["Mode"] = "Action";
                    payload["Userlist"] = userlistField.text;
                    NetworkMain.sendReply(payload, getPayload["ipAddress"], getPayload["Port"]);
                    break;
                case "Refresh Userlist":
                    userlistField.text = getPayload["Userlist"];
                    break;
                //case "Populate Message":
                //    chatField.text += getPayload["Username"] + ": " + getPayload["Message"] + "\n";
                //    break;
                case "Update":
                    chatField.text += getPayload["Username"] + ": " + Time.time + " got update\n";
                    payload["Action"] = "Get Update";
                    payload["Message"] = Time.time + " got update";
                    payload["Username"] = payload["Username"];
                    NetworkMain.sendReply(payload, getPayload["ipAddress"], getPayload["Port"]);
                    break;
                case "Get Update":
                    chatField.text += getPayload["Username"] + ": " + Time.time + " got update\n";
                    break;
                case "Exit":
                    userlistField.text.Replace("\n" + getPayload["Username"], "");
                    payload["Action"] = "Refresh Userlist";
                    payload["Userlist"] = userlistField.text;
                    payload["Mode"] = "Action";
                    NetworkMain.broadcastClients(payload);
                    break;
            }
        }
    }
}
