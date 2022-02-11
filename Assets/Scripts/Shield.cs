using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IShields, IDamagable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void defend(float damage)
    {
        print("Shield has been damaged");
    }

    public GameObject isDamage(bool network, float damage, GameObject attacker)
    {
        print("Damage shield taken");
        return null;
    }
}
