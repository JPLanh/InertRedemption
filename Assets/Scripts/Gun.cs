using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Socket.Newtonsoft.Json;
using Socket.Newtonsoft.Json.Linq;

public class Gun : MonoBehaviour, Interactable, UsableItemInterface, IConsoleOption, IEquipment
{
    public ParticleSystem muzzleFlash;
    public Text ammoCounter;
    public bool isReloading;
    public bool disabled = false;
    public LivingBeing owner;

    public WeaponScopeAddon scopeAddon;
    public WeaponBarrelAddon muzzleAddon;
    public WeaponMagazineAddon magazineAddon;
    public WeaponBatteryAddon batteryAddon;
    public WeaponChamberAddon chamberAddon;

    public Quaternion rotateTowards = Quaternion.Euler(45f, 0, 45f);
    // Start is called before the first frame update

    void Start()
    {
        magazineAddon.ammo = 50;
        isReloading = false;
        //rotateTowards = Quaternion.Euler(45f, 0, 45f);
    }

    public void initGun()
    {
        displayAmmo();
    }

    // Update is called once per frame
    void Update(){}

    public void displayAmmo()
    {
        if (magazineAddon == null)
        {
            ammoCounter.color = new Color(255f / 255f, 0, 0, 255f / 255f);
            ammoCounter.text = "No Clip";

        } else
        {
        if (magazineAddon.ammo != 0)
        {
            ammoCounter.color = new Color(0f / 255f, 0f / 255f, 255f / 255f, 255f / 255f);
            ammoCounter.text = "Ammo\n" + magazineAddon.ammo + " / " + magazineAddon.maxAmmo + "\n";
                ammoCounter.text += "Energy\n" + batteryAddon.charge + " / " + batteryAddon.maxCharge;

            }
            else
        {
            ammoCounter.color = new Color(255f / 255f, 0, 0, 255f / 255f);
            ammoCounter.text = "RELOAD";
        }
        }
        if (batteryAddon.charge <= 0)
        {
            ammoCounter.color = new Color(255f / 255f, 0, 0, 255f / 255f);
            ammoCounter.text = "NO ENERGY";

        }
    }

    #region Mapper
    public void fireOne()
    {
        shoot();
    }

    public void fireTwo()
    {
        aim();
    }

    public void reload()
    {
        startReload();
    }

    public bool isUsable()
    {
        return magazineAddon.ammo > 0;
    }
    #endregion

    #region Actions
    public void shoot()
    {
        if (magazineAddon != null)
        {
            if (Time.time >= chamberAddon.nextTimeToFire)
            {
                chamberAddon.nextTimeToFire = Time.time + 1f / chamberAddon.fireRate;
                if (magazineAddon.ammo >= chamberAddon.ammoConsumption && batteryAddon.charge > 0)
                {
                    muzzleFlash.Play();
                    displayAmmo();
                    magazineAddon.ammo -= chamberAddon.ammoConsumption;
                    durabilityDamage(-1);

                    GameObject lazurBeem = Instantiate(Resources.Load<GameObject>("Laser Beam"), muzzleFlash.transform.position, muzzleFlash.transform.rotation);
                    lazurBeem.GetComponent<Projectile>().setProjectile(muzzleAddon.projectileSpeed, muzzleAddon.projectileRange, chamberAddon.damage, magazineAddon.damageType);
                    lazurBeem.GetComponent<Projectile>().attacker = owner.gameObject;
                }
            }
        }
    }


    public void durabilityDamage(int getVal)
    {
        batteryAddon.charge += getVal;
        if (batteryAddon.charge <= 0)
        {
            //blade.SetActive(false);
        }
    }

    public void aim() {
        owner.GetComponent<PlayerController>().aim();
    }

    public void startReload()
    {
        if (!disabled) StartCoroutine(reloading());
    }

    #endregion

    public IEnumerator reloading()
    {
        owner.setAnimation("isReloading", true);
        disabled = true;
        float reloadTimer = Time.time;

        int curAmmo = magazineAddon.ammo;
        yield return new WaitForSeconds(magazineAddon.reloadSpeed);
        if (owner.handAnimator.GetComponent<Animator>().GetBool("isReloading"))
        {
            if (!owner.GetComponent<PlayerController>().inventory.getAmmo().ContainsKey("Ammo"))
            {
                //magazineAddon.ammo = curAmmo;
                //owner.GetComponent<PlayerController>().inventory.modifyAmount(-curAmmo);
                //owner.GetComponent<PlayerController>().inventory.getAmmo()["Ammo"] -= (magazineAddon.maxAmmo - curAmmo);

            } else
            {
                if (owner.GetComponent<PlayerController>().inventory.getAmmo()["Ammo"]+ magazineAddon.ammo < magazineAddon.maxAmmo)
                {
                    magazineAddon.ammo += owner.GetComponent<PlayerController>().inventory.getAmmo()["Ammo"];
                    owner.GetComponent<PlayerController>().inventory.modifyAmount(-owner.GetComponent<PlayerController>().inventory.getAmmo()["Ammo"]);
                    owner.GetComponent<PlayerController>().inventory.getAmmo()["Ammo"] -= owner.GetComponent<PlayerController>().inventory.getAmmo()["Ammo"];
                } else
                {
                    magazineAddon.ammo = magazineAddon.maxAmmo;
                    owner.GetComponent<PlayerController>().inventory.modifyAmount(-magazineAddon.maxAmmo);
                    owner.GetComponent<PlayerController>().inventory.getAmmo()["Ammo"] -= (magazineAddon.maxAmmo - curAmmo);

                }
            }
        }

        displayAmmo();
        disabled = false;
        owner.setAnimation("isReloading", false);
    }

    public void Interact(PlayerController player)
    {
        print("Gun");
    }

    public int getAmmo()
    {
        return magazineAddon.ammo;
    }

    public int getMaxAmmo()
    {
        return magazineAddon.maxAmmo;
    }

    public void swapItem(bool getBool)
    {        
        if (getBool) owner.handAnimator.runtimeAnimatorController = Resources.Load("Bot/Animations/Hands/2 Handed Gun/Bot_Hand") as RuntimeAnimatorController;
        else owner.setAnimation("isReloading", false);
    }

    public int rechargeDurability(int getVal)
    {
        int rechargeAmt = batteryAddon.recharge(getVal);
        displayAmmo();
        return rechargeAmt;
    }

    public void setOwner(LivingBeing getOwner)
    {
        owner = getOwner;
    }

    public void accessMenu(bool flag)
    {
        disabled = flag;
    }

    public float getEnergy()
    {
        return batteryAddon.charge;
    }

    public List<IAddon> getAllAddons()
    {
        List<IAddon> listOfAddons = new List<IAddon>();
        listOfAddons.Add(scopeAddon);
        listOfAddons.Add(batteryAddon);
        listOfAddons.Add(chamberAddon);
        listOfAddons.Add(muzzleAddon);
        listOfAddons.Add(magazineAddon);
        return listOfAddons;
    }
}

