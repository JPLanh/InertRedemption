using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Socket.Newtonsoft.Json;
using Socket.Newtonsoft.Json.Linq;
using System.Text;
using System;

public class PlayerController : MonoBehaviour, ButtonListenerInterface, IPlayerController
{

    #region variables

    public GameObject buildPlacement = null;

    public LivingBeing livingBeing;
    public Camera playerCamera;
    public CharacterController characterController;
    public GameObject crosshair;
    public Survivors survivorsGO;
    public BasicMovement movementController;
    public EntityManager em;
    public InfectionScript ifs;
    public float movementTimer;
    public float rotationTimer;

    public PlayerCanvas canvas;
    public Animator userProjectionAnimator;
    public GameObject displayInterface;

    public TransferCenter visitingTransferCenter;

    //private float tallShieldLookXHi = 5.0f;
    //private float tallShieldLookXLo = -30.0f;
    ////    private string lookState = "Tall Shield";
    //private string lookState = "Normal";

    public string userID;

    public GameObject harvester;

    private float spamTimer = 0f;
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
    public bool inShip = true;
    public Image damageIndicator;

    public bool isLocal = false;
    public int actionProgress = 0;
    public int actionTimer = 0;
    public List<GameObject> revealTarget;

    public bool withinEnergyGrid = false;
    private IAbilities[] abilities = new IAbilities[5];

    public bool singleHandUse = false;

    private PlayerHubUI hudDisplay;
    private string resourceRemember;

    public Armor armor;
    public Feet feet;
    public Inventory inventory;
    public Visor visor;
    //    public IInventory inventory;
    //    public IVisor visor;

    public bool isHost;

    private bool rechargeStation = false;
    public int weaponState = 0;

    public float insanityLevel = 100;
    private bool onShip = true;
    public AudioSource heartbeat_sound;
    public AudioSource breathing_sound;
    public PlayerNetworkListener networkListener;
    #endregion

    #region Inits
    void Start()
    {
        //        survivorsGO = GameObject.Find("Survivors").GetComponent<Survivors>();
        if (damageIndicator != null)
            damageIndicator.color = new Color(1, 0, 0, 0);
        //For skill testing purposes
        //abilities[0] = gameObject.AddComponent<Rush>() as IAbilities;
        //abilities[1] = gameObject.AddComponent<Leap>() as IAbilities;
        //abilities[2] = gameObject.AddComponent<Pull>() as IAbilities;
        //        abilities[3] = gameObject.AddComponent<Repel>() as IAbilities;
        //abilities[0].assignLivingBeing(livingBeing, this);
        //abilities[1].assignLivingBeing(livingBeing, this);
        //abilities[2].assignLivingBeing(livingBeing, this);
        //        abilities[3].assignLivingBeing(livingBeing, this);
        revealTarget = new List<GameObject>();

        if (livingBeing.mainHand != null && livingBeing.mainHand.childCount > 0)
        {
            livingBeing.currentWeapon = livingBeing.mainHand.GetChild(0);
            livingBeing.currentWeapon.GetComponent<UsableItemInterface>().setOwner(livingBeing);
            livingBeing.currentWeapon.TryGetComponent<WeaponBatteryAddon>(out WeaponBatteryAddon lv_battery);
            livingBeing.currentWeapon.TryGetComponent<WeaponMagazineAddon>(out WeaponMagazineAddon lv_mag);
            if (inControl)
            {
                if (lv_battery != null) canvas.energyIndicator.gameObject.SetActive(true);
                else canvas.energyIndicator.gameObject.SetActive(false);
                if (lv_mag != null) canvas.ammoIndicator.gameObject.SetActive(true);
                else canvas.ammoIndicator.gameObject.SetActive(false);
            }

        }



        if (livingBeing.weaponHarness != null && livingBeing.weaponHarness.childCount > 0)
        {
            livingBeing.offWeapon = livingBeing.weaponHarness.GetChild(0);
            livingBeing.offWeapon.GetComponent<UsableItemInterface>().setOwner(livingBeing);
        }
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
        GetComponent<CharacterController>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera.gameObject.SetActive(true);
        userID = getUserID;
        gameObject.name = getUsername;
        livingBeing.setTeamColor(new Color(0f / 255f, 191f / 255f, 188f / 255f, .81f));
        inControl = true;
    }

    public void setOtherPlayer(string getUserID, string getUsername)
    {
        crosshair = null;
        playerCamera.gameObject.SetActive(false);
        GetComponent<CharacterController>().enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        userID = getUserID;
        gameObject.name = getUsername;
        livingBeing.setTeamColor(new Color(0f / 255f, 191f / 255f, 188f / 255f, .81f));
        inControl = false;
        Debug.Log("Setting other player");
    }


    #endregion

    void Update()
    {

        networkListener.networkActionListen();
        networkListener.networkPositionListen();

        if (rechargeStation) refillWeapons();
        if (isAlive && inControl)
        {


            if (NetworkMain.Username == gameObject.name)
            {
                uiUpdater();
                damageUpdate();
                targetReveals();
                playerKeyboardActionMapper();

                if (autorun)
                {
                    if (Input.GetAxis("Vertical") != 0) autorun = false;
                }
                //if (userProjectionAnimator.GetBool("atShop"))
                //{
                //    livingBeing.currentWeapon.GetComponent<UsableItemInterface>().rechargeDurability(2);
                //}

                // Player and Camera rotation
                //if (canLook)
                //{
                //    if (!livingBeing.handAnimator.GetBool("accessMenu"))
                //    {
                //        fpsCameraView();
                //    }
                //}
            }
        }
    }

    public float getHealth()
    {
        return livingBeing.health;
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
        if (armor != null)
            canvas.lightIndicator.lightLevel.text = (100 * (visor.lightSource.charge / visor.lightSource.chargeCapacity)).ToString();

        if (livingBeing.currentWeapon.TryGetComponent<UsableItemInterface>(out UsableItemInterface getWeapon))
        {
            if (livingBeing.currentWeapon.TryGetComponent<WeaponBatteryAddon>(out WeaponBatteryAddon getBatteryWeapon))
            {
                canvas.energyIndicator.lightLevel.text = (100 * (getBatteryWeapon.charge / getBatteryWeapon.maxCharge)).ToString();
            }
            if (livingBeing.currentWeapon.TryGetComponent<WeaponMagazineAddon>(out WeaponMagazineAddon getMagazineWeapon))
            {
                canvas.ammoIndicator.lightLevel.text = $"{getMagazineWeapon.ammo} / {getMagazineWeapon.maxAmmo}";
            }
        }

    }

