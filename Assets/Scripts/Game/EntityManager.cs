using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntityManager : MonoBehaviour
{
    GridSystem gridSystem;
    [SerializeField] private GameObject nodeList;
    [SerializeField] private GameObject resourceList;
    [SerializeField] private GameObject npcList;
    [SerializeField] private GameObject itemList;
    [SerializeField] private GameObject survivorsList;
    [SerializeField] private TimeSystem gameTime;
    [SerializeField] private GameObject spaceship;
    [SerializeField] private Transform virusList;
    int nodeCounter = 0;
    public int resourceCounter = 0;
    public int npcCounter = 0;

    public PlayerCanvas lv_canvas;

    public Survivors survivorPlaceHolder;

    //public static Dictionary<string, PlayerController> redTeam = new Dictionary<string, PlayerController>();
    //    public static Dictionary<string, PlayerController> blueTeam = new Dictionary<string, PlayerController>();

    public static Dictionary<string, PlayerController> survivors = new Dictionary<string, PlayerController>();
    public static Dictionary<string, VirusController> virus = new Dictionary<string, VirusController>();
    public static Dictionary<string, IPlayerController> players = new Dictionary<string, IPlayerController>();

    //GameObject blueBase;
    //GameObject redBase;

    // Start is called before the first frame update
    void Start()
    {
//        SceneManager.UnloadScene("mainScene");
        if (NetworkMain.local)
        {
            print("Local");
            Dictionary<string, string> localPlayer = StringUtils.getPayload();
            localPlayer["name"] = NetworkMain.Username;
            localPlayer["UserID"] = "8188HFCV6";
            localPlayer["Team"] = NetworkMain.Team;
            localPlayer["health"] = "100";
            spawnPlayer(localPlayer);
            newHost();
        } else
        {
            Dictionary<string, string> localPlayer = StringUtils.getPayload();
            localPlayer["Username"] = NetworkMain.Username;
            localPlayer["UserID"] = NetworkMain.UserID;
            localPlayer["lobbyID"] = NetworkMain.LobbyID;
            localPlayer["Team"] = NetworkMain.Team;
            localPlayer["health"] = "100";
            localPlayer["Action"] = "Join Game";
            NetworkMain.broadcastAction(localPlayer);
 //           NetworkMain.getUpdates(localPlayer);
        }

//        spawnTestDummies("Test Dummy 1");
        //spawnTestDummies("Test Dummy 2");
        //spawnTestDummies("Test Dummy 3");

//        modifyTestDummy("Test Dummy 1");
    }

    private void modifyTestDummy(string in_name)
    {
        survivors[in_name].transform.SetParent(survivorPlaceHolder.survivorList.transform);
        survivors[in_name].transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    private void spawnTestDummies(string in_name)
    {
        Dictionary<string, string> dummy = new Dictionary<string, string>();
        dummy["name"] = in_name;
        dummy["UserID"] = StringUtils.randomStringGen(5);
        dummy["Team"] = "Survivor";
        dummy["health"] = "100";
        spawnPlayer(dummy);
    }

    // Update is called once per frame
    void Update()
    {
        if(survivors.Count == 0 && virus.Count >= 1)
        {
//            Debug.Log("Virus Win");
        }
    }

    public void createResource(Dictionary<string, string> getPayload)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>(getPayload["resource"]), StringUtils.getVectorFromJson(getPayload, "Pos"), Quaternion.Euler(float.Parse(getPayload["xRot"]), 0, 0));
        obj.GetComponent<Resource>().amount = StringUtils.convertToInt(getPayload["amount"]);
        if (!NetworkMain.local)
        {
            obj.GetComponent<Resource>().UID = getPayload["UID"];
            obj.name = getPayload["UID"];
            obj.GetComponent<Resource>().durability = StringUtils.convertToFloat(getPayload["durability"]);
        }
        obj.transform.SetParent(resourceList.transform);
    }

    public void newHost()
    {
        if (NetworkMain.isHost || NetworkMain.local)
        {
            if (NetworkMain.local) resourceCounter = 75;
            nodeCounter = Random.Range(5, 10);
//            resourceCounter = Random.Range(30, 100);
            for (int i = resourceList.transform.childCount; i < resourceCounter; i++)
            {
                var position = new Vector3(Random.Range(-400f, 400f), 200, Random.Range(-400f, 400f));
                if (!(position.x > 250f && position.z > 250f) && !(position.x < -250f && position.z < -250f) && GetClosestResource(position, "Resource") > 5)
                {
                    spawnResources(position);
                }
            }
            StartCoroutine(objectSpawner());
        }
    }

    IEnumerator objectSpawner()
    {
        print("Starting as host");
        while (true)
        {
            if (nodeList.transform.childCount < nodeCounter)
            {
                var position = new Vector3(Random.Range(-500f, 500f), 200, Random.Range(-500f, 500f));
                var dis = GetClosestResource(position, "Node");
                if (!(position.x > 250f && position.z > 250f) && !(position.x < -250f && position.z < -250f) && dis > 25000 && dis != Mathf.Infinity)
                {
                    Dictionary<string, string> payload = StringUtils.getPositionAndRotation(position, Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0));
                    spawnNode(payload);
                }
            }
            if (resourceList.transform.childCount < resourceCounter)
            {
                var position = new Vector3(Random.Range(-500f, 500f), 200, Random.Range(-500f, 500f));
                    spawnResources(position);
                //if (!(position.x > 250f && position.z > 250f) && !(position.x < -250f && position.z < -250f) && GetClosestResource(position, "Resource") > 150)
                //{
                //}
            }
            //if (npcList.transform.childCount < npcCounter)
            //{
            //    var position = new Vector3(Random.Range(-500f, 500f), 200, Random.Range(-500f, 500f));
            //        spawnNPC(position);
            //    //if (!(position.x > 250f && position.z > 250f) && !(position.x < -250f && position.z < -250f) && GetClosestResource(position, "Resource") > 150)
            //    //{
            //    //    //spawnNPC(position, redBase, 0, 1);
            //    //    //spawnNPC(position, redBase, 0, 2);
            //    //    //spawnNPC(position, redBase, 0, 2);
            //    //}
            //}
            yield return new WaitForSeconds(5);
        }

    }

    public static float GetClosestResource(Vector3 getPosition, string getTag)
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag(getTag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = getPosition;
        foreach (GameObject go in gos)
        {
            Vector3 normalized = new Vector3(go.transform.position.x, 200, go.transform.position.z);
            Vector3 diff = normalized - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        if (gos.Length == 0)
        {
            return 30000;
        }
        return distance;
    }


    public PlayerController spawnPlayer(Dictionary<string, string> payload)
    {
        Dictionary<string, string> newPayload = loadPlayer(payload["Username"], payload["UserID"], payload["Team"], out PlayerController out_playerController, out VirusController out_virusController);
        if (out_playerController != null)
        {
            out_playerController.toggleFlashLight();
            out_playerController.survivorsGO = survivorPlaceHolder;
            out_playerController.networkListener = new PlayerNetworkListener(payload["Username"]);
            out_playerController.networkListener.controller = out_playerController;
            out_playerController.em = this;
            survivors.Add(payload["Username"], out_playerController);
            out_playerController.emitSound("Heartbeat", true);
            players.Add(payload["Username"], out_playerController);
            out_playerController.livingBeing.health = StringUtils.convertToFloat(payload["health"]);
            if (payload["Username"] == NetworkMain.Username)
            {
                if (payload.TryGetValue("Host", out string isaHost))
                    NetworkMain.isHost = bool.Parse(isaHost);
                out_playerController.setActivePlayer(payload["UserID"], payload["Username"], lv_canvas);
            }
            else out_playerController.setOtherPlayer(payload["UserID"], payload["Username"]);
        }
        if (out_virusController != null)
        {
            out_virusController.livingBeing.health = StringUtils.convertToFloat(payload["health"]);
            virus.Add(payload["Username"], out_virusController);
            players.Add(payload["Username"], out_virusController);
            out_virusController.networkListener = new PlayerNetworkListener(payload["Username"]);
            out_virusController.networkListener.controller = out_virusController;
            out_virusController.em = this;
            if (payload["Username"] == NetworkMain.Username)
            {
                if (payload.TryGetValue("Host", out string isaHost))
                    NetworkMain.isHost = bool.Parse(isaHost);
                out_virusController.setActivePlayer(payload["UserID"], payload["Username"], lv_canvas);
            }
            else out_virusController.setOtherPlayer(payload["UserID"], payload["Username"]);
        }
//        out_playerController.survivorsGO.spaceshipList = spaceship;



        return out_playerController;
    }

    public Dictionary<string, string> loadPlayer(string getName, string getUserID, string in_team, out PlayerController out_playerController, out VirusController out_virusController)
    {
        out_playerController = null;
        out_virusController = null;
        Dictionary<string, string> payload = new Dictionary<string, string>();
        GameObject newPlayer = null;
        switch (in_team)
        {
            case "Virus":
                newPlayer = Instantiate(Resources.Load<GameObject>("Virus"), transform.position, Quaternion.identity);
                newPlayer.transform.SetParent(virusList);
                newPlayer.transform.localPosition = new Vector3(Random.Range(-475, 475), 3f, Random.Range(-475, 475));
                newPlayer.TryGetComponent<VirusController>(out out_virusController);
                out_virusController.lv_infectionController.virusList = virusList;
                break;
            case "Survivor":
                newPlayer = Instantiate(Resources.Load<GameObject>("Player"), transform.position, Quaternion.identity);
                newPlayer.transform.SetParent(spaceship.transform);
                newPlayer.transform.localPosition = new Vector3(0f, 10f, 0f);
                newPlayer.TryGetComponent<PlayerController>(out out_playerController);
                break;
        }
        //out_playerController.teamColor = in_team;
        //out_playerController.survivorsGO = redTeamPlaceHolder;
        //switch (in_team)
        //{
        //    case "Red":
        //        EntityManager.redTeam.Add(getName, out_playerController);
        //        newPlayer.GetComponent<LivingBeing>().survivorList = redTeamPlaceHolder.survivorList;
        //        break;
        //    case "Blue":
        //        EntityManager.blueTeam.Add(getName, out_playerController);
        //        newPlayer.GetComponent<LivingBeing>().survivorList = blueTeamPlaceHolder.survivorList;
        //        break;
        //}

        payload["Action"] = "Join";
        payload["Team"] = in_team;
        newPlayer.transform.name = getName;
        return payload;
    }


    public void spawnNode(Dictionary<string, string> payload)
    {
        GameObject newNode = Instantiate(Resources.Load<GameObject>("Node"), StringUtils.getVectorFromJson(payload, "Pos"), StringUtils.getQuaternionFromJson(payload, "Rot"));
        newNode.name = "Node";
        newNode.transform.GetComponent<Node>().energy = Random.Range(100, 500);
        newNode.transform.SetParent(nodeList.transform);
    }


    public void spawnResources(Vector3 getPosition)
    {
        GameObject newNode = Instantiate(Resources.Load<GameObject>("Resource Spawner"), getPosition, Quaternion.Euler(0, 0, 0));
        newNode.name = "Resource";
        newNode.transform.GetComponent<ResourceSpawner>().resourceNum = Random.Range(0, 2);
        newNode.transform.SetParent(resourceList.transform);
    }

    public void spawnItem(Dictionary<string, string> payload)
    {
        GameObject GO = Instantiate(Resources.Load<GameObject>("Resource Loot"), StringUtils.getVectorFromJson(payload, "Pos"), Quaternion.Euler(float.Parse(payload["xRot"]), 0, 0));
        print(StringUtils.convertPayloadToJson(payload));
        GO.transform.GetComponent<Data>().resourceName = payload["resource"];
        GO.name = payload["UID"];
        GO.transform.GetComponent<Data>().UID = payload["UID"];
        GO.transform.SetParent(itemList.transform);

    }

    public void spawnNPC(Vector3 getPosition, GameObject getBase, int npcNum, int laneNum)
    {
        GameObject newNode = Instantiate(Resources.Load<GameObject>("NPC Spawner"), getPosition, Quaternion.Euler(0, 0, 0));
        newNode.name = "Monstur";
        newNode.GetComponent<MonsterSpawner>().getBase = getBase.GetComponent<Base>();
        newNode.GetComponent<MonsterSpawner>().npcNum = npcNum;
        newNode.GetComponent<MonsterSpawner>().laneNum = laneNum;
        newNode.GetComponent<MonsterSpawner>().gameTime = gameTime;
        //        newNode.transform.GetComponent<ResourceSpawner>().resourceNum = Random.Range(0, 1);
        newNode.transform.SetParent(npcList.transform);
    }

    public void spawnNPC(Vector3 getPosition)
    {
        GameObject newNode = Instantiate(Resources.Load<GameObject>("NPC Spawner"), getPosition, Quaternion.Euler(0, 0, 0));
        newNode.name = "Monstur";
        newNode.GetComponent<MonsterSpawner>().survivorsList = survivorsList;
        newNode.GetComponent<MonsterSpawner>().gameTime = gameTime;
        //        newNode.transform.GetComponent<ResourceSpawner>().resourceNum = Random.Range(0, 1);
        newNode.transform.SetParent(npcList.transform);
    }
}