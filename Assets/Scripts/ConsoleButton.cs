using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConsoleButton : MonoBehaviour
{
    [SerializeField]
    private Text buttonName;
    private string action;
    public ButtonListenerInterface actionListener;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void click()
    {
        actionListener.listener(action);
    }

    public void setButton(string name, string getAction)
    {
        buttonName.text = name;
        action = getAction;
    }
}