    //private void fpsCameraView()
    //{
    //    rotation.y += Input.GetAxis("Mouse X") * livingBeing.lookSensativity;
    //    rotation.z = 0;

    //    if (livingBeing.currentWeapon.GetComponent<Gun>() && livingBeing.handAnimator.GetBool("isReloading"))
    //    { }
    //    else
    //    {
    //        rotation.x += -Input.GetAxis("Mouse Y") * livingBeing.lookSensativity;
    //        switch (lookState)
    //        {
    //            case "Tall Shield":
    //                rotation.x = Mathf.Clamp(rotation.x, tallShieldLookXLo, tallShieldLookXHi);
    //                break;
    //            default:
    //                rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
    //                break;
    //        }

    //        livingBeing.upperBody.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
    //    }
    //    transform.eulerAngles = new Vector2(0, rotation.y);
    //}

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

    public void jump()
    {
        if (onLedge)
            print("Jump from ledge");
        //{
        //onLedge = false;
        //    moveDirection.y = livingBeing.jumpSpeed + 15;
        //    moveDirection.z = -10;

        //}
    }

    private void playerKeyboardActionMapper()
    {
        string getCmd = null;
        if (Input.GetButtonDown("Menu"))
        {
            getCmd = "Menu";
        }

        frontalView();

        if (canMove)
        {

            if (Input.GetButtonDown("Debugger Mode"))
            {
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload.Add("Time", Time.time.ToString());
                payload.Add("Action", "Ping");
                NetworkMain.broadcastAction("Ping");
            }

            if (Input.GetButtonDown("Build"))
            {
                getCmd = "Build";
            }

            if (Input.GetButtonDown("Jump"))
            {
                getCmd = "Jump";
            }

            if (Input.GetButtonDown("Crouch"))
            {
                getCmd = "Crouch";
            }

            //if (Input.GetButtonUp("Crouch"))
            //{
            //    getCmd = "CrouchUp";
            //}

            if (Input.GetButtonDown("Flash Light"))
            {
                getCmd = "Flash Light";
            }

            if (Input.GetButtonDown("Auto Run"))
            {
                toggleAutoRun();
            }

            if (Input.GetButtonDown("Swap holding"))
            {
                getCmd = "Swap holding";
            }
            if (canAttack)
            {
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
            }

            if (Input.GetButtonDown("Reload"))
            {
                getCmd = "Reload";
            }

            if (Input.GetButtonDown("Fire2"))
            {
                getCmd = "Fire2";
            }

            if (Input.GetButtonDown("Fire3"))
            {
                running(true);
            }
            if (Input.GetButtonUp("Fire3"))
            {
                running(false);
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

            if (Input.GetButtonDown("Skill 1"))
            {
                getCmd = "Skill 1";
            }

            if (Input.GetButtonDown("Skill 2"))
            {
                getCmd = "Skill 2";
            }

            if (Input.GetButtonDown("Skill 3"))
            {
                getCmd = "Skill 3";
            }

            if (Input.GetButtonDown("Skill 4"))
            {
                getCmd = "Skill 4";
            }

            if (Input.GetButtonDown("Skill 5"))
            {
                getCmd = "Skill 5";
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
                accessMenu(true);
                break;
            case "Access Console":
                rechargeStation = true;
                accessEqConsole(true, true);
                break;
            case "Leave Access Console":
                rechargeStation = false;
                accessEqConsole(true, false);
                break;
            //case "Build":
            //    buildModeSwitch();
            //    break;
            case "Swap holding":
                swapGun(true);
                break;
            case "Fire1":
                fireOne();
                break;
            case "Fire1Up":
                singleHandUse = false;
                break;
            case "Flash Light":
                toggleFlashLight();
                break;
            case "Jump":
                jump();
                break;
            case "Reload":
                reload(true);
                break;
            case "Crouch":
                crouching();
                break;
            //case "CrouchUp":
            //    livingBeing.legsAnimator.SetBool("isCrouching", false);
            //    break;
            case "Fire2":
                fireTwo();
                break;
            case "Interact":
                Interact();
                break;
            case "Skill 1":
                useAbility(1);
                break;
            case "Skill 2":
                useAbility(2);
                break;
            case "Skill 3":
                useAbility(3);
                break;
            case "Skill 4":
                useAbility(4);
                break;
            case "Skill 5":
                useAbility(5);
                break;
        }
    }

    #region actions

    public void toggleCrouching()
    {

        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Type"] = "Player Action";
        payload["Action"] = "Crouching";
        payload["Value"] = livingBeing.legsAnimator.GetBool("isCrouching").ToString();
        NetworkMain.broadcastAction(payload);
        //livingBeing.legsAnimator.SetBool("isCrouching", livingBeing.legsAnimator.GetBool("isCrouching") ? false : true);
        //if (livingBeing.legsAnimator.GetBool("isCrouching"))
        //{
        //    livingBeing.speed = 10f;
        //}
        //else
        //{
        //    livingBeing.speed = 25f;
        //}
    }

    public void crouching()
    {
        livingBeing.legsAnimator.SetBool("isCrouching", livingBeing.legsAnimator.GetBool("isCrouching") ? false : true);
        if (livingBeing.legsAnimator.GetBool("isCrouching"))
        {
            livingBeing.speed = 10f;
        }
        else
        {
            livingBeing.speed = 25f;
        }

    }

    public void fireTwo()
    {

        if (livingBeing.currentWeapon.TryGetComponent<UsableItemInterface>(out UsableItemInterface getUsable))
        {
            getUsable.fireTwo();
        }
    }

    public void toggleFlashLight()
    {
        visor.lightSource.toggle();
    }

    public void aim()
    {
        if (!livingBeing.handAnimator.GetBool("isAiming") && !livingBeing.headAnimator.GetBool("isAiming"))
        {
            livingBeing.setAnimation("isAiming", true);
            playerCamera.fieldOfView = 35f;
            playerCamera.farClipPlane = 250f;
            crosshair.SetActive(false);
        }
        else
        if (livingBeing.handAnimator.GetBool("isAiming") && livingBeing.headAnimator.GetBool("isAiming"))
        {
            livingBeing.setAnimation("isAiming", false);
            playerCamera.fieldOfView = 70f;
            playerCamera.farClipPlane = 200f;
            crosshair.SetActive(true);
        }
    }

    public void running(bool isRunning)
    {
        if (isRunning)
        {
            livingBeing.speed = 25;
            livingBeing.legsAnimator.SetBool("Walking", true);
        }
        else
        {
            livingBeing.speed = 10;
            livingBeing.legsAnimator.SetBool("Walking", false);
        }
    }

    public void toggleAutoRun()
    {
        autorun = autorun ? false : true;
    }

    public void frontalView()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out var hit, 20f, ~(1 << 7)))
        {

            if (buildPlacement != null)
            {
                buildPlacement.transform.position = hit.point;//getBuildingPlacement(hit.point);
                                                              //buildPlacement.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            Displayable selection = null;
            selection = hit.transform.GetComponent<Displayable>();
            if (selection != null)
            {
                if (displayInterface.activeInHierarchy == false)
                {
                    displayInterface.SetActive(true);
                }
                displayInterface.transform.Find("Display Text").GetComponent<Text>().text = selection.display();
            }
            else
            {
                displayInterface.SetActive(false);
            }
        }
        else
        {
            if (livingBeing.handAnimator.GetBool("isBuilding"))
            {
                buildPlacement.transform.position = new Vector3(0f, -100f, 0f);
            }
            displayInterface.SetActive(false);
        }
    }

