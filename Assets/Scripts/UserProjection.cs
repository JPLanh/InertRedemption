using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserProjection : MonoBehaviour, ButtonListenerInterface
{
    public Transform downloadScreen;
    public Transform downloadingScreen;
    public Transform uploadScreen;
    public Transform uploadingScreen;

    public Dictionary<string, GameObject> download;
    public Dictionary<string, GameObject> downloading;
    public Dictionary<string, GameObject> upload;
    public Dictionary<string, GameObject> uploading;

    public Transform tradeButton;
    public float downloadWeight;
    public float uploadWeight;
    public GameObject downloadOption;
    public GameObject uploadOption;
    public Transform gauge;
    public LivingBeing accessUser;
    public PlayerController playerController;

    public bool refreshTradeScreen = false;

    public bool transitioning = false;
    // Start is called before the first frame update
    void Start()
    {
        tradeButton.GetComponent<MenuButton>().setActionListener(this);
        /*
        getResource = animator.transform.parent.parent.parent.GetComponent<PlayerController>().visitingTransferCenter.resources;
        getInventory = animator.transform.parent.parent.parent.GetComponent<LivingBeing>().inventory;
        */
    }

    public ButtonListenerInterface getActionListener()
    {
        return this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void listener(string getAction)
    {
        string[] actionParse = getAction.Split(' ');
        switch (actionParse[0])
        {
            case "Trade":
                if (downloadWeight <= uploadWeight && uploadWeight != 0)
                {
                    //                    
                    //Restock the Node so the item doesn't disappear upon exiting screen
                    foreach (KeyValuePair<string, GameObject> getDict in downloading)
                    { 
                        print(getDict.Key);
                        if (!accessUser.inventory.ContainsKey(getDict.Key))
                        {
                            accessUser.inventory.Add(getDict.Key, 0);
                        }
                        accessUser.inventory[getDict.Key] += getDict.Value.transform.GetChild(0).transform.GetComponent<MenuSelection>().amount;
                        getDict.Value.transform.GetChild(0).GetComponent<Animator>().SetBool("Active", false);
                    }
                    downloading.Clear();

                    //Restock the player so the item doesn't disappear upon exiting screen
                    foreach (KeyValuePair<string, GameObject> getDict in uploading)
                    {
                        if (!playerController.visitingTransferCenter.resources.ContainsKey(getDict.Key))
                        {
                            playerController.visitingTransferCenter.resources.Add(getDict.Key, 0);
                        }
                        playerController.visitingTransferCenter.resources[getDict.Key] += getDict.Value.transform.GetChild(0).transform.GetComponent<MenuSelection>().amount;
                        getDict.Value.transform.GetChild(0).GetComponent<Animator>().SetBool("Active", false);
                    }
                        uploading.Clear();
                    refreshTradeScreen = true;
                }
                break;
            case "Build":
//                playerController.accessBuilding(true);
                playerController.startBuilding(getAction.Replace("Build ", ""), true);
//                playerController.buildModeSwitch();
                break;
        }

    }
    //void menuTransitionBegin()
    //{
    //    print("Transition B: " + transitioning);
    //    transitioning = true;
    //}

    void menuTransitioningEnd()
    {
        GetComponent<Animator>().SetBool("Transitioning", false);
    }

    //void downloadOptionEnd()
    //{
    //    foreach (Transform GO in displayScreen)
    //    {
    //        GO.GetChild(0).GetComponent<Animator>().SetBool("Active", false);
    //    }
    //}

    void atShopCheck()
    {
        if (GetComponent<Animator>().GetBool("atShop"))
        {
            downloadOption.SetActive(true);
            uploadOption.SetActive(true);
        } else
        {
            downloadOption.SetActive(false);
            uploadOption.SetActive(false);
        }
    }
}
