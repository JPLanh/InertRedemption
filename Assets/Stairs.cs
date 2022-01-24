using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    public GameObject steps;
    public Base team;
    // Start is called before the first frame update
    void Start()
    {
        for (int stepCounter = 0; stepCounter < 10; stepCounter++)
        {
            GameObject stairStep = null;
                stairStep = Instantiate(steps, this.transform.position + new Vector3(7.5f + 1f * (stepCounter), -1f * (stepCounter), 7.5f + 1f * (stepCounter)), Quaternion.identity);
                stairStep.transform.localScale = new Vector3(20f + (stepCounter * 2f), 1f, 20.0f + (stepCounter*2f));
            stairStep.name = "Step " + stepCounter;
            stairStep.transform.SetParent(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void activate()
    {

    }
}
