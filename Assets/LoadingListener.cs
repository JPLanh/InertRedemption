using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingListener : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Dictionary<string, string> localPlayer = StringUtils.getPayload();
        localPlayer["lobbyID"] = NetworkMain.LobbyID;
        localPlayer["Action"] = "Begin";
        NetworkMain.serverAction(localPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkMain.serverResponse.Count > 0)
        {
            Payload getPayload = NetworkMain.serverResponse.Dequeue();
//            Debug.Log(getPayload.data["Action"]);
            switch (getPayload.data["Action"])
            {
                case "Spawn Resource":
                    ResourceEntity lv_tmp_resource = new ResourceEntity(float.Parse(getPayload.data["xPos"]), float.Parse(getPayload.data["yPos"]), getPayload.data["Type"], getPayload.data["UID"]);
                    EntityManager.resourcesLoad.Add(getPayload.data["UID"], lv_tmp_resource);
//                    Debug.Log($"xPos: {lv_tmp_resource.xPos} yPos: {lv_tmp_resource.yPos} UID: {lv_tmp_resource.UID}");
                    break;
                case "Resource Loaded":
                    SceneManager.LoadScene("mainScene");
                    break;
            }
        }
    }
    void OnApplicationQuit()
    {
            NetworkMain.broadcastAction("Exit");
            NetworkMain.socket.Disconnect();
    }
}
