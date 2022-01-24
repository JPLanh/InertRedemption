using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    public List<GameObject> resources;
    public int resourceNum;
    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (IsGrounded())
        {
                    Dictionary<string, string> payload = StringUtils.getPositionAndRotation(transform.position, transform.rotation);
            resourceNum = Random.Range(0, 2);
            //Add in the unique ID for this object
            payload["amount"] = StringUtils.convertIntToString(Random.Range(25, 500));
            string resourceObj = null;
            switch (resourceNum)
            {
                case 0:
                    resourceObj = "Stone";
                    break;
                case 1:
                    resourceObj = "Tree";
                    break;
            }
            payload["resource"] = resourceObj;
            payload["Action"] = "Spawn Resource";
            payload["durability"] = "100";
            payload["lobbyID"] = NetworkMain.LobbyID;
            if (!NetworkMain.local)
            {
                NetworkMain.messageServer(payload);
//                NetworkMain.socket.Emit("Action", StringUtils.convertPayloadToJson(payload));

            } else
            {
                GameObject obj = Instantiate(Resources.Load<GameObject>(resourceObj), transform.position, transform.rotation);
                obj.GetComponent<Resource>().amount = StringUtils.convertToInt(payload["amount"]);
                obj.transform.SetParent(transform.parent);
            }

            Destroy(gameObject);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 2f, whatIsGround);
    }
}
