using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject cam;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float smoothTime;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float camDistance;
    [SerializeField] private Vector2 maxRotation;
    private Vector3 offset;
    private Vector3 currentVel;
    private void Start()
    {
        offset = transform.position - target.transform.position;
        offset.y = 0;
    }
    private void LateUpdate()
    {
        // rotate cam
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if (Mathf.Abs(x) > 0.1f || Mathf.Abs(y) > 0.1f)
        {
            transform.RotateAround(target.transform.position, Vector3.up, -x * moveSpeed * Time.deltaTime);

            if (transform.position.z > -4.9f && transform.position.z < -1.8f)
            {
                transform.RotateAround(target.transform.position, transform.right, y * moveSpeed * Time.deltaTime);
            }
        }
        transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + offset - transform.forward * camDistance, ref currentVel, smoothTime, smoothSpeed);
    }
}
