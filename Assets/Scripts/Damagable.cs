using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damagable
{
    public GameObject damage(bool network, float getValue, GameObject attacker);
}