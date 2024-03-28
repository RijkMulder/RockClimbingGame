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
    [SerializeField] private ArmTarget otherArm;
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform armAlign;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 moveBounds;
    public IKFootSolver crossFoot;
    public ArmState state;
    public float handOffset;

    public delegate void ArmCheckDelegate();
    public ArmCheckDelegate OnArmSnap;
    private void Start()
    {
        state = ArmState.Searching;
    }
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
            x = handTransform.position.x + bounds.x,
            y = handTransform.position.y + bounds.y
        }; 
        Vector2 minPos = new Vector2
        {
            x = handTransform.position.x - bounds.x,
            y = handTransform.position.y - bounds.y
        };

        float x = transform.position.x + Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
        float y = transform.position.y + Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;
        x = Mathf.Clamp(x, minPos.x, maxPos.x);
        y = Mathf.Clamp(y, minPos.y, maxPos.y);
        transform.position = new Vector3(x, y, transform.position.z); 
    }
    public void ResetArm()
    {
        int dir = transform.position.x > armAlign.position.x ? 1 : -1;
        transform.position = new Vector3(armAlign.position.x + (0.5f * dir), armAlign.position.y, transform.position.z);
        state = ArmState.Free;
    }
}
