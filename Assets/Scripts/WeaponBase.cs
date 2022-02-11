using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [SerializeField]
    private Animator attackAnimator;
    private int damage = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void damageTrigger(Collider other)
    {
        if (attackAnimator.GetBool("isAttacking"))
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            IDamagable target = null;
            if (other.transform.parent != null) target = other.transform.parent.transform.GetComponent<IDamagable>();
            if (target == null) target = other.transform.GetComponent<IDamagable>();

            if (target != null)
            {
                print(target);
                if (other.transform.TryGetComponent<IShields>(out IShields defendable))
                {
                    print(defendable + " defending ");
                    defendable.defend(damage);
                } else
                {
                    print(target + " Damage ");
                    target.isDamage(true, damage, gameObject);
                }
            }
            if (!other.transform.GetComponent<NodeCollision>())
            {
                attackAnimator.SetBool("isAttacking", false);
//                durabilityDamage(-5);
            }
        }
    }
}
