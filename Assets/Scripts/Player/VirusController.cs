using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public subDisplayMenu displayInterfaceMenu;
    public EntityManager em;

    public TransferCenter visitingTransferCenter;

    //private float tallShieldLookXHi = 5.0f;
    //private float tallShieldLookXLo = -30.0f;
    ////    private string lookState = "Tall Shield";
    //private string lookState = "Normal";

    public string userID;
    public string currentAbility;

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
    public Resource infectedResource;
    public Light virusLight;

    public float infectionPoint = 0;
    public float devourPoint = 0;

    public AudioSource heartbeat_sound;
    public AudioSource replenish_sound;
    public AudioSource trapping_sound;
    public PlayerNetworkListener networkListener;
    public GameObject hudObject;
    private PlayerHubUI hudDisplay;
    public VirusUpgrades upgrades;


    #endregion

    #region Inits
    void Start()
    {
        revealTarget = new List<GameObject>();
        currentAbility = "Replenishment";
        upgrades = new VirusUpgrades();
        displayInterface.TryGetComponent<subDisplayMenu>(out displayInterfaceMenu);
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
        canvas = in_canvas;
        crosshair = canvas.crosshair;
        canvas.playerCompass.player = this.transform;
        canvas.timeSystem.time = .46f;
        GetComponent<CharacterController>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera.gameObject.SetActive(true);
        userID = getUserID;
        gameObject.name = getUsername;
        virusLight.gameObject.SetActive(true);
        inControl = true;
    }

    public void setOtherPlayer(string getUserID, string getUsername)
    {
        if (canvas != null) canvas.playerCompass.player = null;
        crosshair = null;
        playerCamera.gameObject.SetActive(false);
        GetComponent<CharacterController>().enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        userID = getUserID;
        virusLight.gameObject.SetActive(false);
        gameObject.name = getUsername;
        inControl = false;
    }
    #endregion

    void Update()
    {
        networkListener.networkActionListen();
        networkListener.networkPositionListen();
        livingBeing.virusDecayTick();
        if (isAlive && inControl)
        {
            frontalView();
            if (NetworkMain.Username == gameObject.name)
            {
                uiUpdater();
                targetReveals();
                playerKeyboardActionMapper();

                if (autorun)
                {
                    if (Input.GetAxis("Vertical") != 0) autorun = false;
                }
            }
        }
    }


    public void frontalView()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out var hit, 20f, ~(1 << 7)))
        {

            Displayable selection = null;
            selection = hit.transform.GetComponent<Displayable>();
            if (selection != null)
            {
                if (displayInterface.activeInHierarchy == false)
                {
                    displayInterface.SetActive(true);
                }
                displayInterfaceMenu.textField.text = selection.display();
            }
            else
            {
                displayInterface.SetActive(false);
            }
        }
        else
        {
            displayInterface.SetActive(false);
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

        canvas.ammoIndicator.lightLevel.text = (100 * (livingBeing.health / 100)).ToString();
        if (infectedPlayer != null)
        {
            canvas.energyIndicator.gameObject.SetActive(true);
            canvas.energyIndicator.lightLevel.text = (100 * (infectedPlayer.livingBeing.infectionRate / 100)).ToString();
        } else
        {
            canvas.energyIndicator.gameObject.SetActive(false);
//            canvas.ammoIndicator.enabled = false;
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
    private void playerExit(string in_UID)
    {
        canvas.toast.newNotification($"Virus player {name} has left the game.");
        if (infectedPlayer != null)
        {
            infectedPlayer.ifs.currentVirus = null;
        }
        NetworkMain.payloadStack.Remove(in_UID);
        EntityManager.virus.Remove(in_UID);
        Destroy(gameObject);
    }
    public void serverControl(Payload in_payload)
    {
        string[] parsedAction = in_payload.data["Action"].Split(' ');
        //        Debug.Log(in_payload.data["Action"]);
        switch (parsedAction[0])
        {
            case "Upgrade":
                performUpgrade(in_payload.data["Ability"]);
                upgrades.addAbilityLevel(new KeyValuePair<string, string>(in_payload.data["Ability"], in_payload.data["Amount"]));
                break;
            case "Death":
                death();
                break;
            case "Eject":
                Eject();
                detachFromHost();
                break;
            case "Infect":
                switch (in_payload.data["Ability"])
                {
                    case "Replenishment":
                        negativeInfect(in_payload.target, float.Parse(in_payload.data["Amount"]));
                        break;
                    case "Consume Host":
                        consumeHost(in_payload.target);
                        break;
        }
        break;
            case "Update":
                serverControl(in_payload.data);
                break;
            case "Attach":
                foreach (KeyValuePair<string, VirusController> it_virus in EntityManager.virus)
                {
                    PlayerController lv_survivor = EntityManager.survivors[in_payload.target];
                    lv_survivor.getInfectionScript().infect(it_virus.Value);
                }
                break;
            default:
                switch (in_payload.data["Action"])
                {
                    case "Modify Health":
                        modifyHealth(float.Parse(in_payload.data["Amount"]));
//                        damage(float.Parse(in_payload.data["Amount"]));
                        break;
                    case "Retrieve Upgrades":
                        foreach (KeyValuePair<string, string> it_upgrades in in_payload.data)
                        {
                            if (!it_upgrades.Key.Equals("Action") && !it_upgrades.Key.Equals("Type"))
                            {
                                upgrades.addAbilityLevel(it_upgrades);
                            }
                        }
                        break;
                    case "Player HUD Menu":
                        hudDisplay.listen(in_payload);
                        break;
                    case "Exhaust Resource":
                    case "Harvest Resource":
                        harvestResource(in_payload.source, in_payload.target, in_payload.data);
                        break;
                    case "Exit Game Session":
                        playerExit(in_payload.source);
                        break;
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

        if (infectedPlayer == null)
        {
            if (Input.GetButtonDown("Menu"))
            {
                getCmd = "Menu";
            }
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
        if (canAttack)
        {



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
            case "Menu":
                accessMenu();
                break;
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

    public void accessMenu()
    {
        if (hudDisplay == null)
        {
            Instantiate(Resources.Load<GameObject>("UI/Player UI Hub"), hudObject.transform).TryGetComponent<PlayerHubUI>(out hudDisplay);
            hudDisplay.TryGetComponent<PlayerHubUI>(out PlayerHubUI out_hub);
            out_hub.init(this);
            canLook = false;
            canAttack = false;
            if (NetworkMain.Username.Equals(name))
            {
                Cursor.lockState = CursorLockMode.None;
                crosshair.SetActive(false);
            }
        }
        else
        {
            hudDisplay.closeMenu();
            Destroy(hudDisplay.gameObject);
            canLook = true;
            canAttack = true;
            if (NetworkMain.Username.Equals(name))
            {
                Cursor.lockState = CursorLockMode.Locked;
                crosshair.SetActive(true);
            }
        }
    }

    public void selectAbility(string in_ability)
    {
        currentAbility = in_ability;
        Debug.Log("Swap abilities");
    }

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
            ejectHost();
        }
    }

    public void ejectHost()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Type"] = "Player Action";
        payload["Action"] = "Eject";
        NetworkMain.broadcastAction(payload);
    }
    #endregion

    #region Multiplayer Configuration

    //Might not need this anymore
    public void serverControl(Dictionary<string, string> payload)
    {
        if (transform.position != StringUtils.getVectorFromJson(payload, "Pos")
            || transform.localRotation != Quaternion.Euler(float.Parse(payload["xRot"]), 0, 0))
        {

            Quaternion newAngle = Quaternion.Euler(0f, float.Parse(payload["yRot"]), 0f);

            if (payload["Username"].Equals(NetworkMain.Username))
            {
                transform.position = StringUtils.getVectorFromJson(payload, "Pos");
                transform.rotation = newAngle;
            }
            else
            {
                StartCoroutine(LerpPosition(StringUtils.getVectorFromJson(payload, "Pos"), .0125f));
                StartCoroutine(LerpRotation(newAngle, .0125f));
            }
            //transform.position = StringUtils.getVectorFromJson(payload, "Pos");
            //transform.rotation = newAngle;
            //            StartCoroutine(LerpPosition(StringUtils.getVectorFromJson(payload, "Pos"), .0125f));
//            StartCoroutine(LerpRotation(newAngle, .0125f));
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
            if (infectedPlayer == null && name.Equals(NetworkMain.Username) && get_infected.currentVirus == null)
            {
                Debug.Log("Attaching");
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
        crosshair = canvas.crosshair;
        canvas.playerCompass.player = in_player.transform;
        in_player.playerCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);
        //playerCamera.enabled = false;
        //in_player.playerCamera.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        canMove = false;
        canAttack = true;
    }

    public void attachToHost(PlayerController in_player)
    {
        infectedPlayer = in_player;
        transform.SetParent(in_player.transform);
        transform.localPosition = new Vector3(0f, 0f, 0f);
        if (hudDisplay != null)
        {
            hudDisplay.closeMenu();
            Destroy(hudDisplay.gameObject);
        }

    }

    public void attachToHost(Resource in_resource)
    {
        canMove = false;
        infectedResource = in_resource;
        transform.SetParent(in_resource.transform);
        transform.localPosition = new Vector3(0f, 2f, 0f);
    }

    public void Eject()
    {
        infectedPlayer.ifs.currentVirus = null;
        infectedPlayer.playerCamera.gameObject.SetActive(false);
        enabled = true;
        if (NetworkMain.Username.Equals(name))
        {
            canvas.playerCompass.player = transform;
            Cursor.lockState = CursorLockMode.Locked;
            playerCamera.gameObject.SetActive(true);
        }
        inControl = true;
        canMove = true;
        canAttack = true;
        canLook = true;
    }

    public void detachFromHost()
    {
        infectedPlayer = null;
        transform.SetParent(virusList);
        transform.localPosition = new Vector3(Random.Range(-475, 475), 3f, Random.Range(-475, 475));
    }

    private void consumeHost(string in_player)
    {
        IPlayerController currentPlayer = EntityManager.players[in_player];
        if (currentPlayer != null)
        {
            LivingBeing getBeing = currentPlayer.getLivingBeing();
            if (getBeing != null)
            {
                if (getBeing.infectionRate >= 100)
                {
                    currentPlayer.death();
                }
            }
            else
            {
                Debug.Log("Living being doesn't exists");
            }
        }
        else
        {
            Debug.Log("PlayerControlelr doesn't exists");
        }
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
                infectionPoint += in_infect / 5f;
                //if (getBeing.infectionRate >= 100)
                //{
                //    currentPlayer.death();
                //}
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
    public void modifyHealth(float in_amount)
    {
        float lv_health = livingBeing.health - in_amount;
  //      Debug.Log(livingBeing.health + " , " + " - " + in_amount + " = " + lv_health);
        livingBeing.health = lv_health > 100 ? 100 : lv_health;
//        Debug.Log(livingBeing.health);
        livingBeing.damageCheck();
    }

    private void damage(float in_amount)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Type"] = "Player Action";
        payload["Action"] = "Modify Health";
        payload["Amount"] = in_amount.ToString();
        NetworkMain.broadcastAction(payload);
    }

    private void harvestResource(string in_harvester_UID, string in_resource_UID, Dictionary<string, string> in_payload){
        EntityManager.resources.TryGetValue(in_resource_UID, out Resource out_resource);
        switch (in_payload["Ability"])
        {
            case "Replenishment":
                out_resource.harvest(float.Parse(in_payload["Amount"]));
                damage((5 * float.Parse(in_payload["Amount"])));
                if (!replenish_sound.isPlaying)
                {
                    replenish_sound.Play();
                }
                break;
            case "Resource Trap":
                if (out_resource.trapping(float.Parse(in_payload["Amount"])))
                {
                    damage(-float.Parse(in_payload["Amount"]));
//                    livingBeing.health += ;
                    if (!trapping_sound.isPlaying)
                    {
                        trapping_sound.Play();
                    }
                }
                break;
        }
    }

    public void death()
    {
        isAlive = false;
        canvas.toast.newNotification($"{name} has died.");
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
        if (hudDisplay == null)
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();

            Physics.Raycast(new Ray(playerCamera.transform.position, playerCamera.transform.forward), out RaycastHit lv_hit, 3f);
            if (lv_hit.transform != null)
            {
                lv_hit.transform.TryGetComponent<Resource>(out Resource out_resource);
                if (out_resource != null && !out_resource.isDestroyed)
                {
                    float lv_infect_amt = -(livingBeing.infectionRate * Time.deltaTime) * 3;
                    payload.Add("Type", "Player Action");
                    payload.Add("Amount", lv_infect_amt.ToString());
                    if (out_resource.durability - lv_infect_amt > 0)
                    {
                        payload.Add("Action", "Harvest Resource");
                        payload.Add("Ability", currentAbility);
                    }
                    else
                    {
                        payload.Add("Action", "Exhaust Resource");
                    }
                    NetworkMain.broadcastAction(payload, out_resource.UID);
                }
            }
            if (infectedPlayer != null)
            {
                payload.Add("Action", "Infect");
                payload.Add("Ability", currentAbility);
                payload.Add("Type", "Player Action");
                payload.Add("Amount", (livingBeing.infectionRate * Time.deltaTime).ToString());
                NetworkMain.broadcastAction(payload, infectedPlayer.userID);
            }

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

    public void crouching()
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

    public void saveUpgrades()
    {

    }

    public void listen(Payload in_payload)
    {

    }


    public Inventory getInventory()
    {
        throw new System.NotImplementedException();
    }
    public bool pickupItem(string in_item)
    {
        return false;
    }

    public bool upgradeAbility(string in_upgrade, int in_level)
    {
        if (checkUpgrade(in_upgrade))
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["Type"] = "Player Action";
            payload["Action"] = "Upgrade";
            payload["Team"] = NetworkMain.Team;
            switch (in_upgrade)
            {
                case "Replenishment":
                    payload["Level"] = StringUtils.convertIntToString(upgrades.replenishment_level+in_level);
                    break;
                case "Resource Trap":
                    payload["Level"] = StringUtils.convertIntToString(upgrades.trapping_level+in_level);
                    break;
                case "Consume Host":
                    payload["Level"] = StringUtils.convertIntToString(upgrades.consumeHost_level+in_level);
                    break;
            }
            payload["Ability"] = in_upgrade;
            payload["Amount"] = in_level.ToString();
            payload["Username"] = NetworkMain.Username;
            NetworkMain.serverAction(payload);
            NetworkMain.broadcastAction(payload);
            canvas.toast.newNotification($"{in_upgrade} has been upgraded");
            return true;
        } else
        {
            return false;
        }
    }

    public bool checkUpgrade(string in_upgrade)
    {
        bool isUpgradable = false;
        foreach(KeyValuePair<string, int> it_upgrades in upgrades.getRequirement(in_upgrade))
        {
            if (infectionPoint >= it_upgrades.Value)
            {
                isUpgradable = true;
            } else
            {
                isUpgradable = false;
                break;
            }
        }
        return isUpgradable;
    }
    public bool performUpgrade(string in_upgrade)
    {
        bool isUpgradable = true;
        foreach (KeyValuePair<string, int> it_upgrades in upgrades.getRequirement(in_upgrade))
        {
            Debug.Log($"{infectionPoint} {it_upgrades.Value}");
            infectionPoint -= it_upgrades.Value;
        }
        return isUpgradable;
    }
}


public class VirusUpgrades
{
    public float replenishment_percent;
    public int replenishment_level;

    public float trapping_percent;
    public int trapping_level;

    public float consumeHost_percent;
    public int consumeHost_level;

    public VirusUpgrades()
    {
        replenishment_level = 1;
        replenishment_percent = 0;

        trapping_level = 1;
        trapping_percent = 0;

        consumeHost_level = 1;
        consumeHost_percent = 0;

    }


    public void addAbilityLevel(KeyValuePair<string, string> in_upgrade)
    {
        switch (in_upgrade.Key)
        {
            case "Replenishment":
                replenishment_level += int.Parse(in_upgrade.Value);
                break;
            case "Resource Trap":
                trapping_level += int.Parse(in_upgrade.Value);
                break;
            case "Consume Host":
                consumeHost_level += int.Parse(in_upgrade.Value);
                break;
        }
    }

    public void setAbilityLevel(KeyValuePair<string, string> in_upgrade)
    {
        switch (in_upgrade.Key)
        {
            case "Replenishment":
                replenishment_level = int.Parse(in_upgrade.Value);
                break;
            case "Resource Trap":
                trapping_level = int.Parse(in_upgrade.Value);
                break;
            case "Consume Host":
                consumeHost_level = int.Parse(in_upgrade.Value);
                break;
        }
    }


    public Dictionary<string, int> getRequirement(string in_ability){

        Dictionary<string, int> lv_requirement = new Dictionary<string, int>();

        switch (in_ability)
        {
            case "Replenishment":
                lv_requirement.Add("Infection Point", 2 * replenishment_level);
                break;
            case "Resource Trap":
                lv_requirement.Add("Infection Point", 2 * trapping_level);
                break;
            case "Consume Host":
                lv_requirement.Add("Infection Point", 2 * consumeHost_level);
                break;
        }

        return lv_requirement;
    }
}