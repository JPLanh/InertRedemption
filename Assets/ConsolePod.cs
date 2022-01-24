using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsolePod : MonoBehaviour
{
    public string action;
    public Survivors team;
    public Text monitorText;
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkMain.Team.Equals("Survivor"))
            team.addMarkedTarget(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateMonitor(string in_string)
    {
        monitorText.text = in_string;
    }
}
