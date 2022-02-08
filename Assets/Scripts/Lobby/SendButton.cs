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
        Dictionary<string, string> payload = StringUtils.getPayload();
        payload["Message"] = chatMessage.text;
        payload["Username"] = NetworkMain.Username;
        payload["Action"] = "Message";
        payload["Type"] = "Action";
        NetworkMain.broadcastAction(payload);
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
