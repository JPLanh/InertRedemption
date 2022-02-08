using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damagable
{
    public GameObject isDamage(bool network, float getValue, GameObject attacker);
}