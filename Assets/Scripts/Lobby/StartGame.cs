using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnClick()
    {
        Dictionary<string, string> payload = StringUtils.getPayload();
        payload["Action"] = "Begin Game";
        payload["Mode"] = "Action";
        SceneManager.LoadScene("mainScene");
        NetworkMain.broadcastClients(payload);
        //NetworkMain.socket.Emit("Lobby", StringUtils.convertPayloadToJson(payload));
    }

}
