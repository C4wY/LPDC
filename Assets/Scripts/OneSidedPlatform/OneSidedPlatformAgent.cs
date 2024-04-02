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

    bool Ignore(OneSidedPlatform platform)
    {
        if (ignoreAll)
            return true;

        if (collidersToIgnore.Contains(platform.boxCollider))
            return true;

        return transform.position.y < platform.Top.y;
    }

    void OnEnable()
    {
        selfColliders = GetRigidbody().GetComponentsInChildren<Collider>();
    }

    void FixedUpdate()
    {
        foreach (var platform in OneSidedPlatform.instances)
        {
            var ignore = Ignore(platform);
            foreach (var collider in selfColliders)
                Physics.IgnoreCollision(collider, platform.boxCollider, ignore);
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach (var platform in OneSidedPlatform.instances)
        {
            Gizmos.color = Ignore(platform) ? Color.red : Color.green;
            Gizmos.DrawWireCube(platform.boxCollider.bounds.center, platform.boxCollider.bounds.size);
        }
    }
}