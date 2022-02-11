using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendButton : MonoBehaviour
{

    public Text chatMessage;
    public InputField chatField;
    public Text chatBox;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            sendMessage();
        }
    }

    public void OnClick()
    {
        sendMessage();
    }

    private void sendMessage()
    {
        if (chatMessage.text.StartsWith(">"))
        {
            string[] parsedCommand = chatMessage.text.Replace(">", "").Split(' ');
            Dictionary<string, string> payload = StringUtils.getPayload();
            switch (parsedCommand[0].ToLower())
            {
                case "list":
                    payload["Action"] = "Get Lobby List";
                    payload["Type"] = "Action";
                    NetworkMain.serverAction(payload);
                    break;
                case "create":
                    payload["Action"] = "Create Lobby";
                    payload["Name"] = chatMessage.text.Replace(">", "").Replace(parsedCommand[0] + " ", "");
                    payload["Type"] = "Action";
                    NetworkMain.serverAction(payload);
                    break;
                case "join":
                    if (!NetworkMain.LobbyID.Replace("Lobby-", "").Equals(chatMessage.text.Replace(">", "").Replace(parsedCommand[0] + " ", "")))
                    {
                        payload["Action"] = "Join Lobby";
                        payload["Name"] = chatMessage.text.Replace(">", "").Replace(parsedCommand[0] + " ", "");
                        payload["Type"] = "Action";
                        NetworkMain.serverAction(payload);
                    } else
                    {
                        chatBox.text += "You're already in that lobby.\n";
                    }
                    break;
                case "leave":
                    payload["Action"] = "Leave Lobby";
                    payload["Name"] = NetworkMain.LobbyID;
                    payload["Type"] = "Action";
                    NetworkMain.serverAction(payload);
                    break;
            }
        } else
        {
            switch (chatMessage.text.ToLower())
            {
                case "help":
                    chatBox.text += "Type the follow commands using >[Command]\n" +
                        "  >List : List all available lobbies\n" +
                        "  >Join <Lobby> : Joins a game lobby\n" +
                        "  >Create <Name> : Create a new lobby\n" +
                        "  >Leave : Leave the lobby\n";
                    break;
                default:
                    Dictionary<string, string> payload = StringUtils.getPayload();
                    payload["Message"] = chatMessage.text;
                    payload["Username"] = NetworkMain.Username;
                    payload["Action"] = "Message";
                    payload["Type"] = "Action";
                    NetworkMain.broadcastAction(payload);
                    break;
            }
        }

        //if (NetworkMain.clientType == "Client")
        //{
        //    payload["Mode"] = "Forward";
        //    payload["Action"] = "Send Message";
        //    NetworkMain.blindSend(payload);
        //}
        //else
        //{
        //    payload["Mode"] = "Action";
        //    chatBox.text += NetworkMain.Username + ": " + chatMessage.text +"\n";
        //    payload["Action"] = "Populate Message";
        //NetworkMain.broadcastClients(payload);
        //}

        //        NetworkMain.socket.Emit("Message", StringUtils.convertPayloadToJson(payload));
        chatField.text = "";
        chatField.Select();
        chatField.ActivateInputField();
    }
}
