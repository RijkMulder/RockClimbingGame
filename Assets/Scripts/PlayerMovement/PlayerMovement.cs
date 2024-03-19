using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // both buttons need to be clicked to start game
    private bool StartGame = false;

    // hands
    [SerializeField] private ArmTarget armTargetLeft;
    [SerializeField] private ArmTarget armTargetRight;
    [SerializeField] private Transform armAlign;
    private Coroutine moveCoroutine;
    private void Start()
    {
        armTargetLeft.OnArmSnap += () => CheckArmDistance(armTargetLeft);
        armTargetRight.OnArmSnap += () => CheckArmDistance(armTargetRight);
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            StartGame = true;
        }

        if (StartGame)
        {
            HandleArmInput(0, armTargetLeft);
            HandleArmInput(1, armTargetRight);
        }
    }
    private void HandleArmInput(int button, ArmTarget armTarget)
    {
        if (Input.GetMouseButtonUp(button))
        {
            armTarget.positionWhenControlled = armTarget.transform.position;
            armTarget.state = ArmState.Free;
        }
        else if (Input.GetMouseButtonDown(button))
        {
            armTarget.positionWhenControlled = armTarget.transform.position;
            armTarget.state = ArmState.Searching;
        }
    }
    /// <summary>
    /// Checks distance of relevant arm to armAlign (point in player body)
    /// </summary>
    /// <param name="armTarget"></param>
    private void CheckArmDistance(ArmTarget armTarget)
    {
        float armY = armTarget.transform.position.y;
        float alignY = armAlign.position.y;

        Vector3 targetPos = new Vector3
        {
            x = transform.position.x,
            y = transform.position.y + (armY - alignY),
            z = transform.position.z
        };

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MovePlayer(targetPos));
    }
    private IEnumerator MovePlayer(Vector3 targetPos)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;

            transform.position = Vector3.Lerp(transform.position, targetPos, t);
            yield return null;
        }
        moveCoroutine = null;
    }
}
