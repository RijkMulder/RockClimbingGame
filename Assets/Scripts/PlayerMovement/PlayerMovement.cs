using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    // hands
    [SerializeField] private ArmTarget armTargetLeft;
    [SerializeField] private ArmTarget armTargetRight;
    [SerializeField] private Transform armAlign;
    [SerializeField] private float maxYDistance;
    [SerializeField] private float maxXDistance;
    [SerializeField] private float climbTime;
    private Coroutine moveCoroutine;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        armTargetLeft.OnArmSnap += () => CheckArmDistance(armTargetLeft);
        armTargetRight.OnArmSnap += () => CheckArmDistance(armTargetRight);
    }
    private void Update()
    {
        HandleArmInput(0, armTargetLeft);
        HandleArmInput(1, armTargetRight);
    }
    private void HandleArmInput(int button, ArmTarget armTarget)
    {
        if (Input.GetMouseButtonUp(button))
        {
            armTarget.state = ArmState.Free;
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
        float xMultiplier = otherArm.transform.position == armTarget.transform.position ? 1f : maxXDistance;

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

        // adjust target y pos
        Vector3 armAlignTargetPos = armAlign.transform.position + (targetPos - transform.position);
        float distance = Vector3.Distance(otherArm.transform.position, armAlignTargetPos);
        if (distance > maxYDistance)
        {
            targetPos.y -= targetPos.y > transform.position.y ? distance - maxYDistance : -(distance - maxYDistance);
        }

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MovePlayer(targetPos));
    }
    private IEnumerator MovePlayer(Vector3 targetPos)
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
        moveCoroutine = null;
    }
}
