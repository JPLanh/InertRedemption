using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionController : MonoBehaviour, IPlayerController
{
    public bool singleHandUse = false;
    private float spamTimer = 0f;
    public PlayerCanvas canvas;
    public VirusController lv_virusController;
    public PlayerController infectedPlayer;
    [SerializeField] VirusBody livingBeing;
    public Transform virusList;
    public EntityManager em;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    void Update()
    {
        uiUpdater();
        infectedControls();


    }

    public LivingBeing getLivingBeing() { return null; }
    public float getHealth()
    {
        return livingBeing.health;
    }

    private void infectedControls()
    {

        string getCmd = null;


            if (Input.GetButtonDown("Debugger Mode"))
            {
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload["Action"] = "Debug Time";
                payload["Username"] = NetworkMain.Username;
                //NetworkMain.messageServer(payload);
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

    public void serverControl(Payload in_payload)
    {

    }

    public void actionDecider(string getAction)
    {
        switch (getAction)
        {
            case "Fire1":
                negativeInfect();
                break;
            case "Fire1Up":
                singleHandUse = false;
                break;
            case "Interact":
                Eject();
                break;
        }
    }

    private void Eject()
    {

//        lv_virusController.crosshair = null;
        //        infectedPlayer = in_player;
        canvas.playerCompass.player = lv_virusController.transform;
        infectedPlayer.playerCamera.gameObject.tag = "PlayerEyes";
        lv_virusController.playerCamera.gameObject.tag = "MainCamera";
        lv_virusController.playerCamera.enabled = true;
        infectedPlayer.playerCamera.enabled = false;
        enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        lv_virusController.inControl = true;
        lv_virusController.enabled = true;

        infectedPlayer = null;
        lv_virusController.infectedPlayer = null;
        transform.SetParent(virusList);
        transform.localPosition = new Vector3(Random.Range(-475, 475), 3f, Random.Range(-475, 475));
    }
    private void negativeInfect()
    {
        Debug.Log("Infecting");
        if (infectedPlayer != null)
        {
            infectedPlayer.livingBeing.infectionRate += livingBeing.infectionRate * Time.deltaTime;
            infectedPlayer.TryGetComponent<TargetMarker>(out TargetMarker get_targetMarker);
            if (get_targetMarker == null && infectedPlayer.livingBeing.infectionRate >= 60f)
            {
                infectedPlayer.markedAsInfected(canvas);
            }           

        }
    }

    private void uiUpdater()
    {

        if (infectedPlayer != null)
        {
            canvas.energyIndicator.lightLevel.text = (100 * (infectedPlayer.livingBeing.infectionRate / 100)).ToString();
            canvas.ammoIndicator.lightLevel.text = (100 * (infectedPlayer.livingBeing.health / 100)).ToString();
        }
    }

    public GameObject getGameObject()
    {
        return gameObject;
    }

    public bool isMovable()
    {
        return false;
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
        negativeInfect();
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


    public void setActivePlayer(string getUserID, string getUsername, PlayerCanvas in_canvas)
    {
    }

    public void setOtherPlayer(string getUserID, string getUsername)
    {
    }


    public void serverControl(Dictionary<string, string> payload)
    {
    }

    public void Interact()
    {
        Eject();
    }

    public void saveUpgrades()
    {

    }
}