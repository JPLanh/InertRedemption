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
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("LobbyID", NetworkMain.LobbyID);
        payload.Add("Action", "Ready");
        NetworkMain.broadcastAction(payload);
//        SceneManager.LoadScene("mainScene");
//        NetworkMain.broadcastClients(payload);
        //NetworkMain.socket.Emit("Lobby", StringUtils.convertPayloadToJson(payload));
    }

}
