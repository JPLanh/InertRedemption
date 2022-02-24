using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{

    [SerializeField] private PlayerController lv_playerController;
    public Vector3 rotation = Vector3.zero;
    public Vector3 moveDirection = Vector3.zero;

    public GameObject lead;

    private float lastUpdate = 0;
    private float lookXLimit = 45.0f;
    private Vector3 hitNormal;
    private float slopeLimit = 60f;
    private float slideSpeed = 6f;
    public float updateTimer;


    public AudioSource running_footstep;
    public AudioSource walking_footstep;

    Vector3 lastRot = Vector3.zero;
    Vector3 lastPos = Vector3.zero;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lv_playerController.inControl)
        {
            //if (transform.localPosition.x < -495)
            //{
            //    lv_playerController.characterController.enabled = false;
            //    transform.localPosition = new Vector3(495f, transform.localPosition.y, transform.localPosition.z);
            //    lv_playerController.characterController.enabled = true;
            //}
            //if (transform.localPosition.x > 495f)
            //{
            //    lv_playerController.characterController.enabled = false;
            //    transform.localPosition = new Vector3(-495f, transform.localPosition.y, transform.localPosition.z);
            //    lv_playerController.characterController.enabled = true;

            //}
            //if (transform.localPosition.z < -495f)
            //{
            //    lv_playerController.characterController.enabled = false;
            //    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 495f);
            //    lv_playerController.characterController.enabled = true;
            //}
            //if (transform.localPosition.z > 495f)
            //{
            //    lv_playerController.characterController.enabled = false;
            //    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -495f);
            //    lv_playerController.characterController.enabled = true;
            //}

            if (!NetworkMain.local && lv_playerController.canMove) PositionalUpdate();

            if (lv_playerController.canLook)
            {
                if (!lv_playerController.livingBeing.handAnimator.GetBool("accessMenu") && NetworkMain.isBroadcastable)
                {
                    fpsCameraView();
                }

                if (lv_playerController.characterController.isGrounded && lv_playerController.canMove)
                {
                    characterMove();
                }


                //if (!onLedge)
                //    moveDirection.y -= livingBeing.gravity * Time.deltaTime;
                //else
                //    moveDirection = Vector3.zero;

                gravity();
                lv_playerController.isGrounded = (Vector3.Angle(Vector3.up, hitNormal) <= slopeLimit);

                if (!lv_playerController.isGrounded)
                {
                    moveDirection.x = ((1f - hitNormal.y) * hitNormal.x) * slideSpeed;
                    moveDirection.z = ((1f - hitNormal.y) * hitNormal.z) * slideSpeed;
                }
                // Move the controller, use this to help calculate to fire ahead
                lv_playerController.characterController.Move(moveDirection * Time.deltaTime);

            }
        }
    }

    private void gravity()
    {
        moveDirection.y -= lv_playerController.livingBeing.gravity * Time.deltaTime;
        if (moveDirection.y > 1 || moveDirection.y < -2)
        {
            walking_footstep.Stop();
            running_footstep.Stop();
        }
    }
    private void fpsCameraView()
    {
        rotation.y += Input.GetAxis("Mouse X") * lv_playerController.livingBeing.lookSensativity;
        rotation.z = 0;

        if (lv_playerController.livingBeing.currentWeapon.GetComponent<Gun>() && lv_playerController.livingBeing.handAnimator.GetBool("isReloading"))
        { }
        else
        {
            rotation.x += -Input.GetAxis("Mouse Y") * lv_playerController.livingBeing.lookSensativity;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);

            //switch (lookState)
            //{
            //    case "Tall Shield":
            //        rotation.x = Mathf.Clamp(rotation.x, tallShieldLookXLo, tallShieldLookXHi);
            //        break;
            //    default:
            //        rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            //        break;
            //}

            lv_playerController.livingBeing.upperBody.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
        }
//        transform.eulerAngles = new Vector2(0, rotation.y);
        lead.transform.eulerAngles = new Vector2(rotation.x, rotation.y);
    }

    public void PositionalUpdate()
    {
        if (Time.time >= lastUpdate && (lastRot != rotation || lastPos != lead.transform.position))
        {
            lastPos = lead.transform.position;
            lastRot = rotation;
            Dictionary<string, string> payload = StringUtils.getPositionAndCameraRotation(lead.transform.position, rotation, lv_playerController.livingBeing.upperBody.transform.localRotation.eulerAngles);
            payload["Action"] = "Update";
            payload["Type"] = "Player Update";
            payload["State"] = "Alive";
            payload["WeaponState"] = StringUtils.convertIntToString(lv_playerController.weaponState);

            payload["Username"] = NetworkMain.Username;
            payload["UserID"] = NetworkMain.UserID;
            payload["Team"] = NetworkMain.Team;
            payload["health"] = lv_playerController.livingBeing.health.ToString();
            payload["insanity"] = lv_playerController.insanityLevel.ToString();
            //            payload["host"] = NetworkMain.isHost.ToString();

            lastUpdate = Time.time + 1f / updateTimer;
            lv_playerController.serverControl(payload);
            NetworkMain.broadcastToOther(payload);
            //            NetworkMain.socket.Emit("Update", StringUtils.convertPayloadToJson(payload));
        }
    }

    public void playMovementSound()
    {
        if (lv_playerController.livingBeing.legsAnimator.GetBool("isCrouching"))
        {
            if (!walking_footstep.isPlaying && lv_playerController.isGrounded)
            {
                running_footstep.Stop();
                walking_footstep.Play();
            }
        }
        else
        {
            Debug.Log(running_footstep.isPlaying);
            if (!running_footstep.isPlaying && lv_playerController.isGrounded)
            {
                Debug.Log("Is running");
                walking_footstep.Stop();
                running_footstep.Play();
            }
        }
    }

    public void stopMovementSound()
    {
        walking_footstep.Stop();
        running_footstep.Stop();
    }

    private void characterMove()
    {
        // We are grounded, so recalculate move direction based on axes
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            lv_playerController.livingBeing.legsAnimator.SetBool("Walking", lv_playerController.livingBeing.legsAnimator.GetBool("Walking"));
            lv_playerController.livingBeing.legsAnimator.SetBool("Running", !lv_playerController.livingBeing.legsAnimator.GetBool("Walking"));
//            playMovementSound();
        }
        else
        {
//            stopMovementSound();
            lv_playerController.livingBeing.legsAnimator.SetBool("Running", false);
            lv_playerController.livingBeing.legsAnimator.SetBool("Walking", false);
        }
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float curSpeedX =
            lv_playerController.canMove
            ?
                lv_playerController.autorun
                ?
                    lv_playerController.livingBeing.speed
                    :
                    (Input.GetAxis("Vertical") > 0
                    ?
                        lv_playerController.livingBeing.speed
                        :
                        lv_playerController.livingBeing.speed * .65f) * Input.GetAxis("Vertical")
                :
            0;
        float curSpeedY = lv_playerController.canMove ? lv_playerController.livingBeing.speed * .65f * Input.GetAxis("Horizontal") : 0;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && lv_playerController.canMove)
        {
            moveDirection.y = lv_playerController.livingBeing.jumpSpeed;
        }


    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }
}
