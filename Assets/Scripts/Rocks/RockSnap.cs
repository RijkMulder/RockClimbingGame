using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSnap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ArmTarget armTarget))
        {
            CheckArm(armTarget);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out ArmTarget armTarget))
        {
            CheckArm(armTarget);
        }
    }
    private void CheckArm(ArmTarget armTarget)
    {
        if (armTarget.state == ArmState.Searching)
        {
            armTarget.state = ArmState.Snapped;
            armTarget.transform.position = transform.position - Offset(armTarget);
            armTarget.OnArmSnap?.Invoke();
        }
    }
    private Vector3 Offset(ArmTarget arm)
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        float zOffset = (mesh.bounds.size.z / 2) + arm.handOffset;
        float yOffset = mesh.bounds.size.y / 2;
        return new Vector3(0, yOffset, zOffset);
    }
}
