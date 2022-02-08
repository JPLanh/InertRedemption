using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponDamagePart : MonoBehaviour
{
    public GameObject baseWeapon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (baseWeapon.TryGetComponent<WeaponBaseInterface>(out WeaponBaseInterface wbi))
        {
            wbi.damageTrigger(other);
        }
    }
}
