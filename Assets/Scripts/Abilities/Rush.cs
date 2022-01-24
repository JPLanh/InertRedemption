using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rush : MonoBehaviour, IAbilities
{
    private LivingBeing livingBeing;
    private PlayerController playerController;
    private BasicMovement basicMovement;
    private float abilityTimer;
    [SerializeField]
    private int speedIncrease = 75;
    private bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive && Time.time > abilityTimer)
        {
            playerController.canMove = true;
            playerController.canLook = true;
            isActive = false;
        }
    }

    public void activate()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float curSpeedX = playerController.canMove ? speedIncrease : 0;
        basicMovement.moveDirection = (forward * curSpeedX) + (right);
        playerController.canMove = false;
        playerController.canLook = false;
        abilityTimer = Time.time + 2f;
        isActive = true;
    }

    public void assignLivingBeing(LivingBeing livingBeing, PlayerController pc)
    {
        this.livingBeing = livingBeing;
        this.playerController = pc;
    }
}
