using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public GameObject isDamage(bool network, float getValue, GameObject attacker);
}