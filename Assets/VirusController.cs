using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusController : MonoBehaviour, ButtonListenerInterface, IPlayerController
{

    #region variables

    public GameObject buildPlacement = null;

    public VirusBody livingBeing;
    public Camera playerCamera;
    public CharacterController characterController;
    public GameObject crosshair;
    public Survivors survivorsGO;

    public PlayerCanvas canvas;
    public Animator userProjectionAnimator;
    public GameObject displayInterface;
    public EntityManager em;

    public TransferCenter visitingTransferCenter;

    //private float tallShieldLookXHi = 5.0f;
    //private float tallShieldLookXLo = -30.0f;
    ////    private string lookState = "Tall Shield";
    //private string lookState = "Normal";

    public string userID;

    public GameObject harvester;

    private float spamTimer = 0f;
    public int dataCount = 0;
    public Base team { get; set; }
    public string teamColor;
    public bool autorun = false;

    //    public Vector3 moveDirection = Vector3.zero;

    public bool inControl = false;
    public bool isGrounded;
    public bool canMove = true;
    public bool canLook = true;
    public bool canAttack = true;
    private bool isAlive = true;
    public bool getDamage = false;
    public bool onLedge = false;

    public bool isLocal = false;
    public int actionProgress = 0;
    public int actionTimer = 0;
    public List<GameObject> revealTarget;

    public bool withinEnergyGrid = false;
    private IAbilities[] abilities = new IAbilities[5];

    public bool singleHandUse = false;

    private GameObject mainMenuGO;
    private string resourceRemember;

    public bool isHost;

    private bool rechargeStation = false;
    public int weaponState = 0;
    public PlayerController infectedPlayer;
    public InfectionController lv_infectionController;

    public AudioSource heartbeat_sound;
    public PlayerNetworkListener networkListener;
    #endregion

    #region Inits
    void Start()
    {
        revealTarget = new List<GameObject>();
    }

    public void setPossessPlayer(string getUserID, string getUsername, PlayerCanvas in_canvas)
    {
        canvas = in_canvas;
        crosshair = canvas.crosshair;
        canvas.playerCompass.player = this.transform;
        playerCamera.gameObject.tag = "MainCamera";
        playerCamera.enabled = true;
        playerCamera.GetComponent<AudioListener>().enabled = true;
        GetComponent<CharacterController>().enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        inControl = false;
    }

    public void setActivePlayer(string getUserID, string getUsername, PlayerCanvas in_canvas)
    {
        canvas = in_canvas;
        crosshair = canvas.crosshair;
        canvas.playerCompass.player = this.transform;
        playerCamera.gameObject.tag = "MainCamera";
        playerCamera.enabled = true;
        playerCamera.GetComponent<AudioListener>().enabled = true;
        GetComponent<CharacterController>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        userID = getUserID;
        gameObject.name = getUsername;
        inControl = true;
    }

    public void setOtherPlayer(string getUserID, string getUsername)
    {
        playerCamera.GetComponent<AudioListener>().enabled = false;
        if (canvas != null) canvas.playerCompass.player = null;
        canvas = null;
        crosshair = null;
        playerCamera.gameObject.tag = "PlayerEyes";
        playerCamera.enabled = false;
        GetComponent<CharacterController>().enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        userID = getUserID;
        gameObject.name = getUsername;
        inControl = false;
    }
    #endregion

    void Update()
    {
        networkListener.networkListen();
        if (isAlive && inControl)
        {
            uiUpdater();
            if (inControl)
            {
                if (NetworkMain.Username == gameObject.name)
                {
                    targetReveals();
                    playerKeyboardActionMapper();

                    if (autorun)
                    {
                        if (Input.GetAxis("Vertical") != 0) autorun = false;
                    }
                }
            }

        }
    }

    public void emitSound(string in_sound, bool repeat)
    {
        switch (in_sound)
        {
            case "Heartbeat":
                heartbeat_sound.Play();
                heartbeat_sound.loop = true;
                break;
        }
    }

    private void uiUpdater()
    {

        if (infectedPlayer != null)
        {
            canvas.energyIndicator.lightLevel.text = (100 * (infectedPlayer.livingBeing.infectionRate / 100)).ToString();
            canvas.ammoIndicator.lightLevel.text = (100 * (infectedPlayer.livingBeing.health / 100)).ToString();
        } else
        {
            canvas.energyIndicator.enabled = false;
            canvas.ammoIndicator.enabled = false;
        }
    }

    private void targetReveals()
    {
        if (revealTarget.Count != 0)
        {
            foreach (GameObject getTarget in revealTarget)
            {
                getTarget.transform.LookAt(getTarget.GetComponent<pointerGuide>().trackedUnit);
                getTarget.transform.position = transform.position;
            }
        }
    }

    public void addTargetRevealed(Transform getTarget)
    {
        GameObject pointer = Instantiate(Resources.Load<GameObject>("Pointer Guide"), playerCamera.transform.position, playerCamera.transform.rotation);
        pointer.GetComponent<pointerGuide>().trackedUnit = getTarget;
        revealTarget.Add(pointer);
        print(pointer + " added");
    }

    private void jump()
    {
        if (onLedge)
            print("Jump from ledge");
        //{
        //onLedge = false;
        //    moveDirection.y = livingBeing.jumpSpeed + 15;
        //    moveDirection.z = -10;

        //}
    }


    public void serverControl(Payload in_payload)
    {
        Debug.Log(in_payload.data["Action"]);
        string[] parsedAction = in_payload.data["Action"].Split(' ');
        switch (parsedAction[0])
        {
            case "Update":
                if (in_payload.source != NetworkMain.Username)
                {
                    if (in_payload.data["State"] == "Alive")
                    {
                        em.spawnPlayer(in_payload.data);
                    }
                    else
                    {
                        if (in_payload.data["State"] == "Alive")
                            serverControl(in_payload.data);
                        else if (in_payload.data["State"] == "Dead")
                            Destroy(gameObject);
                    }
                }
                else
                {
                    if (isMovable())
                    {
                        serverControl(in_payload.data);
                    }
                    if (NetworkMain.isHost != bool.Parse(in_payload.data["host"]))
                    {
                        NetworkMain.isHost = bool.Parse(in_payload.data["host"]);
                        if (bool.Parse(in_payload.data["host"]))
                        {
                            em.newHost();
                        }
                    }
                }
                break;
            case "Infect":
                foreach (KeyValuePair<string, VirusController> it_virus in EntityManager.virus)
                {
                    //                                    it_virus.Value.infe
                    getInfectionScript().infect(it_virus.Value);
                }
                break;
            case "Spaceship":
                survivorsGO.spaceship.addResource(parsedAction[3], int.Parse(parsedAction[2]));
                break;
            default:
                switch (in_payload.data["Action"])
                {
                    case "Join Game":
                        PlayerController playerSpawn = em.spawnPlayer(in_payload.data);
                        NetworkMain.LobbyID = in_payload.data["lobbyID"];
                        NetworkMain.UserID = in_payload.data["UserID"];
                        canvas.timeSystem.setTime(StringUtils.convertToFloat(in_payload.data["Time"]));
                        em.resourceCounter = StringUtils.convertToInt(in_payload.data["resourceLimit"]);
                        in_payload.data["lobbyID"] = NetworkMain.LobbyID;
                        in_payload.data["Action"] = "Server Update";
                        NetworkMain.getUpdates(in_payload.data);
                        break;
                    case "Fire1":
                    case "Fire2":
                    case "Fire1Up":
                    case "Reload":
                    case "Swap holding":
                    case "Swap Gun":
                    case "Menu":
                    case "Interact":
                    case "Build":
                    case "Spawn Resource":
                    case "Flash Light":
                    case "Skill 1":
                    case "Skill 2":
                    case "Skill 3":
                    case "Skill 4":
                    case "Skill 5":
                        actionDecider(in_payload.data["Action"]);
                        break;
                        //case "Debug Time":
                        //    float timeCalc = (StringUtils.convertToFloat(getPayload["Time"]) % 600) / 600;
                        //    print("Server: " + StringUtils.convertFloatToString(timeCalc));
                        //    print("Client: " + currentTime.time);
                        //    print("Difference: " + (currentTime.time - timeCalc));
                        //    break;
                        //case "Pull":
                        //    if (NetworkMain.Username == getPayload["Username"]) getExistance.canMove = false;
                        //    break;
                        //case "Pull Finish":
                        //    if (NetworkMain.Username == getPayload["Username"])
                        //        getExistance.canMove = true;
                        //    break;
                }
                break;

        }
    }
    private void playerKeyboardActionMapper()
    {
        string getCmd = null;

        if (canMove)
        {

            if (Input.GetButtonDown("Debugger Mode"))
            {
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload["Action"] = "Debug Time";
                payload["Username"] = NetworkMain.Username;
                NetworkMain.messageServer(payload);
            }

            if (Input.GetButtonDown("Jump"))
            {
                getCmd = "Jump";
            }

            if (Input.GetButtonDown("Auto Run"))
            {
                toggleAutoRun();
            }

            if (Input.GetButtonDown("Fire1"))
            {
                getCmd = "Fire1";
            }
            if (Input.GetButton("Fire1"))
            {
                getCmd = "Fire1";
            }

            if (Input.GetButtonUp("Fire1"))
            {
                getCmd = "Fire1Up";
            }

            if (Input.GetButtonDown("Fire2"))
            {
                getCmd = "Fire2";
            }
            if (Input.GetButton("Fire2"))
            {
                getCmd = "Fire2";
            }

            if (Input.GetButtonUp("Fire2"))
            {
                getCmd = "Fire2Up";
            }


            if (Input.GetButtonDown("Interact"))
            {
                getCmd = "Interact";
            }

            if (Input.GetButton("Interact"))
            {
                getCmd = "Interact";
            }

            if (Input.GetButtonUp("Interact"))
            {
                getCmd = "InteractUp";
            }
        }

        if (getCmd != null)
        {
            if (getCmd.Contains("Up"))
            {
                if (!NetworkMain.local)
                {
                    NetworkMain.broadcastAction(getCmd);
                }
                else
                {
                    actionDecider(getCmd);
                }
            }
            else
            {
                if (Time.time > spamTimer + .15f)
                {
                    spamTimer = Time.time + .15f;
                    if (!NetworkMain.local)
                    {
                        NetworkMain.broadcastAction(getCmd);
                    }
                    else
                    {
                        actionDecider(getCmd);
                    }
                }
            }
        }
    }

    public void actionDecider(string getAction)
    {
        switch (getAction)
        {
            case "Fire1":
                fireOne();
                break;
            case "Fire1Up":
                singleHandUse = false;
                break;
        }
    }

    #region actions

    public void running(bool isRunning)
    {
        if (isRunning)
        {
            livingBeing.speed = 25;
        }
        else
        {
            livingBeing.speed = 10;
        }
    }

    public void toggleAutoRun()
    {
        autorun = autorun ? false : true;
    }


    public void Interact()
    {

    }

    #endregion

    #region Multiplayer Configuration


    public void serverControl(Dictionary<string, string> payload)
    {
        if (transform.position != StringUtils.getVectorFromJson(payload, "Pos")
            || transform.localRotation != Quaternion.Euler(float.Parse(payload["xRot"]), 0, 0))
        {

//            livingBeing.legsAnimator.SetBool("Running", true);
            this.transform.position = StringUtils.getVectorFromJson(payload, "Pos");// new Vector3(float.Parse(payload["xPos"]), float.Parse(payload["yPos"]), float.Parse(payload["zPos"]));
                                                                                    //        playerCamera.transform.localRotation = Quaternion.Euler(float.Parse(payload["xRot"]), 0, 0);
                                                                                    //livingBeing.mainHand.transform.localRotation = Quaternion.Euler(0, 0, -float.Parse(payload["xRot"]));
                                                                                    //livingBeing.weaponHarness.transform.localRotation = Quaternion.Euler(-float.Parse(payload["xRot"]), 0, 0);
//            livingBeing.upperBody.transform.localRotation = Quaternion.Euler(float.Parse(payload["xRot"]), 0, 0);
            transform.eulerAngles = new Vector2(0, float.Parse(payload["yRot"]));
        }
    }
    #endregion

    //public void joinGame(Dictionary<string, string> payload)
    //{
    //    NetworkMain.socket.Emit("Player", JsonConvert.SerializeObject(payload));

    //}

    public void died()
    {
        isAlive = false;
        if (NetworkMain.Username == gameObject.name)
        {
            //playerCamera.gameObject.transform.SetParent(transform.root);
            //playerCamera.transform.position = new Vector3(0, 800, 0);
            //playerCamera.transform.localRotation = Quaternion.Euler(90, 0, 0);
            NetworkMain.broadcastAction("Death");
        }
    }

    public void getHost(PlayerController in_player)
    {

        crosshair = canvas.crosshair;
        //        infectedPlayer = in_player;
        canvas.energyIndicator.enabled = true;
        canvas.ammoIndicator.enabled = true;
        lv_infectionController.infectedPlayer = in_player;
        lv_infectionController.canvas = canvas;
        canvas.playerCompass.player = in_player.transform;
        playerCamera.gameObject.tag = "PlayerEyes";
        in_player.playerCamera.gameObject.tag = "MainCamera";
        playerCamera.enabled = false;
        in_player.playerCamera.enabled = true;
        GetComponent<VirusController>().enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        inControl = false;
        lv_infectionController.enabled = true;
        transform.SetParent(in_player.transform);
        transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.TryGetComponent<InfectionScript>(out InfectionScript get_infected) && inControl)
        {
            get_infected.infect(this);
        }

        //col.TryGetComponent<PlayerController>(out PlayerController lv_survivor);
        //if (lv_survivor != null)
        //    Debug.Log("Survivor contact");

        //if (col.TryGetComponent<ConsolePod>(out ConsolePod pod))
        //{
        //    switch (pod.action)
        //    {
        //        case "Console":
        //            rechargeStation = true;
        //            accessEqConsole(true, true);
        //            break;
        //        case "Teleport":
        //            transform.SetParent(survivorsGO.spaceshipList.transform);
        //            transform.localPosition = new Vector3(0f, 3f, 0f);
        //            break;
        //    }
        //}
    }

    void OnTriggerExit(Collider col)
    {
        Spaceship getShip = col.GetComponent<Spaceship>();
        if (getShip)
        {
        }
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Contact");

        col.gameObject.TryGetComponent<PlayerController>(out PlayerController lv_survivor);
        if (lv_survivor != null)
            Debug.Log("Survivor contact");
    }
    public void listener(string text)
    {
        string[] parser = text.Split(' ');
        string param = text.Replace(parser[0] + " ", "");
        switch (parser[0])
        {

        }
    }

    private void createText(string getString, Vector3 getPos)
    {

        GameObject tmpText = Instantiate(Resources.Load<GameObject>("Display/Text"), transform.position, Quaternion.identity);
        tmpText.transform.SetParent(mainMenuGO.GetComponent<ConsoleUI>().OptionUI);
        tmpText.GetComponent<UIText>().textField.text = getString;
        tmpText.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        tmpText.transform.localScale = new Vector3(1f, 1f, 1f);
        tmpText.transform.localPosition = getPos;
    }
 
    public void death()
    {
        isAlive = false;
        Destroy(gameObject);
    }
    public GameObject getGameObject()
    {
        return gameObject;
    }

    public bool isMovable()
    {
        return canMove;
    }

    public void accessMenu(bool in_bool)
    {
        throw new System.NotImplementedException();
    }

    public void swapGun(bool in_bool)
    {
        throw new System.NotImplementedException();
    }

    public void fireOne()
    {
        if (infectedPlayer != null)
        {
            infectedPlayer.livingBeing.infectionRate += livingBeing.infectionRate * Time.deltaTime;

            Debug.Log($"Infecting: {livingBeing.infectionRate * Time.deltaTime}");
        }
    }

    public void toggleFlashLight()
    {
    }

    void IPlayerController.jump()
    {
    }

    public void reload(bool in_bool)
    {
    }

    public void toggleCrouching()
    {
    }

    public void fireTwo()
    {
    }

    public void useAbility(int in_num)
    {
    }

    public void setSingleHandUse(bool in_bool)
    {
        singleHandUse = in_bool;
    }

    public InfectionScript getInfectionScript()
    {
        return null;
    }

    public void buildModeSwitch() { }

}
