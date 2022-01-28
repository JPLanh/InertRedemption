using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingBeingCollisionBlocker : MonoBehaviour
{
    public Collider livingBeingCollider;
    public Collider livingBeingBlockerCollider;

    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreCollision(livingBeingCollider, livingBeingBlockerCollider, true);
    }

}
