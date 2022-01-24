using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuBehavior : StateMachineBehaviour, ButtonListenerInterface
{

    private float nextUpdate = 0f;
    Dictionary<string, int> getResource;
    Dictionary<string, int> getInventory;
    private UserProjection parentScreen;

    //    OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        parentScreen = animator.transform.GetComponent<UserProjection>();
        parentScreen.uploadWeight = 0;
        parentScreen.downloadWeight = 0;
        reweight();
        getResource = parentScreen.playerController.visitingTransferCenter.resources;
        getInventory = parentScreen.accessUser.inventory;
        parentScreen.downloading = new Dictionary<string, GameObject>();
        parentScreen.uploading = new Dictionary<string, GameObject>();
        initWindow("Download");
        initWindow("Upload");
    }

    public void listener(string getAction)
    {
        string parseAction = getAction.Split(' ')[0];
        string parseParam = getAction.Replace(parseAction + " ", "");


        switch (parseAction)
        {
            case "Download":
                if (!parentScreen.downloading.ContainsKey(parseParam))
                {
                    createNewButton(parseParam, "Downloading", parentScreen.downloading);
                }
                parentScreen.downloading[parseParam].transform.GetChild(0).transform.GetComponent<MenuSelection>().addItemValue(parseParam, 1);

                getResource[parseParam] -= 1;
                parentScreen.downloadWeight += items.valueOf(parseParam);
                if (getResource[parseParam] < 1)
                {
                    getResource.Remove(parseParam);
                    refreshScreen(parseParam, parentScreen.download);
                }
                break;
            case "Downloading":
                if (!getResource.ContainsKey(parseParam))
                {
                    getResource.Add(parseParam, 0);
                }
                getResource[parseParam] += 1;
                parentScreen.downloadWeight -= items.valueOf(parseParam);

                if (parentScreen.downloading[parseParam].transform.GetChild(0).transform.GetComponent<MenuSelection>().addItemValue(parseParam, -1))
                {
                refreshScreen(parseParam, parentScreen.downloading);
                }
                break;
            case "Upload":
                if (!parentScreen.uploading.ContainsKey(parseParam))
                {
                    createNewButton(parseParam, "Uploading", parentScreen.uploading);
                }
                parentScreen.uploading[parseParam].transform.GetChild(0).transform.GetComponent<MenuSelection>().addItemValue(parseParam, 1);

                getInventory[parseParam] -= 1;
                parentScreen.uploadWeight += items.valueOf(parseParam);
                if (getInventory[parseParam] < 1)
                {
                    getInventory.Remove(parseParam);
                    refreshScreen(parseParam, parentScreen.upload);
                }
                break;
            case "Uploading":
                if (!getInventory.ContainsKey(parseParam))
                {
                    getInventory.Add(parseParam, 0);
                }
                getInventory[parseParam] += 1;
                parentScreen.uploadWeight -= items.valueOf(parseParam);

                if (parentScreen.uploading[parseParam].transform.GetChild(0).transform.GetComponent<MenuSelection>().addItemValue(parseParam, -1))
                {
                    refreshScreen(parseParam, parentScreen.uploading);
                }
                break;

        }
        reweight();
    }

    private void refreshScreen(string lastObject, Dictionary<string, GameObject> getList)
    {
        if (getList[lastObject].transform.GetChild(0).transform.GetComponent<MenuSelection>().amount < 1)
        {
            Transform theParent = getList[lastObject].transform.parent;
            int childNum = getList[lastObject].transform.GetSiblingIndex();
            foreach (KeyValuePair<string, GameObject> getDict in getList)
            {
                int childIndex = getDict.Value.transform.GetSiblingIndex();
                if (childIndex > childNum)
                {
                getDict.Value.transform.localPosition = new Vector3(0f, 210f - ((childIndex-1)% 10) * 40f, 0f);
                }
            }
            Destroy(getList[lastObject]);
            getList.Remove(lastObject);

        }
    }
    
    private void initWindow(string getString)
    {
        switch (getString)
        {
            case "Download":
                foreach (Transform child in parentScreen.downloadScreen)
                {
                    Destroy(child.gameObject);
                }
                parentScreen.download = new Dictionary<string, GameObject>();
                foreach (KeyValuePair<string, int> getDict in getResource)
                {
                    createNewButton(getDict.Key, "Download", parentScreen.download);
                }
                break;
            case "Upload":
                foreach (Transform child in parentScreen.uploadScreen)
                {
                    Destroy(child.gameObject);
                }
                parentScreen.upload = new Dictionary<string, GameObject>();
                foreach (KeyValuePair<string, int> getDict in getInventory)
                {
                    createNewButton(getDict.Key, "Upload", parentScreen.upload);
                }
                break;

        }
    }


    private GameObject createNewButton(string text, string getAction, Dictionary<string, GameObject> getList)
    {
        GameObject newMenu = Instantiate(Resources.Load<GameObject>("Menu Selection Button"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        Transform menuObj = newMenu.transform.GetChild(0).transform;
        newMenu.name = text;
        menuObj.GetComponent<MenuSelection>().getButtonText.text = text + "\n" + 0;
        menuObj.GetComponent<MenuSelection>().getButton.GetComponent<MenuButton>().action = getAction + " " + text;
        menuObj.GetComponent<MenuSelection>().actionListener = this;
        menuObj.GetComponent<Animator>().SetBool("Active", true);
        switch (getAction)
        {
            case "Download":
                newMenu.transform.SetParent(parentScreen.downloadScreen);
                break;
            case "Downloading":
                newMenu.transform.SetParent(parentScreen.downloadingScreen);
                break;
            case "Upload":
                newMenu.transform.SetParent(parentScreen.uploadScreen);
                break;
            case "Uploading":
                newMenu.transform.SetParent(parentScreen.uploadingScreen);
                break;
        }
        newMenu.transform.localPosition = new Vector3(0f, 210f - (getList.Count % 10) * 40f, 0f);
        newMenu.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        newMenu.transform.localScale = new Vector3(.6f, .6f, .6f);
        getList.Add(text, newMenu);

        return newMenu;
    }

    private void updateWindow()
    {
        if (parentScreen.download.Count == getResource.Count)
        {
            foreach (KeyValuePair<string, int> getDict in getResource)
            {
                if (parentScreen.download.TryGetValue(getDict.Key, out GameObject ComponentsAmount))
                {
                    ComponentsAmount.transform.GetChild(0).transform.GetComponent<MenuSelection>().updateShopButton(getDict.Key, getDict.Value);
                }
                else
                {
                    initWindow("Download");
                    break;
                }
            }
        }
        else
        {
            initWindow("Download");
        }

        if (parentScreen.upload.Count == getInventory.Count)
        {
            foreach (KeyValuePair<string, int> getDict in getInventory)
            {
                if (parentScreen.upload.TryGetValue(getDict.Key, out GameObject ComponentsAmount))
                {
                    ComponentsAmount.transform.GetChild(0).transform.GetComponent<MenuSelection>().updateShopButton(getDict.Key, getDict.Value);
                }
                else
                {
                    initWindow("Upload");
                    break;
                }
            }
        }
        else
        {
            initWindow("Upload");
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.GetBool("Initialise"))
        {
            if (animator.transform.GetComponent<UserProjection>().downloadScreen.childCount == 0)
            {
                animator.SetBool("Transitioning", true);
            }
        }
        if (!animator.GetBool("Access Download"))
            closeWindow();
        if (Time.time >= nextUpdate)
        {
            nextUpdate = Time.time + 1f / 10;
            updateWindow();
        }
        if (parentScreen.refreshTradeScreen)
        {
            parentScreen.refreshTradeScreen = false;
            initWindow("Download");
            initWindow("Upload");
        }
    }

    private void reweight()
    {

        parentScreen.gauge.localRotation = Quaternion.Slerp(parentScreen.gauge.rotation, Quaternion.Euler(0, 0, parentScreen.downloadWeight - parentScreen.uploadWeight), 2 * Time.time);
        if (parentScreen.downloadWeight > parentScreen.uploadWeight)
        {
            parentScreen.gauge.GetChild(0).GetComponent<Image>().color = Color.red;
        } else if (parentScreen.downloadWeight < parentScreen.uploadWeight)
        {
            parentScreen.gauge.GetChild(0).GetComponent<Image>().color = Color.green;
        } else
        {
            parentScreen.gauge.GetChild(0).GetComponent<Image>().color = Color.white;
        }
    }

    private void closeWindow()
    {
        //Restock the Node so the item doesn't disappear upon exiting screen
        foreach (KeyValuePair<string, GameObject> getDict in parentScreen.downloading)
        {
            if (!getResource.ContainsKey(getDict.Key))
            {
                getResource.Add(getDict.Key, 0);
            }
            getResource[getDict.Key] += getDict.Value.transform.GetChild(0).transform.GetComponent<MenuSelection>().amount;
            getDict.Value.transform.GetChild(0).GetComponent<Animator>().SetBool("Active", false);
        }

        //Restock the player so the item doesn't disappear upon exiting screen
        foreach (KeyValuePair<string, GameObject> getDict in parentScreen.uploading)
        {

            if (!getInventory.ContainsKey(getDict.Key))
            {
                getInventory.Add(getDict.Key, 0);
            }
            getInventory[getDict.Key] += getDict.Value.transform.GetChild(0).transform.GetComponent<MenuSelection>().amount;
            getDict.Value.transform.GetChild(0).GetComponent<Animator>().SetBool("Active", false);
        }

        //Clear all menu respectively
        foreach (KeyValuePair<string, GameObject> getDict in parentScreen.upload)
        {
            getDict.Value.transform.GetChild(0).GetComponent<Animator>().SetBool("Active", false);
        }
        foreach (KeyValuePair<string, GameObject> getDict in parentScreen.download)
        {
            getDict.Value.transform.GetChild(0).GetComponent<Animator>().SetBool("Active", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
