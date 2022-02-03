using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginListener : MonoBehaviour
{
    public Text username;
    public InputField getName;
    public ToastNotifications toast;
    [SerializeField]
    private Text IP;
    public LoginScripts loginScript;
    // Start is called before the first frame update

    void Start()
    {
        //        getName.Select();
    }

    // Update is called once per frame
    void Update()
    {
        listenHandler();
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    login();
        //}
    }

//    public void login()
//    {
//        NetworkMain.serverIP = IP.text;
//        NetworkMain.senderInit();
//        Dictionary<string, string> payload = new Dictionary<string, string>();
//        payload["Action"] = "Join";
//        NetworkMain.sendString(payload);
////        NetworkMain.joinGame(username.text, "123");
//    }

    private void listenHandler()
    {
        if (NetworkMain.mainMenuResponse.Count > 0)
        {
            {
                Dictionary<string, string> payload = NetworkMain.mainMenuResponse.Dequeue();
                switch (payload["Action"])
                {
                    case "Enter Game":
                        NetworkMain.Team = payload["Team"];
                        SceneManager.LoadScene("Lobby");
                        break;
                    case "Welcome":
                        NetworkMain.Username = payload["Username"];
                        NetworkMain.UserID = payload["UserID"];
                        NetworkMain.LobbyID = payload["Server"];
                        //                        NetworkMain.recieverInit();
                        //                        NetworkMain.clientType = "Client";
                        //                        SceneManager.LoadScene("mainScene");
                        EnjinScript.AuthApp();
                        toast.newNotification("Logged in");
                        switch (EnjinScript.getPlayer(payload["Username"]))
                        {
                            case 0:

//                                SceneManager.LoadScene("mainScene");
                                NetworkMain.Login(NetworkMain.Username, NetworkMain.LobbyID, "Join");
                                //                                loginScript.listener("Check Character Asset");
                                break;
                            case -1:
                                EnjinScript.createEnjinUser(payload["Username"]);
                                loginScript.listener("Register Enjin Wallet");
                                break;
                            case -2:
                                toast.newNotification("No Enjin Wallet linked");
                                loginScript.listener("Register Enjin Wallet");
                                break;
                        }
                        NetworkMain.currentPlayer = true;
                        break;
                    case "Denied":
                        toast.newNotification(payload["Reason"]);
                        NetworkMain.disconnect();
                        break;
                }
            }
        }
    }
}

public class ResourceWrapper
{
    public List<Resource> Resources;
}