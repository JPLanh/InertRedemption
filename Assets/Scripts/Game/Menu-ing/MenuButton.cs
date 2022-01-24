using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public Text centerText;
    public Text item;
    public bool isMenu = false;
    public int buttonCounter;
    public PlayerController player;
    public string buttonName;
    public string action;
    public UserProjection parentMenu;
    public ButtonListenerInterface actionListener;
    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        if (parentMenu != null)
            actionListener = parentMenu.getActionListener();
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = .1f;
    }

    public void setMenu(int getCounter)
    {
        isMenu = true;
        buttonCounter = getCounter;
    }

    public void setActionListener(ButtonListenerInterface getListener)
    {
        actionListener = getListener;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClick()
    {
        actionListener.listener(action);
    }
   
    public void buttonHooverEnter()
    {
        centerText.text = buttonName + "\n";
        if (action.Contains("Build"))
        {
            List<InventoryMapping> requirement = null;
            switch (buttonName)
            {
                case "Storage Depot":
                    requirement = StorageDepot.BuildingRequirement();
                    break;
                case "Ammo Depot":
                    requirement = AmmoDepot.BuildingRequirement();
                    break;
                case "Barricade":
                    requirement = Barricade.BuildingRequirement();
                    break;
                case "Spot Light":
                    requirement = SpotLight.BuildingRequirement();
                    break;
                case "Turret":
                    requirement = Turret.BuildingRequirement();
                    break;
                case "Core Emitter":
                    requirement = Building.BuildingRequirement();
                    break;
                case "Energy Core":
                    requirement = EnergyCore.BuildingRequirement();
                    break;
            }
            foreach(InventoryMapping req in requirement)
            {
                centerText.text += req.key + ": " +
                    (player.inventory.getInventory().ContainsKey(req.key) ? player.inventory.getInventory()[req.key] : 0) + "/" + req.value +"\n";
            }
        }

    }

    public void buttonHooverExit()
    {
        centerText.text = "";
    }

    public void downloadClick()
    {
        transform.parent.GetComponent<Animator>().SetBool("Access Upload", false);
        transform.parent.GetComponent<Animator>().SetBool("Access Download", true);
    }

    public void uploadClick()
    {
        print("Upload clicked");
        transform.parent.GetComponent<Animator>().SetBool("Access Upload", true);
        transform.parent.GetComponent<Animator>().SetBool("Access Download", false);
    }

    public void tradeClick()
    {
        actionListener.listener("Trade");
    }
}
