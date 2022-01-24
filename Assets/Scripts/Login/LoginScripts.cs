using System.Collections;
using System.Collections.Generic;
using Enjin.SDK.Models.v2;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginScripts : MonoBehaviour, ButtonListenerInterface
{
    public GameObject usernameField;
    public GameObject IpField;
    [SerializeField]
    private ToastNotifications toast;
    bool actionable = true;
    private float spamTimer;
    public Dictionary<string, GameObject> labelList = new Dictionary<string, GameObject>();
    [SerializeField] EnjinScript lv_enjin;
    [SerializeField] Transform displayList;
    [SerializeField] GameObject QRCodeDisplay;
    [SerializeField] RawImage QRCode;

    float enjinTickTimer;
    bool enjinAwait;
    string state;

    // Start is called before the first frame update
    void Start()
    {

        createNewButton("Offline", "Go Offline", new Vector3(145f, 0f, 0f));
        createNewButton("Online", "Go Online", new Vector3(-145f, 0f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        if (enjinAwait)
        {
            enjinTickTimer -= Time.deltaTime;
            if (enjinTickTimer < 0)
            {
                Debug.Log(state);
                enjinTickTimer = 5;
                switch (state)
                {
                    //Linking
                    case "Register Enjin Wallet":
                        if (checkLinkingStatus())
                        {
                            NetworkMain.Login(NetworkMain.Username, NetworkMain.LobbyID, "Join");
                            SceneManager.LoadScene("mainScene");
                        }
                        break;
                    ////Creating Character asset
                    //case "Check Character Asset":
                    //    if (checkAssetCreation() != null)
                    //    {
                    //        listener("Check Character Ownership");
                    //    }
                    //    break;
                    ////Minting
                    //case "Check Character Ownership":
                    //    if (checkingCharacterAsset() != null)
                    //    {
                    //        listener("Check Character Asset URI");
                    //    }
                    //    break;
                    //case "Check Character Asset URI":
                    //    EnjinBalance lv_character_asset = checkingCharacterAsset();
                    //    if (lv_character_asset.Token.Metadatauri.Equals(@"https://jplanh.tk/Json/" + NetworkMain.Username + ".json"))
                    //    {
                    //        SceneManager.LoadScene("mainScene");
                    //    }
                    //    break;
                }
            }
        }

        if (!actionable)
        {
            if (Time.time > spamTimer)
            {
                toast.newNotification("Unable to connect");
                actionable = true;
            }
        }
    }

    private EnjinToken checkAssetCreation()
    {
        List<EnjinToken> lv_assets = EnjinScript.findAssetByName();
        if (lv_assets == null) return null;
        EnjinScript.currentCharacterToken = lv_assets[0];
        return lv_assets[0];
    }

    private bool checkLinkingStatus()
    {
        if (EnjinScript.getPlayer(NetworkMain.Username) == 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public void listener(string getString)
    {
        if (actionable)
        {
            switch (getString)
            {
                case "Go Online":
                    foreach (Transform child in transform)
                    {
                        child.GetChild(0).transform.GetComponent<Animator>().SetBool("Active", false);
                    }
                    createNewButton("Join", "Join Host", new Vector3(145f, 0f, 0f));
                    createNewButton("Create", "Create Game", new Vector3(-145f, 0f, 0f));
                    createNewButton("Back", "Return back to main menu", new Vector3(0f, -145f, 0f));
                    usernameField.SetActive(false);
                    IpField.SetActive(false);
                    break;
                case "Return back to main menu":
                    foreach (Transform child in transform)
                    {
                        child.GetChild(0).transform.GetComponent<Animator>().SetBool("Active", false);
                    }
                    usernameField.SetActive(false);
                    createNewButton("Offline", "Go Offline", new Vector3(145f, 0f, 0f));
                    createNewButton("Online", "Go Online", new Vector3(-145f, 0f, 0f));

                    break;
                case "Go Offline":
                    foreach (Transform child in transform)
                    {
                        child.GetChild(0).transform.GetComponent<Animator>().SetBool("Active", false);
                    }
                    usernameField.SetActive(true);
                    createNewButton("Play", "Play Offline", new Vector3(145f, 0f, 0f));
                    createNewButton("Back", "Return back to main menu", new Vector3(0f, -145f, 0f));
                    break;
                case "Create Game":
                    foreach (Transform child in transform)
                    {
                        child.GetChild(0).transform.GetComponent<Animator>().SetBool("Active", false);
                    }
                    usernameField.SetActive(true);
                    IpField.SetActive(true);
                    createNewButton("Create Lobby", "Initiate server", new Vector3(112f, 33f, 0f));
                    createNewButton("Register", "Register", new Vector3(112f, -33f, 0f));
                    createNewButton("Back", "Go Online", new Vector3(0f, -145f, 0f));
                    break;
                case "Initiate server":
                    if (usernameField.GetComponent<InputField>().text == null)
                    {
                        NetworkMain.Username = StringUtils.randomStringGen(7);
                    }
                    else
                        NetworkMain.Username = usernameField.GetComponent<InputField>().text;
                        var AET = StringUtils.SimpleAESEncryption.Encrypt(IpField.GetComponent<InputField>().text, "Existing", "H@rDc0d3d P4%5w0rd");
                        NetworkMain.Password = AET.EncryptedText;
                    //NetworkMain.recieverInit();
                    //NetworkMain.clientType = "Server";
                    toast.newNotification("Logging In");
                    NetworkMain.joinGame(NetworkMain.Username, NetworkMain.Password, "Login");
                    spamTimer = Time.time + 5f;
                    NetworkMain.local = false;
                    actionable = false;
                    //                NetworkMain.createServer();
                    //SceneManager.LoadScene("mainScene");
                    break;
                case "Join Host":
                    foreach (Transform child in transform)
                    {
                        child.GetChild(0).transform.GetComponent<Animator>().SetBool("Active", false);
                    }
                    usernameField.SetActive(true);
                    IpField.SetActive(true);
                    createNewButton("Join Lobby", "Enter Lobby", new Vector3(112f, 33f, 0f));
                    createNewButton("Register", "Register", new Vector3(112f, -33f, 0f));
                    createNewButton("Back", "Go Online", new Vector3(0f, -145f, 0f));
                    break;
                case "Register":
                    NetworkMain.Username = usernameField.GetComponent<InputField>().text;
                    var AETred = StringUtils.SimpleAESEncryption.Encrypt(IpField.GetComponent<InputField>().text, "Existing", "H@rDc0d3d P4%5w0rd");
                    NetworkMain.Password = AETred.EncryptedText; 
                    NetworkMain.joinGame(NetworkMain.Username, NetworkMain.Password, "Register");
                    break;
                case "Check Linking":
                    state = getString;
                    if (EnjinScript.getPlayer(NetworkMain.Username) == -2)
                    {
                        toast.newNotification("Wallet has not been linked yet, please try again");
                        lv_enjin.getQRLinkingCode(QRCode);
                    } else
                    {
                        listener("Check Character Asset");
                        //EnjinScript.setMetadataURI();
                        //NetworkMain.Login(NetworkMain.Username, NetworkMain.LobbyID, "Join");
                    }
                    break;
                case "Check Character Asset URI":
                    state = getString;
                    EnjinBalance lv_character_asset = checkingCharacterAsset();
                    if (lv_character_asset.Token.Metadatauri.Equals(@"https://jplanh.tk/Json/" + NetworkMain.Username + ".json"))
                    {
                        SceneManager.LoadScene("mainScene");
                    }
                    else
                    {
                        EnjinScript.setMetadataURI();
                        toast.newNotification("Finalizing Character account, please confrim the Engine Wallet Request");
                    }
                    break;
                case "Check Character Ownership":
                    state = getString;
                    if (checkingCharacterAsset() != null)
                    {
                        listener("Check Character URI");
                    } else
                    {
                        EnjinScript.mintCharacterAssetToUser();
                        toast.newNotification("Assigning character to your wallet, please confirm the Engine Wallet Request");
                    }
                    break;
                case "Check Character Asset":
                    state = getString;
                    enjinAwait = true;
                    if (checkingCharacterAsset() != null)
                    {
                        SceneManager.LoadScene("mainScene");
                    }
                    else
                    {
                        //if (checkAssetCreation() != null)
                        {
                        //    toast.newNotification("Sorry this user already exists, please register a new username");
                        //} else
                        //{
                        //    EnjinScript.createNewCharacter();
                            toast.newNotification("Notification has been sent to your phone through your Engine Wallet App. Please Confirm.");
                        }
                    }
                    break;
                case "Register Enjin Wallet":
                    foreach (Transform child in transform)
                    {
                        child.GetChild(0).transform.GetComponent<Animator>().SetBool("Active", false);
                    }
                    state = getString;
                    usernameField.SetActive(false);
                    IpField.SetActive(false);
                    createNewText("LabelHeader", out GameObject out_gameObject, out mainMenu out_mm);
                    out_mm.centerText.text = "Please link your Enjin Wallet\n\n";
                    out_mm.centerText.text += "1. Download 'Enjin Wallet' On your mobile phone\n";
                    out_mm.centerText.text += "2. Create a wallet\n";
                    out_mm.centerText.text += "3. Tap on the advance menu (3 bar)\n";
                    out_mm.centerText.text += "4. Select Scan QR\n";
                    out_mm.centerText.text += "5. Scan QR on the right\n";
                    out_mm.centerText.text += "6. Select Your Wallet on your Enjin Wallet on your mobile app\n";
                    out_mm.centerText.text += "7. Once you link your wallet, please click the confirm button\n";
//                    createNewButton("Confirm", "Check Linking", new Vector3(0f, -145f, 0f));
                    QRCodeDisplay.SetActive(true);
                    enjinAwait = true;
                    lv_enjin.getQRLinkingCode(QRCode);
                    out_gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                    break;
                case "Enter Lobby":
                    //              NetworkMain.serverIP = IpField.GetComponent<InputField>().text;
                    //              NetworkMain.senderInit();
                    NetworkMain.Username = usernameField.GetComponent<InputField>().text;
                    var AETEnter = StringUtils.SimpleAESEncryption.Encrypt(IpField.GetComponent<InputField>().text, "Existing", "H@rDc0d3d P4%5w0rd");
                    NetworkMain.Password = AETEnter.EncryptedText;
                    //NetworkMain.recieverInit();
                    //NetworkMain.clientType = "Server";
                    toast.newNotification("Logging In");
                    NetworkMain.joinGame(NetworkMain.Username, NetworkMain.Password, "Login");

                    //              Dictionary<string, string> payload = new Dictionary<string, string>();
                    //              payload["Username"] = usernameField.GetComponent<InputField>().text;
                    //              payload["Action"] = "Join";
                    //              payload["Mode"] = "Action";
                    //              NetworkMain.sendString(payload);
                    break;
                case "Play Offline":
                    if (string.IsNullOrEmpty(usernameField.GetComponent<InputField>().text))
                    {
                        NetworkMain.Username = StringUtils.randomStringGen(7);
                    } else
                    NetworkMain.Username = usernameField.GetComponent<InputField>().text;
                    NetworkMain.UserID = "Local";
                    NetworkMain.local = true;
                    SceneManager.LoadScene("mainScene");
                    //                    NetworkMain.Team = "Survivor";
                    NetworkMain.Team = "Virus";
                    NetworkMain.currentPlayer = true;
                    break;

            }
        }
    }

    private EnjinBalance checkingCharacterAsset()
    {
        List<EnjinBalance> lv_assets = EnjinScript.getUserAssets();
        foreach(EnjinBalance it_asset in lv_assets)
        {
//            Debug.Log(it_asset.Token.Name);
            if (it_asset.Token.Name.Equals(NetworkMain.Username))
            {
                return it_asset;
            }

        }
        return null;
    }

    private void createNewButton(string text, string getAction, Vector3 position)
    {
        GameObject newMenu = Instantiate(Resources.Load<GameObject>("Menu Selection Button"), transform.position, Quaternion.identity);
        Transform menuObj = newMenu.transform.GetChild(0).transform;
        menuObj.GetComponent<MenuSelection>().getButtonText.text = text;
        menuObj.GetComponent<MenuSelection>().getButton.GetComponent<MenuButton>().action = getAction;
        menuObj.GetComponent<MenuSelection>().getButton.GetComponent<MenuButton>().actionListener = this;
        menuObj.GetComponent<MenuSelection>().actionListener = this;
        menuObj.GetComponent<Animator>().SetBool("Active", true);
        newMenu.transform.SetParent(transform);
        newMenu.transform.position = transform.position + position;
        newMenu.transform.localScale = new Vector3(.6f, .6f, .6f);
        newMenu.name = text;
    }

    private void createNewText(string in_name, out GameObject out_gameObject, out mainMenu out_mm)
    {
        out_gameObject = Instantiate(Resources.Load<GameObject>("Display/Menu_Text"), displayList);
        out_gameObject.name = in_name;
        out_gameObject.TryGetComponent<mainMenu>(out out_mm);
        labelList.Add(in_name, out_gameObject);
    }
}
