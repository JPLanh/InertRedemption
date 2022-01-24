using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour, IArmor, IEquipment
{
    [SerializeField]
    private string partName;
    public GameObject components;

    public string getName()
    {
        return partName;
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
