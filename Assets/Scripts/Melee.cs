using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Melee : MonoBehaviour, UsableItemInterface, WeaponBaseInterface, IConsoleOption, IEquipment
{

    public float swingSpeed;
    public float swingRate;
    public float nextTimeToSwing;
    public bool paused = false;
    public bool disabled = false;

    public LivingBeing owner;
    public GameObject blade;
    [SerializeField]
    private Text displayText;

    public WeaponBatteryAddon batteryAddon;
    public WeaponBladeAddon bladeAddon;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Mapper
    public void fireOne()
    {
        swing();
    }

    public void fireTwo()
    {
        block();
    }
    public void reload()
    {

    }

    public void setOwner(LivingBeing getOwner)
    {
        owner = getOwner;
    }

    public bool isUsable()
    {
        return true;
    }

    public void accessMenu(bool flag)
    {

    }
    public void swapItem(bool getBool)
    {
        if (getBool) owner.handAnimator.runtimeAnimatorController = Resources.Load("Bot/Animations/Hands/2 Handed Melee/Bot_Hand") as RuntimeAnimatorController;
        else owner.handAnimator.SetBool("isAttacking", false);

    }
    #endregion

    #region action
    private void swing()
    {
        if (!owner.handAnimator.GetBool("isAttacking") && owner.handAnimator.GetBool("canSwing"))
        {
            if (Time.time >= nextTimeToSwing)
            {
                owner.handAnimator.SetBool("isAttacking", true);
                nextTimeToSwing = Time.time + swingRate;
            }
        }
    }

    private void block()
    {

    }
    #endregion

    public void durabilityDamage(int getVal)
    {
        batteryAddon.charge += getVal;
        displayText.text = StringUtils.convertFloatToString(batteryAddon.charge);
        if (batteryAddon.charge <= 0)
        {
            blade.SetActive(false);
        }
    }

    public int rechargeDurability(int getVal)
    {
        int rechargeAmt = batteryAddon.recharge(getVal);
        displayText.text = StringUtils.convertFloatToString(batteryAddon.charge);
        if (batteryAddon.charge > 0)
        {
            blade.SetActive(true);
        }
        return rechargeAmt;
    }
    public void damageTrigger(Collider other)
    {
        if (owner.handAnimator.GetBool("isAttacking"))
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            Damagable target = null;
            if (other.transform.parent != null) target = other.transform.parent.transform.GetComponent<Damagable>();
            if (target == null) target = other.transform.GetComponent<Damagable>();

            if (target != null)
            {
                target.isDamage(true, bladeAddon.damage, owner.gameObject);
                owner.handAnimator.SetBool("isAttacking", false);
                durabilityDamage(-5);
            }
        }
    }

    public float getEnergy()
    {
        return batteryAddon.charge;
    }
    public List<IAddon> getAllAddons()
    {
        List<IAddon> listOfAddons = new List<IAddon>();
        listOfAddons.Add(bladeAddon);
        listOfAddons.Add(batteryAddon);
        //listOfAddons.Add(chamberAddon);
        //listOfAddons.Add(muzzleAddon);
        //listOfAddons.Add(magazineAddon);
        return listOfAddons;
    }
}
