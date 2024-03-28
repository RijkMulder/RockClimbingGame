using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum EPlayerState
{
    Climbing,
    Falling
}
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    [Header("Player State")]
    public EPlayerState playerState;

    [Header("Arm Targets")]
    [SerializeField] private ArmTarget armTargetLeft;
    [SerializeField] private ArmTarget armTargetRight;
    [SerializeField] private float handMoveStaminaAmount;

    [Header("Foot Targets")]
    [SerializeField] private IKFootSolver leftFootTarget;
    [SerializeField] private IKFootSolver rightFootTarget;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpTime;
    [SerializeField] private float jumpStamina;

    [Header("Transform References")]
    [SerializeField] private Transform armAlign;
    [SerializeField] private Transform headTransform;

    [Header("Movement Parameters")]
    [SerializeField] private float maxArmDistance;
    [SerializeField] private float xOffset;
    [SerializeField] private float climbTime;

    [Header("Stamina")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float stamina;
    [SerializeField] private float jumpStaminaAmount;
    [SerializeField] private float rechargeAmnt;
    [SerializeField] private TMP_Text stamText;

    [HideInInspector] public Rigidbody rb;
    private Transform currentHand;
    private Coroutine moveCoroutine;
    private GameManager gm;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        gm = GameManager.instance;
        armTargetLeft.OnArmSnap += () => OnArmSnapped(armTargetLeft);
        armTargetRight.OnArmSnap += () => OnArmSnapped(armTargetRight);
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (currentHand != null) LookAtHand(currentHand);
        HandleArmInput(0, armTargetLeft);
        HandleArmInput(1, armTargetRight);
        RechargeStamina();

        if (Input.GetButton("Jump"))
        {
            Jump();
        }
        if (stamina <= 0.1)
        {
            Fall(true);
            this.enabled = false;
        }
    }
    private void HandleArmInput(int button, ArmTarget armTarget)
    {
        ArmTarget otherArm = (armTarget == armTargetLeft) ? armTargetRight : armTargetLeft;

        // Handle falling if the other arm is snapped
        if (otherArm.state == ArmState.Snapped) Fall(false);

        // Handle releasing the arm
        if (Input.GetMouseButtonUp(button))
        {
            if (otherArm.state == ArmState.Free)
            {
                Fall(true);
                playerState = EPlayerState.Falling;
            }
            armTarget.state = ArmState.Free;
            currentHand = armTarget.transform;
        }
        // Handle searching for the arm
        else if (Input.GetMouseButtonDown(button))
        {
            armTarget.state = ArmState.Searching;
        }

        // Move towards controlled arm if other arm is snapped and current arm is free
        if (otherArm.state == ArmState.Snapped && armTarget.state == ArmState.Free)
        {
            float distanceToOtherArm = Vector3.Distance(otherArm.transform.position, armAlign.position);
            float distanceToArmAlign = Vector3.Distance(armTarget.transform.position, armAlign.position);
            if (distanceToOtherArm < maxArmDistance && distanceToArmAlign + 0.1f > maxArmDistance)
            {
                Vector3 targetPos = new Vector3(armTarget.transform.position.x, armTarget.transform.position.y - 1, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, 1 * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Checks distance of relevant arm to armAlign (point in player body)
    /// </summary>
    /// <param name="armTarget"></param>
    private void OnArmSnapped(ArmTarget armTarget)
    {
        // deplete stamina
        UpdateStamina(handMoveStaminaAmount);

        ArmTarget otherArm = armTarget == armTargetLeft ? armTargetRight : armTargetLeft;
        float xMultiplier = otherArm.transform.position == armTarget.transform.position ? 1f : xOffset;

        // get positions
        Vector3 armPos = armTarget.transform.position;
        Vector3 alignPos = armAlign.position;

        // set base target pos
        Vector3 targetPos = new Vector3
        {
            x = transform.position.x + ((armPos.x - alignPos.x) / xMultiplier),
            y = transform.position.y + (armPos.y - alignPos.y),
            z = transform.position.z
        };

        // adjust target pos
        Vector3 armAlignTargetPos = armAlign.transform.position + (targetPos - transform.position);
        float distance = Vector3.Distance(otherArm.transform.position, armAlignTargetPos);
        float distanceX = otherArm.transform.position.x - armAlignTargetPos.x;
        if (distance > maxArmDistance)
        {
            targetPos.y -= targetPos.y > transform.position.y ? distance - maxArmDistance : -(distance - maxArmDistance);
        }
        if (distanceX > maxArmDistance)
        {
            targetPos.x -= targetPos.x > transform.position.x ? distanceX - maxArmDistance : -(distanceX - maxArmDistance);
        }
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MovePlayer(targetPos, armTarget));
    }
    private IEnumerator MovePlayer(Vector3 targetPos, ArmTarget arm = null)
    {
        float t = 0;
        Vector3 startPos = transform.position;  
        while (t < climbTime)
        {
            t += Time.deltaTime;
            float prc = t / climbTime;
            transform.position = Vector3.Slerp(startPos, targetPos, prc);
            yield return null;
        }
        if (arm)arm.crossFoot.MoveFoot();
        moveCoroutine = null;
    }
    private void LookAtHand(Transform target)
    {
        Quaternion targetRot = Quaternion.LookRotation(target.position - headTransform.position);
        headTransform.rotation = Quaternion.Slerp(headTransform.rotation, targetRot, 10 * Time.deltaTime);
    }
    private void UpdateStamina(float value)
    {
        if (stamina + value <= 0) value = -stamina;
        stamina += value;
        stamText.text = stamina.ToString("f0");
    }
    private void RechargeStamina()
    {
        if (armTargetLeft.state == ArmState.Snapped && armTargetRight.state == ArmState.Snapped && stamina < maxStamina)
        {
            UpdateStamina(rechargeAmnt * Time.deltaTime);
        }
    }
    private void Jump()
    {
        Vector3 dir = new Vector3
        {
            x = Input.GetAxis("Mouse X"),
            y = Input.GetAxis("Mouse Y"),
            z = 0
        };
        dir.Normalize();
        if (dir.magnitude < 0.3f) dir = Vector3.up;
        if (armTargetRight.state == ArmState.Snapped && armTargetLeft.state == ArmState.Snapped && stamina > -jumpStamina)
        {
            UpdateStamina(jumpStamina);


            rb.isKinematic = false;
            rb.AddForce(dir * jumpForce);
            armTargetLeft.state = ArmState.Free;
            armTargetRight.state = ArmState.Free;
        }
    }
    public void ResetLimbs()
    {
        gm.playerFalling = false;
        UpdateStamina(maxStamina);
        armTargetLeft.ResetArm();
        armTargetRight.ResetArm();
        leftFootTarget.ResetFoot();
        rightFootTarget.ResetFoot();
    }
    public void Fall(bool state)
    {
        playerState = EPlayerState.Falling;
        rb.isKinematic = !state;
        gm.playerFalling = true;
    }
}