    public void buildModeSwitch()
    {
        //Menu os dosabled
        if (hudDisplay == null)
        {
            //If building is active
            if (buildPlacement == null)
            {
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 10f, ~(1 << 7)))
                {

                    buidingPlacer target = hit.transform.GetComponent<buidingPlacer>();
                    if (target != null && target.startTimer == 0)
                    {
                        target.pickupBuildPlacer(this);

                    }
                    else
                    {
                        accessBuilding(true);
                    }

                }
                else
                {
                    accessBuilding(true);
                }
            }
            else
            {
                if (buildPlacement != null)
                {
                    reallocateResourceFromHold();
                    Destroy(buildPlacement);
                }
            }
        }
        else //If menu is already active
        {
            //If not building
            if (buildPlacement == null)
            {
                accessBuilding(true);
            }
        }
    }

    public void startBuilding(string getBuilding, bool firstTime)
    {
        if (buildPlacement == null)
        {
            switch (getBuilding)
            {
                case "Storage Depot":
                case "Ammo Depot":
                case "Barricade":
                case "Spot Light":
                case "Turret":
                case "Core Emitter":
                case "Energy Core":
                    resourceRemember = "Building/" + getBuilding;
                    break;
            }
            if (resourceRemember != null)
            {

                buildPlacement = Instantiate(Resources.Load<GameObject>(resourceRemember), transform.position, Quaternion.identity);
                bool buildable = true;

                foreach (InventoryMapping getResource in buildPlacement.GetComponent<IBuilding>().getRequirement())
                {
                    if (inventory.getInventory().TryGetValue(getResource.key, out int amount))
                    {
                        if (getResource.value > amount)
                        {
                            canvas.toast.newNotification("Insufficient " + getResource.key);
                            reallocateResourceFromHold();
                            buildable = false;
                            break;
                        }
                        else
                        {
                            inventory.getInventory()[getResource.key] -= getResource.value;
                            inventory.getHoldAmt(getResource.key, getResource.value);
                            buildPlacement.GetComponent<buidingPlacer>().resourceHold.Add(getResource);
                        }
                    }
                    else
                    {
                        canvas.toast.newNotification("Insufficient " + getResource.key);
                        reallocateResourceFromHold();
                        buildable = false;
                        break;
                    }
                }

                if (buildable)
                {
                    buildPlacement.transform.SetParent(this.transform);
                    buildPlacement.GetComponent<buidingPlacer>().accessor = this;
                    buildPlacement.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    if (firstTime) accessBuilding(true);
                }
                else
                {
                    reallocateResourceFromHold();
                    Destroy(buildPlacement);
                    buildPlacement = null;
                }
            }
        }
    }

    public void reallocateResourceFromHold()
    {
        foreach (InventoryMapping items in buildPlacement.GetComponent<buidingPlacer>().resourceHold)
        {
            if (inventory.getInventory().TryGetValue(items.key, out int amount))
            {
                inventory.getInventory()[items.key] += items.value;
            }
            else
            {
                inventory.getInventory().Add(items.key, items.value);
            }
            inventory.getHoldAmt(items.key, items.value);
        }
        buildPlacement.GetComponent<buidingPlacer>().resourceHold = new List<InventoryMapping>();
    }

    public void fireOne()
    {
        if (buildPlacement != null)
        {
            if (!singleHandUse)
            {
                //TODO: Broadcast the buildings
                buildPlacement.GetComponent<buidingPlacer>().placeBuilding(this);
                buildPlacement.transform.SetParent(livingBeing.survivorList.transform);
                inventory.convertAllHold();
                buildPlacement = null;
                singleHandUse = true;

                if (Input.GetButton("Shift"))
                {
                    startBuilding(resourceRemember, false);
                }

            }
        }
        else
        {
            if (!singleHandUse)
            {
                if (!livingBeing.handAnimator.GetBool("accessMenu") && !livingBeing.handAnimator.GetBool("Transitioning"))
                {
                    if (livingBeing.currentWeapon.TryGetComponent<UsableItemInterface>(out UsableItemInterface getUsable))
                    {
                        getUsable.fireOne();
                    }
                }
            }
        }
    }

    public void reload(bool isLocal)
    {
        if (!livingBeing.handAnimator.GetBool("accessMenu"))
        {
            if (livingBeing.currentWeapon.TryGetComponent<UsableItemInterface>(out UsableItemInterface getUsable))
            {
                getUsable.reload();
            }

        }
    }

    public void accessEqConsole(bool isLocal, bool activating)
    {
        //if (activating)
        //{
        //    if (hudDisplay == null)
        //    {
        //        livingBeing.handAnimator.SetBool("accessMenu", true);
        //        hudDisplay = Instantiate(Resources.Load<GameObject>("Display/Equiptment Console"), transform.position, Quaternion.identity);
        //        hudDisplay.transform.SetParent(playerCamera.transform);
        //        hudDisplay.transform.localPosition = new Vector3(-.2f, .25f, 2f);
        //        hudDisplay.transform.localScale = new Vector3(.005f, .005f, .005f);
        //        hudDisplay.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        //        displayInterface.SetActive(false);
        //        livingBeing.setAnimation("isAiming", false);
        //        //                canLook = false;
        //        if (name.Equals(NetworkMain.Username))
        //        {
        //            Cursor.lockState = CursorLockMode.None;
        //            crosshair.SetActive(false);
        //        }

        //        //-33
        //        consoleUIMenu(livingBeing.currentWeapon.name, "Select Current", "Current Weapon", new Vector3(0f, 13, 0f), hudDisplay.GetComponent<ConsoleUI>().weaponUI);
        //        consoleUIMenu(livingBeing.offWeapon.name, "Select Off", "Off Weapon", new Vector3(0f, -20, 0f), hudDisplay.GetComponent<ConsoleUI>().weaponUI);

        //        consoleUIMenu(visor.getName(), "Select Visor", "Visor", new Vector3(0f, 49f, 0f), hudDisplay.GetComponent<ConsoleUI>().BodyUI);
        //        consoleUIMenu(armor.GetComponent<IArmor>().getName(), "Select Armor", "Armor", new Vector3(0f, 49f - (1 * 33), 0f), hudDisplay.GetComponent<ConsoleUI>().BodyUI);
        //        consoleUIMenu(feet.getName(), "Select Feet", "Feet", new Vector3(0f, 49f - (2 * 33), 0f), hudDisplay.GetComponent<ConsoleUI>().BodyUI);
        //        consoleUIMenu(inventory.getName(), "Select Inventory", "Inventory", new Vector3(0f, 49f - (3 * 33), 0f), hudDisplay.GetComponent<ConsoleUI>().BodyUI);

        //    }
        //    else
        //    {
        //        if (hudDisplay.transform.GetChild(0).TryGetComponent<mainMenu>(out mainMenu menu))
        //        {

        //            if (menu.state == "Building")
        //            {
        //                accessBuilding(true);
        //                accessEqConsole(true, true);
        //            }
        //            else if (menu.state == "Main Menu")
        //            {
        //                accessMenu(true);
        //                accessEqConsole(true, true);
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    livingBeing.handAnimator.SetBool("accessMenu", false);
        //    Destroy(hudDisplay);
        //    //            mainMenuGO.transform.GetChild(0).GetComponent<mainMenu>().animator.SetBool("isActive", false);
        //    hudDisplay = null;
        //    canLook = true;
        //    //            sendAction("Menu");
        //    if (name.Equals(NetworkMain.Username))
        //    {
        //        Cursor.lockState = CursorLockMode.Locked;
        //        crosshair.SetActive(true);
        //    }
        //}
    }

    private void consoleUIMenu(string getButtonName, string getAction, string getTextLabel, Vector3 getPosition, Transform parentTransform)
    {

        GameObject subMenu = Instantiate(Resources.Load<GameObject>("Display/Console_Button"), transform.position, Quaternion.identity);
        subMenu.transform.SetParent(parentTransform);
        subMenu.GetComponent<UIConsoleButton>().setButton(getButtonName, getAction);
        subMenu.GetComponent<UIConsoleButton>().actionListener = this;
        subMenu.transform.localPosition = getPosition;
        subMenu.transform.localScale = new Vector3(1f, 1f, 1f);
        subMenu.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        GameObject subMenuText = Instantiate(Resources.Load<GameObject>("Display/Text"), transform.position, Quaternion.identity);
        subMenuText.transform.SetParent(subMenu.transform);
        subMenuText.GetComponent<UIText>().textField.text = getTextLabel;
        subMenuText.transform.localPosition = new Vector3(0, 15f, 0f);
        subMenuText.transform.localScale = new Vector3(1f, 1f, 1f);
        subMenuText.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void accessMenu(bool isLocal)
    {
        if (hudDisplay == null)
        {
            Instantiate(Resources.Load<GameObject>("UI/Player UI Hub"), livingBeing.upperBody).TryGetComponent<PlayerHubUI>(out hudDisplay);
            hudDisplay.TryGetComponent<PlayerHubUI>(out PlayerHubUI out_hub);
            livingBeing.handAnimator.SetBool("accessMenu", true);
            out_hub.init(this);
            canLook = false;
            if (NetworkMain.Username.Equals(name))
            {
                Cursor.lockState = CursorLockMode.None;
                livingBeing.setAnimation("isAiming", false);
                crosshair.SetActive(false);
            }
        }
        else
        {
            hudDisplay.closeMenu();
            Destroy(hudDisplay.gameObject);
            livingBeing.handAnimator.SetBool("accessMenu", false);
            canLook = true;
            if (NetworkMain.Username.Equals(name))
            {
                Cursor.lockState = CursorLockMode.Locked;
                crosshair.SetActive(true);
            }
        }
        //if (mainMenuGO == null)
        //{
        //    livingBeing.handAnimator.SetBool("accessMenu", true);
        //    mainMenuGO = Instantiate(Resources.Load<GameObject>("Display/Main"), transform.position, Quaternion.identity);
        //    mainMenuGO.transform.SetParent(playerCamera.transform);
        //    mainMenuGO.transform.localPosition = new Vector3(-1.75f, .75f, 2f);
        //    mainMenuGO.transform.localScale = new Vector3(.005f, .005f, .005f);
        //    mainMenuGO.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        //    mainMenuGO.transform.GetChild(0).GetComponent<mainMenu>().playerController = this;
        //    mainMenuGO.transform.GetChild(0).GetComponent<mainMenu>().mainUI();
        //    displayInterface.SetActive(false);
        //    canLook = false;
        //    //sendAction("Menu");
        //    if (isLocal)
        //    {
        //        Cursor.lockState = CursorLockMode.None;
        //        livingBeing.setAnimation("isAiming", false);
        //        crosshair.SetActive(false);
        //    }
        //}
        //else
        //{
        //    if (mainMenuGO.transform.GetChild(0).TryGetComponent<mainMenu>(out mainMenu menu))
        //    {

        //        if (menu.state == "Building")
        //        {
        //            accessBuilding(true);
        //            accessMenu(true);
        //        }
        //        else if (menu.state == "Main Menu")
        //        {
        //            livingBeing.handAnimator.SetBool("accessMenu", false);
        //            Destroy(mainMenuGO);
        //            //            mainMenuGO.transform.GetChild(0).GetComponent<mainMenu>().animator.SetBool("isActive", false);
        //            mainMenuGO = null;
        //            canLook = true;
        //            //sendAction("Menu");
        //            if (isLocal)
        //            {
        //                Cursor.lockState = CursorLockMode.Locked;
        //                crosshair.SetActive(true);
        //            }
        //        }
        //    }
        //}
    }

    public void accessBuilding(bool isLocal)
    {
        //if (hudDisplay == null)
        //{
        //    livingBeing.handAnimator.SetBool("accessMenu", true);
        //    hudDisplay = Instantiate(Resources.Load<GameObject>("Display/Main"), transform.position, Quaternion.identity);
        //    hudDisplay.transform.SetParent(playerCamera.transform);
        //    hudDisplay.transform.localPosition = new Vector3(-1.75f, .75f, 2f);
        //    hudDisplay.transform.localScale = new Vector3(.005f, .005f, .005f);
        //    hudDisplay.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        //    hudDisplay.transform.GetChild(0).GetComponent<mainMenu>().playerController = this;
        //    hudDisplay.transform.GetChild(0).GetComponent<mainMenu>().buildUI();
        //    displayInterface.SetActive(false);
        //    canLook = false;
        //    //sendAction("Menu");
        //    if (isLocal)
        //    {
        //        Cursor.lockState = CursorLockMode.None;
        //        livingBeing.setAnimation("isAiming", false);
        //        crosshair.SetActive(false);
        //    }
        //}
        //else
        //{
        //    if (hudDisplay.transform.GetChild(0).TryGetComponent<mainMenu>(out mainMenu menu))
        //    {
        //        if (hudDisplay.transform.GetChild(0).GetComponent<mainMenu>().state == "Main Menu")
        //        {
        //            accessMenu(true);
        //            accessBuilding(true);
        //        }
        //        else if (hudDisplay.transform.GetChild(0).GetComponent<mainMenu>().state == "Building")
        //        {
        //            livingBeing.handAnimator.SetBool("accessMenu", false);
        //            Destroy(hudDisplay);
        //            //            mainMenuGO.transform.GetChild(0).GetComponent<mainMenu>().animator.SetBool("isActive", false);
        //            hudDisplay = null;
        //            canLook = true;
        //            //sendAction("Menu");
        //            if (isLocal)
        //            {
        //                Cursor.lockState = CursorLockMode.Locked;
        //                crosshair.SetActive(true);
        //            }
        //        }
        //    }

        //}
        //if (!livingBeing.handAnimator.GetBool("isBuilding") && !livingBeing.handAnimator.GetBool("isReloading"))
        //{
        //    bool tempBool = livingBeing.handAnimator.GetBool("accessMenu") ? false : true;
        //    livingBeing.handAnimator.SetBool("accessMenu", tempBool);
        //    userProjectionAnimator.SetBool("isBuilding", tempBool);
        //    displayInterface.SetActive(!tempBool);
        //    isAiming = false;
        //    canLook = !tempBool;
        //    sendAction("Menu");
        //    if (isLocal)
        //    {
        //        if (!tempBool) Cursor.lockState = CursorLockMode.Locked;
        //        else Cursor.lockState = CursorLockMode.None;
        //        crosshair.SetActive(!tempBool);
        //    }
        //}
    }

    public void swapGun(bool isLocal)
    {
        weaponState = weaponState == 0 ? 1 : 0;
        livingBeing.currentWeapon.GetComponent<UsableItemInterface>().swapItem(false);
        livingBeing.offWeapon.GetComponent<UsableItemInterface>().swapItem(true);
        Transform tmpHold = livingBeing.currentWeapon;
        livingBeing.currentWeapon = livingBeing.offWeapon;
        livingBeing.offWeapon = tmpHold;
        livingBeing.currentWeapon.SetParent(livingBeing.mainHand);
        livingBeing.offWeapon.SetParent(livingBeing.weaponHarness);
        livingBeing.currentWeapon.localPosition = new Vector3(0f, 0f, 0f);
        livingBeing.offWeapon.localPosition = new Vector3(0f, 0f, 0f);
        livingBeing.currentWeapon.localRotation = Quaternion.Euler(0f, 0f, 0f);
        livingBeing.offWeapon.localRotation = Quaternion.Euler(0f, 0f, 0f);
        livingBeing.currentWeapon.TryGetComponent<WeaponBatteryAddon>(out WeaponBatteryAddon lv_battery);
        livingBeing.currentWeapon.TryGetComponent<WeaponMagazineAddon>(out WeaponMagazineAddon lv_mag);

        if (lv_battery != null) canvas.energyIndicator.gameObject.SetActive(true);
        else canvas.energyIndicator.gameObject.SetActive(false);
        if (lv_mag != null) canvas.ammoIndicator.gameObject.SetActive(true);
        else canvas.ammoIndicator.gameObject.SetActive(false);
    }

    public void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 10f, ~(1 << 7)))
        {
            Interactable target = hit.transform.GetComponent<Interactable>();
            if (target != null)
            {
                target.Interact(this);
            }

        }
    }


    public void damageUpdate()
    {
        if (getDamage)
        {
            Color Opaque = new Color(1, 0, 0, 1);
            damageIndicator.color = Color.Lerp(damageIndicator.color, Opaque, 4 * Time.deltaTime);
            if (damageIndicator.color.a >= 0.8) //Almost Opaque, close enough
            {
                getDamage = false;
            }
        }
        if (!getDamage)
        {
            Color Transparent = new Color(1, 0, 0, 0);
            damageIndicator.color = Color.Lerp(damageIndicator.color, Transparent, 4 * Time.deltaTime);
        }
    }
    #endregion

    #region Multiplayer Configuration

    //private void sendAction(string getAction)
    //{
    //    if (!NetworkMain.local)
    //    {
    //        if (NetworkMain.clientType == "Server") NetworkMain.broadcastClientsAction(getAction, NetworkMain.Username, NetworkMain.UserID);
    //        else NetworkMain.client_sendActionToClients(getAction);
    //    }
    //}

    private void playerExit(string in_UID)
    {
        canvas.toast.newNotification($"Survivor player {name} has left the game.");
        if (ifs.currentVirus != null)
        {
            ifs.currentVirus.Eject();
            ifs.currentVirus.detachFromHost();
        }
        NetworkMain.payloadStack.Remove(in_UID);
        EntityManager.survivors.Remove(in_UID);
        Destroy(gameObject);
    }

    public void serverControl(Payload in_payload)
    {
        string[] parsedAction = in_payload.data["Action"].Split(' ');
        switch (parsedAction[0])
        {
            case "Update":
                serverControl(in_payload.data);
                break;
            case "Attach":
                Debug.Log("Attaching perform");
                foreach (KeyValuePair<string, VirusController> it_virus in EntityManager.virus)
                {
                    getInfectionScript().infect(it_virus.Value);
                }
                break;
            case "Exit":
                playerExit(in_payload.source);
                break;
            case "Spaceship":
                Debug.Log(in_payload.data["Action"]);
                survivorsGO.spaceship.addResource(parsedAction[3], int.Parse(parsedAction[2]));
                break;
            default:
                switch (in_payload.data["Action"])
                {
                    case "Player HUD Menu":
                        hudDisplay.listen(in_payload);
                        break;
                    case "Retrieve Upgrades":
                        foreach (KeyValuePair<string, string> it_upgrades in in_payload.data)
                        {
                            if (!it_upgrades.Key.Equals("Action") && !it_upgrades.Key.Equals("Type"))
                            {
                                string[] parsedGear = it_upgrades.Key.Split(' ');
                                string gearName = it_upgrades.Key.Replace(parsedGear[0] + " ", "");
                                foreach (IAddon it_addon in livingBeing.currentWeapon.GetComponent<IEquipment>().getAllAddons())
                                {
                                    if (it_addon.getName().Equals(it_upgrades.Key))
                                    {
                                        it_addon.setLevel(int.Parse(it_upgrades.Value));
                                    }
                                }
                            }
                        }
                        break;
                    case "Survivors Win":
                        canvas.initLoadingScreen("Survivors has left the area. Survivors Win");
                        canvas.gameOver();
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

    public void serverControl(Dictionary<string, string> payload)
    {
        if (transform.position != StringUtils.getVectorFromJson(payload, "Pos")
            || transform.localRotation != Quaternion.Euler(float.Parse(payload["xRot"]), 0, 0))
        {
            if (StringUtils.convertIntToString(weaponState) != payload["WeaponState"])
            {
                swapGun(false);
                weaponState = StringUtils.convertToInt(payload["WeaponState"]);
            }

            insanityLevel = float.Parse(payload["insanity"]);
            if (livingBeing.legsAnimator.GetBool("isCrouching"))
            {
                livingBeing.legsAnimator.SetBool("Running", false);
                livingBeing.legsAnimator.SetBool("Walking", true);
            }
            else
            {
                livingBeing.legsAnimator.SetBool("Running", true);
                livingBeing.legsAnimator.SetBool("Walking", false);
            }
            //            movementController.playMovementSound();
            /*
             * 
             *             Quaternion newAngle = Quaternion.Euler(0f, float.Parse(payload["yRot"]), 0f);
            Quaternion newCameraAngle = Quaternion.Euler(float.Parse(payload["yRot"]), 0f, 0f);

            if (payload["Username"].Equals(NetworkMain.Username))
            {
                transform.position = StringUtils.getVectorFromJson(payload, "Pos");
                transform.rotation = newAngle;
            } else
            {
                StartCoroutine(LerpPosition(StringUtils.getVectorFromJson(payload, "Pos"), (1f / movementTimer)));
                StartCoroutine(LerpRotation(transform.rotation, newAngle, (1f / rotationTimer)));
                StartCoroutine(LerpRotation(playerCamera.transform.rotation, newCameraAngle, (1f / rotationTimer)));
            }

             * 
             */
            Quaternion newAngle = Quaternion.Euler(0f, float.Parse(payload["yRot"]), 0f);
            Vector3 newCameraAngle = new Vector3(float.Parse(payload["xRot"]), 0f, 0f);

            if (payload["Username"].Equals(NetworkMain.Username))
            {
                transform.position = StringUtils.getVectorFromJson(payload, "Pos");
                transform.rotation = newAngle;
            } else
            {
                StartCoroutine(LerpPosition(StringUtils.getVectorFromJson(payload, "Pos"), (1f / movementTimer)));
                StartCoroutine(LerpRotation(transform.rotation, newAngle, (1f / rotationTimer)));
                livingBeing.upperBody.transform.localEulerAngles = newCameraAngle;
//                StartCoroutine(LerpCameraRotation(newCameraAngle, (1f / rotationTimer)));
            }
        }
        else
        {
            livingBeing.legsAnimator.SetBool("Running", false);
            livingBeing.legsAnimator.SetBool("Walking", false);
            //            movementController.stopMovementSound();
        }
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime/2;
            yield return null;
        }
        transform.position = targetPosition;
    }

    IEnumerator LerpRotation(Quaternion currentPosition, Quaternion targetPosition, float duration)
    {
        float time = 0;
        Quaternion startPosition = transform.rotation;

        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetPosition, time / duration);
            time += Time.deltaTime/2;
            yield return null;
        }
        transform.rotation = targetPosition;
    }

    IEnumerator LerpCameraRotation(Vector3 targetPosition, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            //livingBeing.upperBody.transform.localEulerAngles = Quaternion.Lerp(livingBeing.upperBody.transform.localEulerAngles, targetPosition, time / duration);
            time += Time.deltaTime / 2;
            yield return null;
        }
        livingBeing.upperBody.transform.localEulerAngles = targetPosition;
    }
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

    private void exitMenu(bool isLocal)
    {
        if (userProjectionAnimator.GetBool("Initialise"))
        {
            //            myAnimationController.SetBool("Transitioning", true);
            userProjectionAnimator.SetBool("Initialise", false);
            crosshair.SetActive(true);
            livingBeing.handAnimator.SetBool("accessMenu", false);

            canLook = true;
            Gun gunCheck = livingBeing.currentWeapon.GetComponent<Gun>();
            if (gunCheck != null)
                gunCheck.accessMenu(false);

        }
    }
    #endregion



    public void accessConsole()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Type"] = "Player Action";
        payload["Action"] = "Access Console";
        NetworkMain.broadcastAction(payload);
    }
    public void leaveAccessConsole()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Type"] = "Player Action";
        payload["Action"] = "Leave Access Console";
        NetworkMain.broadcastAction(payload);
    }

    public void fuelShip(ConsolePod in_pod)
    {
//        Debug.Log("Access fuling ship");
        foreach (KeyValuePair<String, int> it_inventory in inventory.getInventory())
        {
            //            Debug.Log($"{it_inventory.Key} : {it_inventory.Value}");
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["Type"] = "Player Action";
            if (it_inventory.Key.Equals("Log"))
            {
                if (it_inventory.Value > 0)
                {
                    if (survivorsGO.spaceship.resources.ContainsKey("Log"))
                    {
                        if (!NetworkMain.local)
                        {
                            payload["Action"] = $"Spaceship Add {it_inventory.Value} Log";
                            NetworkMain.broadcastAction(payload);
                        }
                        else
                            survivorsGO.spaceship.addResource("Log", it_inventory.Value);
                    }

                }
            }
            if (it_inventory.Key.Equals("Stone"))
            {
                if (it_inventory.Value > 0)
                {
                    if (survivorsGO.spaceship.resources.ContainsKey("Stone"))
                    {
                        if (!NetworkMain.local)
                        {
                            payload["Action"] = $"Spaceship Add {it_inventory.Value} Stone";
                            NetworkMain.broadcastAction(payload);
                        }
                        else
                            survivorsGO.spaceship.addResource("Stone", it_inventory.Value);
                    }

                }
            }
        }

        inventory.getInventory()["Log"] = 0;
        inventory.getInventory()["Stone"] = 0;

    }

    //        survivorsGO.spaceship.resources

    public bool pickupItem(Data getItem)
    {

        if (inventory.recieveItem(getItem.resourceName, 1))
        {
            canvas.toast.newNotification("You picked up 1 " + getItem.resourceName + " (Total: " + inventory.getInventory()[getItem.resourceName] + ")");
            //                Destroy(col.gameObject);
            return true;
        }
        else
        {
            canvas.toast.newNotification("Inventory is full");
            return false;
        }
    }

    public bool pickupItem(string in_item)
    {

        if (inventory.recieveItem(in_item, 1))
        {
            if (NetworkMain.Username.Equals(name))
            canvas.toast.newNotification("You picked up 1 " + in_item + " (Total: " + inventory.getInventory()[in_item] + ")");            
            return true;
        }
        else
        {
            if (NetworkMain.Username.Equals(name))
            {
                canvas.toast.newNotification("Inventory is full");
            }
            return false;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //if (col.TryGetComponent<TransferCenter>(out TransferCenter transferCentre))
        //{
        //    //            if (transferCentre.team == team.GetTeam())
        //    //            {
        //    visitingTransferCenter = transferCentre;
        //    userProjectionAnimator.SetBool("atShop", true);
        //    //            }
        //}
        //else if (col.TryGetComponent<EnergyGridRange>(out EnergyGridRange energyGrid))
        //{
        //    withinEnergyGrid = true;
        //}
        //else
        //{

        if (col.TryGetComponent<Spaceship>(out Spaceship spaceship))
        {
            canAttack = false;
            transform.SetParent(spaceship.transform);
            inShip = true;
            if (hudDisplay != null)
            {
                hudDisplay.closeMenu();
                Destroy(hudDisplay.gameObject);
                livingBeing.handAnimator.SetBool("accessMenu", false);
                canLook = true;
                if (NetworkMain.Username.Equals(name))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    crosshair.SetActive(true);
                }

            }
        }

        if (col.TryGetComponent<ConsolePod>(out ConsolePod pod))
        {
            if (name.Equals(NetworkMain.Username))
            {
                switch (pod.action)
                {
                    case "Infection Checker":
                        pod.updateMonitor($"Current Infection: {livingBeing.infectionRate}");
                        break;
                    case "Console":
                        if (hudDisplay == null)
                            accessConsole();
                        break;
                    case "Enter Teleport":
                        canvas.lead.transform.SetParent(survivorsGO.spaceshipList.transform);
                        canvas.lead.transform.localPosition = new Vector3(0f, 3f, 0f);
                        canvas.lead.transform.SetParent(canvas.transform.parent);
                        break;
                    case "Exit Teleport":
                        canAttack = true;
                        canvas.lead.transform.SetParent(survivorsGO.landing_zone);
                        canvas.lead.transform.localPosition = new Vector3(0f, 0f, 0f);
                        canvas.lead.transform.SetParent(canvas.transform.parent);
                        break;
                    case "Fuel Ship":
                        fuelShip(pod);
                        break;
                }

            }
        }
    }
    void OnTriggerExit(Collider col)
    {
        Spaceship getShip = col.GetComponent<Spaceship>();
        if (getShip)
        {
            inShip = false;
            canAttack = true;
            transform.SetParent(survivorsGO.survivorList.transform);
            if (hudDisplay != null)
            {
                hudDisplay.closeMenu();
                Destroy(hudDisplay.gameObject);
                livingBeing.handAnimator.SetBool("accessMenu", false);
                canLook = true;
                if (NetworkMain.Username.Equals(name))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    crosshair.SetActive(true);
                }


            }
        }


            if (col.TryGetComponent<ConsolePod>(out ConsolePod pod))
        {
            if (name.Equals(NetworkMain.Username))
            {
                switch (pod.action)
                {
                    case "Console":
                 //       if (mainMenuGO != null)
                            leaveAccessConsole();
                        break;
                }
            }
        }
        //if (col.TryGetComponent<TransferCenter>(out TransferCenter transferCentre))
        //{

        //    userProjectionAnimator.SetBool("atShop", false);
        //    if (userProjectionAnimator.GetBool("Initialise"))
        //    {
        //        accessMenu(true);
        //    }

        //    //if (transferCentre.team == team.GetTeam())
        //    //    {
        //    //    visitingTransferCenter = null;
        //    //    exitMenu(true);
        //    //    userProjectionAnimator.SetBool("Access Download", false);
        //    //    userProjectionAnimator.SetBool("Access Upload", false);
        //    //    //GameObject.Find("Canvas").GetComponent<PlayerCanvas>().shop = false;
        //    //}
        //}
        //else if (col.TryGetComponent<EnergyGridRange>(out EnergyGridRange energyGrid))
        //{
        //    withinEnergyGrid = false;
        //}

    }

    private void refillWeapons()
    {
        if (livingBeing.currentWeapon != null)
        {
            if (livingBeing.currentWeapon.TryGetComponent<UsableItemInterface>(out UsableItemInterface getWeapon))
            {
                if (livingBeing.currentWeapon.TryGetComponent<WeaponBatteryAddon>(out WeaponBatteryAddon getBatteryWeapon))
                {
                    getWeapon.rechargeDurability(50);
                }
                if (livingBeing.currentWeapon.TryGetComponent<WeaponMagazineAddon>(out WeaponMagazineAddon getMagazineWeapon))
                {
                    inventory.intervalAmmoIncrease("Ammo", 30);
                }
            }
        }
    }

    public Vector3 getBuildingPlacement(Vector3 worldPosition)
    {
        int x = -400 + 2 * Mathf.FloorToInt((worldPosition.x + 400f) / 2);
        float y = worldPosition.y + 1f;
        int z = -400 + 2 * Mathf.FloorToInt((worldPosition.z + 400f) / 2);
        return new Vector3(x, y, z);
    }

    public void useAbility(int abilitySlot)
    {
        if (abilities[abilitySlot - 1] != null)
        {
            abilities[abilitySlot - 1].activate();
        }
    }

    public void listener(string text)
    {
        string[] parser = text.Split(' ');
        string param = text.Replace(parser[0] + " ", "");
        switch (parser[0])
        {
            case "Select":
                selectListenerOption(param);
                break;
        }
    }

    public void listen(Payload in_payload)
    {
        hudDisplay.listen(in_payload);
    }

    private void createText(string getString, Vector3 getPos)
    {

        GameObject tmpText = Instantiate(Resources.Load<GameObject>("Display/Text"), transform.position, Quaternion.identity);
        tmpText.transform.SetParent(hudDisplay.GetComponent<ConsoleUI>().OptionUI);
        tmpText.GetComponent<UIText>().textField.text = getString;
        tmpText.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        tmpText.transform.localScale = new Vector3(1f, 1f, 1f);
        tmpText.transform.localPosition = getPos;
    }
    private void selectListenerOption(string param)
    {
        int count;
        count = 0;
        foreach (Transform child in hudDisplay.GetComponent<ConsoleUI>().OptionUI)
        {
            Destroy(child.gameObject);
        }
        createText("Hand", new Vector3(-98.5f, 101f, 0f));
        createText("Need", new Vector3(-73.5f, 101f, 0f));
        createText("Item", new Vector3(-36.5f, 101f, 0f));

        createText("Hand", new Vector3(8f, 101f, 0f));
        createText("Need", new Vector3(33f, 101f, 0f));
        createText("Hand", new Vector3(70f, 101f, 0f));

        switch (param)
        {
            case "Current":
                selectMainConsole(livingBeing.currentWeapon.GetComponent<IEquipment>().getAllAddons(), new Vector3(-157f, -11f - (37f * count), 30f), hudDisplay.GetComponent<ConsoleUI>().OptionUI, "Current");
                break;
            case "Off":
                selectMainConsole(livingBeing.offWeapon.GetComponent<IEquipment>().getAllAddons(), new Vector3(-157f, -11f - (37f * count), 30f), hudDisplay.GetComponent<ConsoleUI>().OptionUI, "Current");

                break;
            case "Visor":
                break;
            case "Armor":
                selectMainConsole(armor.GetComponent<IEquipment>().getAllAddons(), new Vector3(-157f, -11f - (37f * count), 30f), hudDisplay.GetComponent<ConsoleUI>().OptionUI, "Current");
                break;
            case "Feet":
                break;
            case "Inventory":
                //foreach (IAddon getAddon in inventory.getAllAddons())
                //{
                //    selectMainConsole(getAddon, new Vector3(-157f, -11f - (37f * count), 30f), mainMenuGO.GetComponent<ConsoleUI>().OptionUI, "Inventory");
                //    count++;
                //}
                break;
        }
    }

    private void selectMainConsole(List<IAddon> getItem, Vector3 getPosition, Transform parentTransform, string optionType)
    {
        int counter = 0;
        foreach (IAddon getTrans in getItem)
        {
            GameObject subMenu = Instantiate(Resources.Load<GameObject>("Display/Console Upgrade Option"), transform.position, Quaternion.identity);
            subMenu.transform.SetParent(hudDisplay.GetComponent<ConsoleUI>().OptionUI);
            Vector3 pos = new Vector3(-157f, -11f - (37f * counter), 30f);
            subMenu.GetComponent<Console_Upgrade_Option>().setOption(getTrans, inventory, canvas.toast, this, optionType, hudDisplay.GetComponent<ConsoleUI>().infoText, hudDisplay.GetComponent<ConsoleUI>().upgradeText);
            subMenu.transform.localPosition = pos;
            subMenu.transform.localScale = new Vector3(1f, 1f, 1f);
            subMenu.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            counter++;
        }
    }

    public void markedAsInfected(PlayerCanvas in_canvas)
    {
        TargetMarker lv_marker = gameObject.AddComponent(Type.GetType("TargetMarker")) as TargetMarker;
        lv_marker.icon = Resources.Load("Textures/Navi_Enemy") as Sprite;
        in_canvas.playerCompass.addTarget(lv_marker);
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

    public InfectionScript getInfectionScript()
    {
        return ifs;
    }

    public void setSingleHandUse(bool in_bool)
    {
        singleHandUse = in_bool;
    }

    public LivingBeing getLivingBeing()
    {
        return livingBeing;
    }

    public List<IAddon> getAllAddons()
    {
        List<IAddon> allAddon = new List<IAddon>();
        livingBeing.currentWeapon.TryGetComponent<IEquipment>(out IEquipment out_current_Gear);
        allAddon.AddRange(out_current_Gear.getAllAddons());
        livingBeing.offWeapon.TryGetComponent<IEquipment>(out IEquipment out_off_Gear);
        allAddon.AddRange(out_off_Gear.getAllAddons());
        armor.TryGetComponent<IEquipment>(out IEquipment out_armor_Gear);
        allAddon.AddRange(out_armor_Gear.getAllAddons());
        visor.TryGetComponent<IEquipment>(out IEquipment out_visor_Gear);
        allAddon.AddRange(out_visor_Gear.getAllAddons());
        feet.TryGetComponent<IEquipment>(out IEquipment out_feet_Gear);
        allAddon.AddRange(out_feet_Gear.getAllAddons());
        inventory.TryGetComponent<IEquipment>(out IEquipment out_inventory_Gear);
        allAddon.AddRange(out_inventory_Gear.getAllAddons());
        return allAddon;
    }

    public void saveUpgrades()
    {
        List<IAddon> lv_all_addons = getAllAddons();
        foreach(IAddon it_addon in lv_all_addons)
        {
//            Debug.Log($"Saving {it_addon.getName()} : level {it_addon.getLevel()}");
        }
    }

    public void insanityUpdate(float in_light_level)
    {
        if (insanityLevel < 65f)
        {
            if (!breathing_sound.isPlaying)
            {
                breathing_sound.Play();
            }
        } else
        {
            if (breathing_sound.isPlaying)
            {
                breathing_sound.Stop();
            }
        }
        if (name.Equals(NetworkMain.Username))
        {
            if (in_light_level == 0)
            {
                insanityLevel -= Time.deltaTime;
            }
            else
            {
                insanityLevel = insanityLevel + Time.deltaTime / 3 > 100 ? 100 : insanityLevel + Time.deltaTime / 3;
            }
        }
    }

    public Inventory getInventory()
    {
        return inventory;
    }
}
