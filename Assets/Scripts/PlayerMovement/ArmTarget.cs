using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum ArmState
{
    Snapped,
    Searching,
    Free
}
public class ArmTarget : MonoBehaviour
{
    [HideInInspector] public Vector3 positionWhenControlled;
    [SerializeField] private ArmTarget otherArm;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 moveBounds;
    public ArmState state;
    public float handOffset;

    public delegate void ArmCheckDelegate();
    public ArmCheckDelegate OnArmSnap;
    private void Update()
    {
        switch (state)
        {
            case ArmState.Snapped:
                break;
            case ArmState.Searching:
                ControllTarget(moveBounds);
                break;
            case ArmState.Free:
                ControllTarget(moveBounds);
                break;
        }
    }
    private void ControllTarget(Vector2 bounds)
    {
        Vector2 maxPos = new Vector2
        {
            x = positionWhenControlled.x + bounds.x,
            y = positionWhenControlled.y + bounds.y
        }; 
        Vector2 minPos = new Vector2
        {
            x = positionWhenControlled.x - bounds.x,
            y = positionWhenControlled.y - bounds.y
        };

        float x = transform.position.x + Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
        float y = transform.position.y + Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;
        x = Mathf.Clamp(x, minPos.x, maxPos.x);
        y = Mathf.Clamp(y, minPos.y, maxPos.y);
        transform.position = new Vector3(x, y, transform.position.z); 
    }
}