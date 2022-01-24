using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    public PlayerController playerController;
    public LivingBeing livingBeing;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string display()
    {
        string str = playerController.transform.parent.gameObject.name + "\n" +
            "Health: " + livingBeing.health;
        return str;
    }
}
