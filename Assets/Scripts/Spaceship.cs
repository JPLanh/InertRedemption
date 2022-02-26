using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spaceship : MonoBehaviour
{
    public Dictionary<string, int> resources = new Dictionary<string, int>();
    public Dictionary<string, int> requirement = new Dictionary<string, int>();
    [SerializeField] Text resourceMonitorText;

    public ButtonScript disinfectionButton;

    // Start is called before the first frame update
    void Start()
    {
        //resources.Add("Log", 0);
        //resources.Add("Stone", 0);
        requirement.Add("Log", 2);
        requirement.Add("Stone", 1);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addResource(string in_resource, int in_amount)
    {
        resources[in_resource] += in_amount;
        resourceMonitorText.text = "";
        foreach (KeyValuePair<string, int> it_reources in resources)
        {
            resourceMonitorText.text += $"{it_reources.Key}: {it_reources.Value} / {requirement[it_reources.Key]}\n";
        }
    }
}
