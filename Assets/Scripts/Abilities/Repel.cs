using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repel : MonoBehaviour, IAbilities
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
    public float radius = 25.0F;
    public float power = 50.0F;
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
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            CharacterController rb = hit.GetComponent<CharacterController>();

            if (rb != null)
            {
                print(rb);
                hit.GetComponent<Rigidbody>().isKinematic = false;
                hit.GetComponent<Rigidbody>().useGravity = true;
                hit.GetComponent<Rigidbody>().AddExplosionForce(power, explosionPos, radius, 1.0F, ForceMode.Impulse);

            }
        }
    }

    public void assignLivingBeing(LivingBeing livingBeing, PlayerController pc)
    {
        this.livingBeing = livingBeing;
        this.playerController = pc;
    }
}
