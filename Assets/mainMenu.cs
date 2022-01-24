using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour, ButtonListenerInterface
{

    public Text centerText;
    public int buttonCounter = 0;
    public Animator animator;
    public PlayerController playerController;
    public string state;

    public void listener(string getAction)
    {
        string[] actionParse = getAction.Split(' ');
        switch (actionParse[0])
        {
            case "Build":
                //                playerController.accessBuilding(true);
                playerController.startBuilding(getAction.Replace("Build ", ""), true);
                //                playerController.buildModeSwitch();
                break;
        }
    }

    public ButtonListenerInterface getListener()
    {
        return this;
    }

    public void createNewButton(string getName, string getAction)
    {
        GameObject newMenu = Instantiate(Resources.Load<GameObject>("Display/Menu button"), new Vector3(0f, 85f*buttonCounter, 0f), Quaternion.identity);
        newMenu.transform.GetChild(0).GetComponent<MenuButton>().animator.SetBool("isActive", true);
        newMenu.transform.GetChild(0).GetComponent<MenuButton>().centerText = centerText;
        newMenu.transform.GetChild(0).GetComponent<MenuButton>().buttonName = getName;
        newMenu.transform.GetChild(0).GetComponent<MenuButton>().action = getAction;
        newMenu.transform.GetChild(0).GetComponent<MenuButton>().player = playerController;
        newMenu.transform.GetChild(0).GetComponent<MenuButton>().setActionListener(this);
        newMenu.transform.SetParent(transform.parent);
        newMenu.transform.localPosition = new Vector3(0f + (50f * (buttonCounter/6)), -75f - (50f*(buttonCounter%6)), 0f);
        newMenu.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        newMenu.transform.localScale = new Vector3(.75f, .75f, .75f);
        buttonCounter += 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator.SetBool("isActive", true);
    }

    public void mainUI()
    {
        buttonCounter = 0;
        state = "Main Menu";
        createNewButton("Status", "Status");
        createNewButton("Inventory", "Inventory");
        createNewButton("Equipment", "Equipment");
        createNewButton("Agumentation", "Augmentation");
    }

    public void buildUI()
    {
        buttonCounter = 0;
        state = "Building";
        createNewButton("Core Emitter", "Build Core Emitter");
        createNewButton("Storage Depot", "Build Storage Depot");
        createNewButton("Ammo Depot", "Build Ammo Depot");
        createNewButton("Barricade", "Build Barricade");
        createNewButton("Spot Light", "Build Spot Light");
//        createNewButton("Turret", "Build Turret");
        createNewButton("Energy Core", "Build Energy Core");

    }

    public void consoleUI()
    {
        state = "Console";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
