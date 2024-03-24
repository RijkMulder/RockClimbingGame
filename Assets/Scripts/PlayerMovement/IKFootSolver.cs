using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] private IKFootSolver otherFoot;
    [SerializeField] private float moveTime;
    [SerializeField] private Transform newFootTarget;
    [SerializeField] private float maxDistance;
    private Coroutine moveCoroutine;
    public void MoveFoot()
    {
        Vector3 targetPos = newFootTarget.position;
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(Move(targetPos));
    }
    private void TryMoveOtherFoot()
    {
        float distance = Vector3.Distance(transform.position, otherFoot.transform.position);
        if (distance > maxDistance)
        {
            otherFoot.MoveFoot();
        }
    }
    private IEnumerator Move(Vector3 target)
    {
        float t = 0;
        Vector3 startPos = transform.position;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float prc = t / moveTime;
            transform.position = Vector3.Lerp(startPos, target, prc);
            yield return null;
        }
        TryMoveOtherFoot();
        moveCoroutine = null;
    }
}
