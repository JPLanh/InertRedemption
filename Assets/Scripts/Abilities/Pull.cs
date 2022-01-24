 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pull : MonoBehaviour, IAbilities
{
    private LivingBeing livingBeing;
    private PlayerController playerController;
    private float abilityTimer;
    [SerializeField]
    private int speedIncrease = 75;
    private Vector3 targetPosition;
    private Vector3 initPosition;
    private Vector3 initAnchorPosition;
    private Vector3 targetAnchorPosition;
    private float abilityDuration = 1f;
    private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void activate()
    {

        if (Physics.Raycast(playerController.playerCamera.transform.position, playerController.playerCamera.transform.forward, out var hit, 100f, ~(1 << 7)))
        {
            //initPosition = transform.position;
            //initAnchorPosition = transform.position + new Vector3(0f, 35f, 0f);
            //targetPosition = hit.point + new Vector3(0f, 3f, 0f);
            //targetAnchorPosition = hit.point + new Vector3(0f, 35f, 0f);
            if (hit.transform.gameObject.GetComponent<LivingBeing>())
            {
                targetPosition = transform.position + new Vector3(0f, 2f, 0f) + transform.forward * 10f;
                targetAnchorPosition = transform.position + new Vector3(0f, 10f, 0f);
                initPosition = hit.point;
                initAnchorPosition = hit.point + new Vector3(0f, 10f, 0f);
                StartCoroutine(pulling(hit.transform));
                abilityTimer = Time.time + abilityDuration;
                isActive = true; 
            }
        }
    }

    public void assignLivingBeing(LivingBeing livingBeing, PlayerController pc)
    {
        this.livingBeing = livingBeing;
        this.playerController = pc;
    }

    IEnumerator pulling(Transform other)
    {
        float leapTimer = 0;
        NetworkMain.broadcastAction("Pull", other.name);
        while (leapTimer < abilityDuration)
        {
            other.transform.position = cubeBezier3(initPosition, initAnchorPosition, targetAnchorPosition, targetPosition, leapTimer / abilityDuration);
            NetworkMain.updateEntityPosition(other.gameObject);
            leapTimer += Time.deltaTime;
            yield return null;
        }

        NetworkMain.broadcastAction("Pull Finish", other.name);
        isActive = false;
    }
    public static Vector3 cubeBezier3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float r = 1f - t;
        float f0 = r * r * r;
        float f1 = r * r * t * 3;
        float f2 = r * t * t * 3;
        float f3 = t * t * t;
        return f0 * p0 + f1 * p1 + f2 * p2 + f3 * p3;
    }
}
