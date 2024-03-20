using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class LegTarget : MonoBehaviour
{
    [SerializeField] private Transform kneeTransform;
    [SerializeField] private Transform legBaseTransform;
    [SerializeField] private float moveTime;
    [SerializeField] private float maxDistance;
    [SerializeField] private float stepDistance;

    private Coroutine moveCoroutine;

    private void Update()
    {
        if (Vector3.Distance(kneeTransform.position, transform.position) > maxDistance)
        {
            if (moveCoroutine == null) moveCoroutine = StartCoroutine(MoveFoot());
        }
    }
    private IEnumerator MoveFoot()
    {
        Vector3 targetPos = new Vector3
        {
            x = legBaseTransform.position.x,
            y = kneeTransform.position.y + stepDistance,
            z = legBaseTransform.position.z,
        };

        float t = 0;
        Vector3 startPos = transform.position;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            float prc = t / moveTime;
            transform.position = Vector3.Lerp(startPos, targetPos, prc);
            yield return null;
        }
        moveCoroutine = null;
    }
}
