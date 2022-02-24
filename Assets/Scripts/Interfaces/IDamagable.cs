using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public bool isDamage(bool network, float getValue, GameObject attacker);
}