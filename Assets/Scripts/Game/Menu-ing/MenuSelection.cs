using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSelection : MonoBehaviour
{
    public Button getButton;
    public MenuButton lv_buttonScript;
    public Text getButtonText;
    public Text getAmountText;
    public bool shopItem = false;
    public int amount = 0;
    public LivingBeing accessUser;
    public TransferCenter transferCentre;
    public ButtonListenerInterface actionListener;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
    }

    public void onTransitionExit()
    {
        GetComponent<Animator>().SetBool("Transitioning", false);
    }

    public void updateShopButton(string name, int getValue)
    {
        amount = getValue;
        getButtonText.text = name + "\n" + amount;
    }

    public bool addItemValue(string getName, int getValue)
    {
        amount += getValue;
        getButtonText.text = getName + "\n" + amount;
        return getValue != 0;
    }
}
