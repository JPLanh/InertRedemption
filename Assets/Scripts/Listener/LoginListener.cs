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

    private bool useEnjin = false;
    // Start is called before the first frame update

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        listenHandler();

    }

    private void listenHandler()
    {
        if (NetworkMain.mainMenuResponse.Count > 0)
        {
            {
                Dictionary<string, string> payload = NetworkMain.mainMenuResponse.Dequeue();
                switch (payload["Action"])
                {
                    case "Enter Game":
                        SceneManager.LoadScene("Lobby");
                        break;
                    case "Welcome":
                        NetworkMain.Username = payload["Username"];
                        NetworkMain.UserID = payload["UserID"];
                        NetworkMain.LobbyID = payload["Server"];
                        if (useEnjin)
                        {
                            EnjinScript.AuthApp();
                            toast.newNotification("Logged in");
                            switch (EnjinScript.getPlayer(payload["Username"]))
                            {
                                case 0:
                                    NetworkMain.Login(NetworkMain.Username, NetworkMain.LobbyID, "Join");
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
                        }
                        else
                        {
                            NetworkMain.Login(NetworkMain.Username, NetworkMain.LobbyID, "Join");
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