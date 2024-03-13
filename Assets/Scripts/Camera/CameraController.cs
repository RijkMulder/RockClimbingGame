using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float smoothTime;
    [SerializeField] private float moveSpeed;
    private Vector3 currentVel;
    private void Update()
    {
        // rotate cam
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if (Mathf.Abs(x) > 0.1f || Mathf.Abs(y) > 0.1f)
        {
            transform.RotateAround(target.transform.position, transform.up, -x * moveSpeed * Time.deltaTime);
            transform.RotateAround(target.transform.position, transform.right, y * moveSpeed * Time.deltaTime);
            transform.LookAt(target.transform.position);
        }

        // set position
        offset.x = transform.position.x - target.transform.position.x;
        offset.z = transform.position.z - target.transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + offset, ref currentVel, smoothTime, smoothSpeed);
    }
}
