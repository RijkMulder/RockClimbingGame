using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum EPlayerState
{
    Climbing,
    Falling
}
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    private EPlayerState playerState;

    [SerializeField] private ArmTarget armTargetLeft;
    [SerializeField] private ArmTarget armTargetRight;
    [SerializeField] private Transform armAlign;
    [SerializeField] private Transform headTransform;

    [SerializeField] private float maxYDistance;
    [SerializeField] private float maxXDistance;
    [SerializeField] private float xOffset;
    [SerializeField] private float climbTime;

    private Rigidbody rb;
    private Transform currentHand;
    private Coroutine moveCoroutine;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        armTargetLeft.OnArmSnap += () => CheckArmDistance(armTargetLeft);
        armTargetRight.OnArmSnap += () => CheckArmDistance(armTargetRight);
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (currentHand != null) LookAtHand(currentHand);
        HandleArmInput(0, armTargetLeft);
        HandleArmInput(1, armTargetRight);
    }
    private void HandleArmInput(int button, ArmTarget armTarget)
    {
        ArmTarget otherArm = armTarget == armTargetLeft ? armTargetRight : armTargetLeft;
        if (Input.GetMouseButtonUp(button))
        {
            if (otherArm.state == ArmState.Free)
            {
                Fall();
                playerState = EPlayerState.Falling;
            }
            armTarget.state = ArmState.Free;
            currentHand = armTarget.transform;
        }
        else if (Input.GetMouseButtonDown(button))
        {
            armTarget.state = ArmState.Searching;
        }
    }
    /// <summary>
    /// Checks distance of relevant arm to armAlign (point in player body)
    /// </summary>
    /// <param name="armTarget"></param>
    private void CheckArmDistance(ArmTarget armTarget)
    {
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
        if (distance > maxYDistance)
        {
            targetPos.y -= targetPos.y > transform.position.y ? distance - maxYDistance : -(distance - maxYDistance);
        }
        if (distanceX > maxXDistance)
        {
            targetPos.x -= targetPos.x > transform.position.x ? distanceX - maxXDistance : -(distanceX - maxXDistance);
        }
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MovePlayer(targetPos, armTarget));
    }
    private IEnumerator MovePlayer(Vector3 targetPos, ArmTarget arm)
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
        arm.crossFoot.MoveFoot();
        moveCoroutine = null;
    }
    private void LookAtHand(Transform target)
    {
        Quaternion targetRot = Quaternion.LookRotation(target.position - headTransform.position);
        headTransform.rotation = Quaternion.Slerp(headTransform.rotation, targetRot, 10 * Time.deltaTime);
    }
    private void Fall()
    {
        rb.useGravity = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        
    }
}
