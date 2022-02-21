using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visor : MonoBehaviour, IVisor, IEquipment
{
    [SerializeField]
    private string partName;

    public BodyFlashLightAddon lightSource;
    public string getName()
    {
        return partName;
    }
    public void toggleLightSource()
    {
        lightSource.toggle();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<IAddon> getAllAddons()
    {
        List<IAddon> listOfAddons = new List<IAddon>();
        //listOfAddons.Add(chamberAddon);
        //listOfAddons.Add(muzzleAddon);
        //listOfAddons.Add(magazineAddon);
        return listOfAddons;
    }
}
