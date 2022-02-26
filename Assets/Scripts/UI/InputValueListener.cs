using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputValueListener : MonoBehaviour
{
    [SerializeField] InputField lv_inputField;
    public int maxValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onValueChange()
    {
        Debug.Log("Value has changed");
        if (!string.IsNullOrEmpty(lv_inputField.text))
        {
            Debug.Log("Is not null");
            Debug.Log(lv_inputField.text);
            if (int.Parse(lv_inputField.text) > maxValue)
            {
                lv_inputField.text = maxValue.ToString();
            }
            else if (int.Parse(lv_inputField.text) < 0)
            {
                lv_inputField.text = "0";
            }
            Debug.Log(lv_inputField.text);
            if (!lv_inputField.text.Equals("0"))
            {
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload["Type"] = "Player Update";
                payload["Action"] = "Player HUD Menu";
                payload["Menu"] = "Update InputField";
                payload["Value"] = lv_inputField.text;
                payload["Name"] = name;
                NetworkMain.broadcastAction(payload);
            }
        }
    }
}
