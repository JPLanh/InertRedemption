using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDisplay : MonoBehaviour, Displayable
{
    public Entity entity;
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
        string disp = "Bit \n";
        disp += "Team: " + entity.team.GetTeam() + "\n";
        disp += "Health: " + livingBeing.health;

        return disp;
    }
}
