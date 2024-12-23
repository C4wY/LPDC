using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class OneSidedPlatformAgent : MonoBehaviour
{
    public bool ignoreAll = false;

    readonly HashSet<Collider> collidersToIgnore = new();
    Collider[] selfColliders;

    Rigidbody GetRigidbody()
    {
        if (TryGetComponent<Rigidbody>(out var rigidbody))
            return rigidbody;

        rigidbody = GetComponentInParent<Rigidbody>();

        return rigidbody;
    }

    bool ShouldIgnore(OneSidedPlatform platform)
    {
        if (ignoreAll)
            return true;

        if (collidersToIgnore.Contains(platform.collider))
            return true;

        return transform.position.y < platform.Anchor.y;
    }

    void OnEnable()
    {
        selfColliders = GetRigidbody().GetComponentsInChildren<Collider>();
    }

    void FixedUpdate()
    {
        foreach (var platform in OneSidedPlatform.instances)
        {
            var ignore = ShouldIgnore(platform);
            foreach (var collider in selfColliders)
                Physics.IgnoreCollision(collider, platform.collider, ignore);
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach (var platform in OneSidedPlatform.instances)
        {
            Gizmos.color = ShouldIgnore(platform) ? Color.red : Color.green;
            platform.DrawOneSidedPlatformGizmos();
        }
    }
}