using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHubUI : MonoBehaviour
{
    public PlayerController lv_playerController;
    public VirusController lv_virusController;
    private string state;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (state.Split(' ')[0].Equals("Transfer"))
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload["Type"] = "Player Update";
                payload["Action"] = "Player HUD Menu";
                payload["Menu"] = state;
                NetworkMain.broadcastAction(payload);
            }
        }
    }


    public void init(PlayerController in_player)
    {
        lv_playerController = in_player;
        initMenu("Survivor");
    }

    public void init(VirusController in_player)
    {
        lv_virusController = in_player;
        initMenu("Virus");
    }

    public void listen(Payload in_payload)
    {
        string[] parsedAction = in_payload.data["Menu"].Split(' ');
        switch (parsedAction[0])
        {
            case "Ability":
                switch (parsedAction[1])
                {
                    case "Virus":
                        lv_virusController.selectAbility(in_payload.data["Menu"].Replace($"{parsedAction[0]} {parsedAction[1]} ", ""));
                        break;
                    case "Survivor":
//                        lv_virusController.selectAbility(in_payload.data["Menu"].Replace($"{parsedAction[0]} {parsedAction[1] }", ""));
                        break;
                }
                break;
            case "Upgrade":
                switch (parsedAction[1])
                {
                    case "Virus":
                        if(NetworkMain.Username.Equals(lv_virusController.name))
                            lv_virusController.upgradeAbility(in_payload.data["Menu"].Replace($"{parsedAction[0]} {parsedAction[1]} {parsedAction[2]} ", ""), int.Parse(parsedAction[2]));
                        hud_VirusMutation();
                        break;
                }
                break;
            case "Transfer":
                string lv_transfer_item = in_payload.data["Menu"].Replace(parsedAction[0] + " ", "").Replace(parsedAction[1] + " ", "").Replace(parsedAction[2] + " ", "")
                    .Replace(parsedAction[3] + " ", "").Replace(parsedAction[4] + " ", "");
                transfer(parsedAction[2], parsedAction[4], lv_transfer_item);
                
                // $"Transfer From {in_from_UID} To {in_to_UID} {in_item}"
                break;
            case "Ship":
                switch (parsedAction[1])
                {
                    case "Select":
                        string lv_item = in_payload.data["Menu"].Replace($"Ship Select {parsedAction[2]} {parsedAction[3]} " , "");
                        transferItem(parsedAction[2], parsedAction[3], lv_item);
                        break;
                    case "Donation":
                        initShipDonation();
                        break;
                }
                break;
            default:
                switch (in_payload.data["Menu"])
                {
                    case "Update InputField":
                        EntityManager.inputFields.TryGetValue(in_payload.data["Name"], out InputField out_if);
                        if (out_if != null)
                        {
                            out_if.text = in_payload.data["Value"];
                        }
                        break;
                    case "Player Status":
                        //                hudDisplay.initPlayerStatus();
                        break;
                    case "Ship Status":
                        //                hudDisplay.initShipStatus();
                        break;
                    case "Player Gears":
                        //                hudDisplay.initPlayerGears();
                        break;
                    case "Main Menu":
                        initMenu(in_payload.data["Team"]);
                        break;
                    case "Virus Abilities":
                        hud_VirusAbilities();
                        break;
                    case "Virus Mutation":
                        hud_VirusMutation();
//                        Debug.Log("Mutation");
//                        subDisplayMenu tets = Instantiate(Resources.Load<subDisplayMenu>("UI/Display Info"), transform);
                        //
                        break;
                }
                break;
        }
    }

    private void hud_VirusAbilities()
    {
        closeMenu();
        loadVerticalPanel(out GameObject out_panel, out RectTransform out_rect, 0, AnchorPresets.StretchAll, PivotPresets.MiddleCenter);
        createText("", out_rect, new Vector3(0f, -375f, 0f), out UIText hint_text);
        hint_text.textField.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 60);
        createNewUpgradeButton("Replenishment", "Ability Virus Replenishment", new Vector3(-500f, 300f, 0f), out_panel.transform, out HUDButton out_test_1);
        out_test_1.hintText = hint_text.textField;
        out_test_1.hint = $"Replenishment (Level: {lv_virusController.upgrades.replenishment_level})";

        createNewUpgradeButton("Resource Trap", "Ability Virus Resource Trap", new Vector3(-350f, 300f, 0f), out_panel.transform, out HUDButton out_test_2);
        out_test_2.hintText = hint_text.textField;
        out_test_2.hint = $"Resource Trap (Level: {lv_virusController.upgrades.trapping_level})";

        createNewUpgradeButton("Consume Host", "Ability Virus Consume Host", new Vector3(-200f, 300f, 0f), out_panel.transform, out HUDButton out_test_3);
        out_test_3.hintText = hint_text.textField;
        out_test_3.hint = $"Consume Host (Level: {lv_virusController.upgrades.consumeHost_level})";
    }

    private void hud_VirusMutation()
    {
        closeMenu();
        loadVerticalPanel(out GameObject out_panel, out RectTransform out_rect, 0, AnchorPresets.StretchAll, PivotPresets.MiddleCenter);
        createText("", out_rect, new Vector3(0f, -375f, 0f), out UIText hint_text);
        hint_text.textField.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 60);
        createNewUpgradeButton("Replenishment", "Upgrade Virus 1 Replenishment", new Vector3(0f, 0f, 0f), out_panel.transform, out HUDButton out_test_1);
        out_test_1.hintText = hint_text.textField;
        out_test_1.hint = $"Replenishment (Level: {lv_virusController.upgrades.replenishment_level})" + "\n\nRequirements:\n";
        foreach(KeyValuePair<string, int> it_upgrades in lv_virusController.upgrades.getRequirement("Replenishment"))
        {
            out_test_1.hint += $"{it_upgrades.Key}: {lv_virusController.infectionPoint} / {it_upgrades.Value}" + "\n";
        }

        createNewUpgradeButton("Resource Trap", "Upgrade Virus 1 Resource Trap", new Vector3(0f, 150f, 0f), out_panel.transform, out HUDButton out_test_2);
        out_test_2.hintText = hint_text.textField;
        out_test_2.hint = $"Resource Trap (Level: {lv_virusController.upgrades.trapping_level})" + "\n\nRequirements:\n";
        foreach (KeyValuePair<string, int> it_upgrades in lv_virusController.upgrades.getRequirement("Resource Trap"))
        {
            out_test_2.hint += $"{it_upgrades.Key}: {lv_virusController.infectionPoint} / {it_upgrades.Value}" + "\n";
        }

        createNewUpgradeButton("Consume Host", "Upgrade Virus 1 Consume Host", new Vector3(0f, 300f, 0f), out_panel.transform, out HUDButton out_test_3);
        out_test_3.hintText = hint_text.textField;
        out_test_3.hint = $"Consume Host (Level: {lv_virusController.upgrades.consumeHost_level})" + "\n\nRequirements:\n";
        foreach (KeyValuePair<string, int> it_upgrades in lv_virusController.upgrades.getRequirement("Consume Host"))
        {
            out_test_3.hint += $"{it_upgrades.Key}: {lv_virusController.infectionPoint} / {it_upgrades.Value}" + "\n";
        }

    }

    private void transfer(string in_from_uid, string in_to_uid, string in_item)
    {

        EntityManager.players.TryGetValue(in_from_uid, out IPlayerController out_from);
        EntityManager.players.TryGetValue(in_to_uid, out IPlayerController out_to);

        bool itemExhausted = false;

        if (!string.IsNullOrEmpty(EntityManager.inputFields[lv_playerController.name + " Left Transfer Amount"].text))
        {
            int in_amount = int.Parse(EntityManager.inputFields[lv_playerController.name + " Left Transfer Amount"].text);
            if (out_from != null)
            {
                itemExhausted = out_from.getInventory().useItem(in_item, in_amount);
                //            out_from.getInventory().getInventory()[in_item] -= in_amount;
            }
            else
            {
                lv_playerController.survivorsGO.spaceship.resources[in_item] -= in_amount;
                itemExhausted = false;
                if (lv_playerController.survivorsGO.spaceship.resources[in_item] <= 0)
                {
                    lv_playerController.survivorsGO.spaceship.resources.Remove(in_item);
                    itemExhausted = true;
                }
            }

            if (out_to != null)
            {
                out_to.getInventory().recieveItem(in_item, in_amount);
            }
            else
            {
                if (!lv_playerController.survivorsGO.spaceship.resources.ContainsKey(in_item))
                {
                    lv_playerController.survivorsGO.spaceship.resources.Add(in_item, 0);
                }
                lv_playerController.survivorsGO.spaceship.resources[in_item] += in_amount;
            }

            if (itemExhausted)
            {
                initShipDonation();
            }
            else
            {
                transferItem(in_from_uid, in_to_uid, in_item);
            }
        }
    }


    public void transferItem(string in_from_UID, string in_to_UID, string in_item)
    {
        closeMenu();

        loadVerticalPanel(out GameObject left_panel, out RectTransform out_left_rect, -400, AnchorPresets.VertStretchLeft, PivotPresets.MiddleLeft);
        loadVerticalPanel(out GameObject right_panel, out RectTransform out_right_panel, 400, AnchorPresets.VertStretchRight, PivotPresets.MiddleRight);

        EntityManager.players.TryGetValue(in_from_UID, out IPlayerController out_from);
        EntityManager.players.TryGetValue(in_to_UID, out IPlayerController out_to);

        int lv_from_value = 0;
        if (out_from != null)
        {
            out_from.getInventory().getInventory().TryGetValue(in_item, out lv_from_value);
        } else
        {
            lv_playerController.survivorsGO.spaceship.resources.TryGetValue(in_item, out lv_from_value);
        }

        int lv_to_value = 0;
        if (out_to != null)
        {
            out_to.getInventory().getInventory().TryGetValue(in_item, out lv_to_value);
        }
        else
        {
            lv_playerController.survivorsGO.spaceship.resources.TryGetValue(in_item, out lv_to_value);
        }


        createText(out_from == null ? "Ship" : out_from.getGameObject().name, left_panel.transform, new Vector3(200f, 280f, 0f));
        createText(in_item, left_panel.transform, new Vector3(200f, 200f, 0f));
        createInputField(lv_playerController.name + " Left Transfer Amount", new Vector3(-35f, 150f, 0f), lv_from_value, left_panel.transform, true);
        createText($"/ {lv_from_value}" , left_panel.transform, new Vector3(300, 150f, 0f), out UIText lv_from_text);
        lv_from_text.textField.alignment = TextAnchor.MiddleLeft;

        createNewButton("Transfer", $"Transfer From {in_from_UID} To {in_to_UID} {in_item}", new Vector3(200f, 100f, 0f), left_panel.transform);
        state = $"Transfer From {in_from_UID} To {in_to_UID} {in_item}";

        createText(out_to == null ? "Ship" : out_to.getGameObject().name, right_panel.transform, new Vector3(-200f, 280f, 0f));
        createText(in_item, right_panel.transform, new Vector3(-200, 200f, 0f));
        createText($"{lv_to_value}", right_panel.transform, new Vector3(-200, 150f, 0f));

        createNewButton("Back", $"Ship Donation", new Vector3(0f, -325f, 0f), transform);
    }

    public void closeMenu()
    {
        foreach(KeyValuePair<string, InputField> it_if in EntityManager.inputFields)
        {
            Destroy(it_if.Value.gameObject);
        }
        EntityManager.inputFields.Clear();

        foreach (Transform it_child in transform)
        {
            Destroy(it_child.gameObject);
        }
        state = "";
    }
    private void initMenu(string in_team)
    {
        closeMenu();
        loadVerticalPanel(out GameObject left_panel, out RectTransform out_rect_panel, -400, AnchorPresets.VertStretchLeft, PivotPresets.MiddleLeft);
        switch (in_team)
        {
            case "Survivor":
                initMenu_Survivors(left_panel.transform);
                break;
            case "Virus":
                initMenu_Virus(left_panel.transform);
                break;
        }

    }

    private void initMenu_Survivors(Transform in_panel)
    {
        addMainMenuButton($"Status", $"Player Status", in_panel);
        addMainMenuButton($"Gear", $"Player Gears", in_panel);
        addMainMenuButton($"Ship Status", $"Ship Status", in_panel);
        if (lv_playerController.inShip) addMainMenuButton($"Ship Donation", $"Ship Donation", in_panel);
    }

    private void initMenu_Virus(Transform in_panel)
    {
        addMainMenuButton($"Status", $"Virus Status", in_panel);
        addMainMenuButton($"Mutations", $"Virus Mutation", in_panel);
        addMainMenuButton($"Abilities", $"Virus Abilities", in_panel);
    }

    public void initShipDonation()
    {
        closeMenu();
        loadVerticalPanel(out GameObject left_panel, out RectTransform out_left_rect, -400, AnchorPresets.VertStretchLeft, PivotPresets.MiddleLeft);
        loadVerticalPanel(out GameObject right_panel, out RectTransform out_right_rect, 400, AnchorPresets.VertStretchRight, PivotPresets.MiddleRight);

        createText("Item", left_panel.transform, new Vector3(150f, 280f, 0f));
        createText("Amount", left_panel.transform, new Vector3(350f, 280f, 0f));
        int counter = 0;
        foreach(KeyValuePair<string, int> it_item in lv_playerController.inventory.getInventory())
        {
            createNewButton($"{it_item.Key}", $"Ship Select {lv_playerController.userID} Ship {it_item.Key}", new Vector3(145, 245f - (counter * 60), 0f), left_panel.transform);
            createText($"{it_item.Value}", left_panel.transform, new Vector3(350f, 245 - (counter * 60), 0f));
            counter++;
        }

        createText("Item", right_panel.transform, new Vector3(-245, 280f, 0f));
        createText("Amount", right_panel.transform, new Vector3(-50f, 280f, 0f));
        counter = 0;
        foreach (KeyValuePair<string, int> it_item in lv_playerController.survivorsGO.spaceship.resources)
        {
            createNewButton($"{it_item.Key}", $"Ship Select Ship {lv_playerController.userID} {it_item.Key}", new Vector3(-250, 245f - (counter * 60), 0f), right_panel.transform);
            createText($"{it_item.Value}", right_panel.transform, new Vector3(-50, 245 - (counter * 60), 0f));
            counter++;
        }

        createNewButton("Back", "Main Menu", new Vector3(0f, -325f, 0f), transform);
    }

    private void addMainMenuButton(string in_text, string in_action, Transform in_parent)
    {
        createNewButton(in_text, in_action, new Vector3(200, 245f - (in_parent.childCount * 60), 0f), in_parent);
    }

    private void loadVerticalPanel(out GameObject out_verticalPanel, out RectTransform out_rectTransform, int in_xOffset, AnchorPresets in_present, PivotPresets in_pivot)
    {
        out_verticalPanel = Instantiate(Resources.Load<GameObject>("UI/Vertical Panel"), transform);
        out_verticalPanel.TryGetComponent<RectTransform>(out out_rectTransform);
        out_rectTransform.offsetMin = new Vector2(out_rectTransform.offsetMin.x, 0);
        out_rectTransform.offsetMax = new Vector2(out_rectTransform.offsetMax.x, 0);
        RectTransformUtils.SetAnchor(out_rectTransform, in_present, in_xOffset);
        RectTransformUtils.SetPivot(out_rectTransform, in_pivot);     
    }


    private void createNewButton(string text, string getAction, Vector3 position, Transform in_parent)
    {
        GameObject newMenu = Instantiate(Resources.Load<GameObject>("UI/Menu Selection"), in_parent);
        newMenu.TryGetComponent<HUDButton>(out HUDButton out_buttonScript);
        out_buttonScript.buttonText.text = text;
        out_buttonScript.action = getAction;
        newMenu.transform.localPosition = position;
        newMenu.name = text;
    }

    private void createNewUpgradeButton(string text, string getAction, Vector3 position, Transform in_parent, out HUDButton out_button)
    {
        GameObject newMenu = Instantiate(Resources.Load<GameObject>("UI/Upgrade_Button"), in_parent);
        newMenu.TryGetComponent<HUDButton>(out out_button);
        //        out_buttonScript.buttonText.text = text;
        out_button.action = getAction;
        newMenu.transform.localPosition = position;
        newMenu.name = text;
    }

    private void createText(string getString, Transform in_transform, Vector3 getPos)
    {
        GameObject tmpText = Instantiate(Resources.Load<GameObject>("UI/Text"), in_transform);
        tmpText.GetComponent<UIText>().textField.text = getString;
        tmpText.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        tmpText.transform.localScale = new Vector3(1f, 1f, 1f);
        tmpText.transform.localPosition = getPos;
    }

    private void createText(string getString, Transform in_transform, Vector3 getPos, out UIText out_text)
    {
        GameObject tmpText = Instantiate(Resources.Load<GameObject>("UI/Text"), in_transform);
        tmpText.TryGetComponent<UIText>(out out_text);
        out_text.textField.fontSize = 30;
        out_text.textField.text = getString;
        tmpText.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        tmpText.transform.localScale = new Vector3(1f, 1f, 1f);
        tmpText.transform.localPosition = getPos;        
    }

    private void createInputField(string in_name, Vector3 in_pos, int in_max_value, Transform in_parent, bool in_selected)
    {
        GameObject tmpText = Instantiate(Resources.Load<GameObject>("UI/Input Field"), in_parent);
        tmpText.TryGetComponent<InputField>(out InputField out_IF);
        tmpText.name = in_name;
        tmpText.TryGetComponent<InputValueListener>(out InputValueListener out_IVL);
        out_IVL.maxValue = in_max_value;
        EntityManager.inputFields.Add(in_name, out_IF);
        tmpText.TryGetComponent<RectTransform>(out RectTransform out_rect);
        if (in_selected) out_IF.Select();
        out_rect.anchoredPosition = in_pos;
    }
}
