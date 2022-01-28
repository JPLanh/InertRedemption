using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float distance;
    public float damage;
    public string damageType;
    private Vector3 initPos;
    public GameObject impactEffect;
    public GameObject attacker;

    public void setProjectile(float getSpeed, float getDistance, float getDamage)
    {
        speed = getSpeed;
        distance = getDistance;
        damage = getDamage;
    }

    public void setProjectile(float getSpeed, float getDistance, float getDamage, string getDamageType)
    {
        speed = getSpeed;
        distance = getDistance;
        damage = getDamage;
        damageType = getDamageType;
    }
    // Start is called before the first frame update
    void Start()
    {
        //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if (Physics.Raycast(ray, out var hit, distance))
        //{
        //    var selection = hit.transform.parent.GetComponent<Displayable>();
        //    if (selection != null)
        //    {
        //        if (displayInterface.activeInHierarchy == false)
        //        {
        //            displayInterface.SetActive(true);
        //        }
        //        displayInterface.transform.Find("Display Text").GetComponent<Text>().text = selection.display();
        //    }
        //    else
        //    {
        //        displayInterface.SetActive(false);
        //    }
        //}
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        if (Vector3.Distance(initPos, transform.position) > distance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        Damagable target = null;
        if (other.transform.parent != null) target = other.transform.parent.transform.GetComponent<Damagable>();
        if (target == null) target = other.transform.GetComponent<Damagable>();

        if (target != null && (other.gameObject.layer != LayerMask.NameToLayer("LivingBeingsBlocker")))
        {

            target.damage(true, damage, attacker);
//            if (isPlayer != null) NetworkMain.socket.Emit("Player", JsonConvert.SerializeObject(payload));
        }
        if (!other.transform.GetComponent<Projectile>() &&
            !other.transform.GetComponent<NodeCollision>() &&
            (other.gameObject.layer != LayerMask.NameToLayer("LivingBeingsBlocker")))
        {
            GameObject objHit = Instantiate(impactEffect, transform.position, transform.rotation);
            objHit.transform.SetParent(GameObject.Find("Item List").transform);
            Destroy(objHit, 1f);
            Destroy(gameObject);
        }
    }

}
