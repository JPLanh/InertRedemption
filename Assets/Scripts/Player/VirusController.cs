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
    public VirusMovement movementController;
    public GameObject crosshair;
    public Survivors survivorsGO;
    public Transform virusList;

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

    public float getHealth()
    {
        return livingBeing.health;
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
        Debug.Log("Setting active plater");
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
        Debug.Log("Setting second plater");
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
        networkListener.networkActionListen();
        networkListener.networkPositionListen();
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

    private void FixedUpdate()
    {
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
        string[] parsedAction = in_payload.data["Action"].Split(' ');
        //        Debug.Log(in_payload.data["Action"]);
        switch (parsedAction[0])
        {
            case "Eject":
                Eject();
                if (in_payload.source.Equals(NetworkMain.Username)) detachFromHost();
                break;
            case "Infect":
                negativeInfect(in_payload.target, float.Parse(in_payload.data["Amount"]));
                break;
            case "Update":
                serverControl(in_payload.data);
                break;
            case "Attach":
                foreach (KeyValuePair<string, VirusController> it_virus in EntityManager.virus)
                {
                    IPlayerController lv_survivor = EntityManager.players[in_payload.target];
                    lv_survivor.getInfectionScript().infect(it_virus.Value);
                }
                break;
            default:
                switch (in_payload.data["Action"])
                {
                    case "Join Game":
                        PlayerController playerSpawn = em.spawnPlayer(in_payload.data);
                        canvas.timeSystem.setTime(StringUtils.convertToFloat(in_payload.data["Time"]));
                        em.resourceCounter = StringUtils.convertToInt(in_payload.data["resourceLimit"]);
                        break;
                    default:
                        actionDecider(in_payload.data["Action"]);
                        break;
                }
                break;
        }
    }

    private void playerKeyboardActionMapper()
    {
        string getCmd = null;


        if (Input.GetButtonDown("Debugger Mode"))
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload.Add("Time", Time.time.ToString());
            payload.Add("Action", "Ping");
            NetworkMain.broadcastAction(payload);
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



        if (getCmd != null)
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["Type"] = "Player Action";
            payload["Action"] = getCmd;
            if (getCmd.Contains("Up"))
            {
                if (!NetworkMain.local)
                {
                    NetworkMain.broadcastAction(payload);
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
                        NetworkMain.broadcastAction(payload);
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
            case "Interact":
                Interact();
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
        if (infectedPlayer != null)
        {
            NetworkMain.broadcastAction("Eject");
        }
        //Eject();
    }

    #endregion

    #region Multiplayer Configuration

    //Might not need this anymore
    public void serverControl(Dictionary<string, string> payload)
    {
        if (transform.position != StringUtils.getVectorFromJson(payload, "Pos")
            || transform.localRotation != Quaternion.Euler(float.Parse(payload["xRot"]), 0, 0))
        {

            StartCoroutine(LerpPosition(StringUtils.getVectorFromJson(payload, "Pos"), .0125f));
            Quaternion newAngle = Quaternion.Euler(0f, float.Parse(payload["yRot"]), 0f);
            StartCoroutine(LerpRotation(newAngle, .0125f));
        }
    }


    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
    IEnumerator LerpRotation(Quaternion targetPosition, float duration)
    {
        float time = 0;
        Quaternion startPosition = transform.rotation;

        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetPosition;
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

    void OnTriggerEnter(Collider col)
    {
        if (col.TryGetComponent<InfectionScript>(out InfectionScript get_infected))
        {
            if(infectedPlayer == null && name.Equals(NetworkMain.Username))
            {
                get_infected.infect(this);
                NetworkMain.broadcastAction("Attach", get_infected.currentPlayer.name);
            }
        }
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
    public void getHost(PlayerController in_player)
    {
        //Something wrong here.. null pointer? no crosshair? or no canvas?
        crosshair = canvas.crosshair;
        //        infectedPlayer = in_player;
        canvas.energyIndicator.enabled = true;
        canvas.ammoIndicator.enabled = true;
        canvas.playerCompass.player = in_player.transform;
        playerCamera.gameObject.tag = "PlayerEyes";
        in_player.playerCamera.gameObject.tag = "MainCamera";
        playerCamera.enabled = false;
        in_player.playerCamera.enabled = true;
        //        GetComponent<VirusController>().enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        canMove = false;
        //      lv_infectionController.enabled = true;
    }

    public void attachToHost(PlayerController in_player)
    {
        infectedPlayer = in_player;
        transform.SetParent(in_player.transform);
        transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    private void Eject()
    {

        //        lv_virusController.crosshair = null;
        //        infectedPlayer = in_player;
        canvas.playerCompass.player = transform;
        Debug.Log(infectedPlayer);
        Debug.Log(infectedPlayer.playerCamera);
        Debug.Log(infectedPlayer.playerCamera.gameObject);
        infectedPlayer.playerCamera.gameObject.tag = "PlayerEyes";
        playerCamera.gameObject.tag = "MainCamera";
        playerCamera.enabled = true;
        infectedPlayer.playerCamera.enabled = false;
        enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        inControl = true;
        canMove = true;
    }

    public void detachFromHost()
    {
        infectedPlayer = null;
        transform.SetParent(virusList);
        transform.localPosition = new Vector3(Random.Range(-475, 475), 3f, Random.Range(-475, 475));
    }
    private void negativeInfect(string in_player, float in_infect)
    {
        IPlayerController currentPlayer = EntityManager.players[in_player];
        if (currentPlayer != null)
        {
            LivingBeing getBeing = currentPlayer.getLivingBeing();
            if (getBeing != null)
            {
                getBeing.infectionRate += in_infect;
            }
            else
            {
                Debug.Log("Living being doesn't exists");
            }
        } else
        {
            Debug.Log("PlayerControlelr doesn't exists");
        }
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
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload.Add("Action", "Infect");
            payload.Add("Type", "Player Action");
            payload.Add("Amount", (livingBeing.infectionRate * Time.deltaTime).ToString());
            NetworkMain.broadcastAction(payload, infectedPlayer.userID);
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

    public LivingBeing getLivingBeing()
    {
        return null;
    }
}
