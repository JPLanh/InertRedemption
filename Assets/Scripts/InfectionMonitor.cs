using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfectionMonitor : MonoBehaviour
{
    public Text monitorText;

    // Start is called before the first frame update
    void Start()
    {
        EntityManager.infectionMonitor = monitorText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
