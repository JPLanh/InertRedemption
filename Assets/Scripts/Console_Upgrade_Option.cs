using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console_Upgrade_Option : MonoBehaviour, ButtonListenerInterface
{
    public Text optionText;
    public Text levelText;
    public GameObject requirements;
    [SerializeField]
    private UIConsoleButton upgradeButton;
    [SerializeField]
    private UIConsoleButton infoButton;
    private IAddon currentIAddon;
    private IInventory currentInventory;
    private ToastNotifications toast;
    private string optionType;
    public ButtonListenerInterface parentListener;

    private Text infoText;
    private Text upgradeText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setOption(IAddon getItem, IInventory getInventory, ToastNotifications getToast, ButtonListenerInterface getListener, string getOptionType, Text getInfoText, Text getUpgradeText)
    {
        optionText.text = getItem.getName();
        levelText.text = StringUtils.convertIntToString(getItem.getLevel());
        currentInventory = getInventory;
        infoText = getInfoText;
        upgradeText = getUpgradeText;

        foreach (KeyValuePair<string, int> getReq in getItem.getRequirements())
        {
            addNewRequirement(getReq.Key, getReq.Value);
        }
        upgradeButton.actionListener = this;
        upgradeButton.setButton("Upgrade", "Upgrade");

        infoButton.actionListener = this;
        infoButton.setButton("Info", "Info");

        currentIAddon = getItem;
        toast = getToast;
        parentListener = getListener;
        optionType = getOptionType;
    }

    public void addNewRequirement(string getItem, int getAmount)
    {
        GameObject tmpRequirement = Instantiate(Resources.Load<GameObject>("Display/Console UI/Requirement"), transform.position, Quaternion.identity);
        tmpRequirement.transform.SetParent(requirements.transform);
        tmpRequirement.GetComponent<RequirementTextScript>().amountText.text = StringUtils.convertIntToString(getAmount);
        if (currentInventory.getInventory().TryGetValue(getItem, out int invAmount))
        {
            tmpRequirement.GetComponent<RequirementTextScript>().heldText.text = StringUtils.convertIntToString(invAmount);
        } else
        {
            tmpRequirement.GetComponent<RequirementTextScript>().heldText.text = "0";
        }
        tmpRequirement.GetComponent<RequirementTextScript>().nameText.text = getItem;
        tmpRequirement.transform.localPosition = new Vector3(40f + 115f*((requirements.transform.childCount-1)%2) , 10f - 11f * ((requirements.transform.childCount - 1) / 2), 2f);
        tmpRequirement.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        tmpRequirement.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void listener(string text)
    {
        switch (text)
        {
            case "Info":
                infoText.text = currentIAddon.getInfo();
                upgradeText.text = currentIAddon.getUpgradeInfo();
                break;
            case "Upgrade":
                bool insufficient = false;
                foreach (KeyValuePair<string, int> getReq in currentIAddon.getRequirements())
                {
                    if (insufficient) break;
                    if (currentInventory.getInventory().TryGetValue(getReq.Key, out int amount))
                    {
                        if (getReq.Value > amount)
                        {
                            insufficient = true;
                        }
                    } else
                    {
                        insufficient = true;
                    }
                }
                if (!insufficient)
                {

                    //TODO: Broadcast the upgrade instead of doing this locally
                    foreach (Transform child in requirements.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    foreach (KeyValuePair<string, int> getReq in currentIAddon.getRequirements())
                    {
                        currentInventory.getInventory()[getReq.Key] -= getReq.Value;
                        currentInventory.modifyAmount(-getReq.Value);
                    }

                    Dictionary<string, string> payload = new Dictionary<string, string>();
                    payload["Type"] = "Player Action";
                    payload["Action"] = "Upgrade";
                    payload["Level"] = StringUtils.convertIntToString(int.Parse(levelText.text) + 1);
                    string[] parsedUpgrade = optionText.text.Split(' ');
                    payload["Type"] = parsedUpgrade[0];
                    payload["Gear"] = optionText.text.Replace(parsedUpgrade[0] + " ", "");
                    payload["Username"] = NetworkMain.Username;
                    NetworkMain.serverAction(payload);
                    currentIAddon.updateLevel(1);

//                    setOption(currentIAddon, currentInventory, toast, parentListener);
                    toast.newNotification(currentIAddon.getName() + " has been upgraded");
                    parentListener.listener("Select " + optionType);
                } else
                {
                    toast.newNotification("Insufficient materials to upgrade " + currentIAddon.getName());
                }
                break;
        }
    }
}
