using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public List<GameObject> npcsSet;
    public int npcNum;
    public int laneNum;
    public Base getBase;
    public GameObject survivorsList;
    public TimeSystem gameTime;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (IsGrounded())
        {
            GameObject obj = Instantiate(npcsSet[npcNum], transform.position + new Vector3(0f, 1f, 0f), transform.rotation);
            //            obj.GetComponent<Worm>().setMinion(laneNum, getBase, -1);
            obj.GetComponent<Worm>().setMinion(gameTime);
            obj.transform.SetParent(transform.parent);
            obj.GetComponent<LivingBeing>().survivorList = survivorsList;
            survivorsList.GetComponent<Survivors>().addMarkedTarget(obj);
//            GameObject.Find("Canvas").GetComponent<PlayerCanvas>().playerCompass.GetComponent<compass>().addTarget(obj.GetComponent<TargetMarker>());
            Destroy(gameObject);
        }

        if (transform.position.y < -1000)
            Destroy(gameObject);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 2f);
    }
}
