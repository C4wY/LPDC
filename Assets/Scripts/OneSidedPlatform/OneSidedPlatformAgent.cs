using UnityEngine;

[ExecuteAlways]
public class OneSidedPlatformAgent : MonoBehaviour
{
    public Collider[] colliders;

    Rigidbody GetRigidbody()
    {
        if (TryGetComponent<Rigidbody>(out var rigidbody))
            return rigidbody;

        rigidbody = GetComponentInParent<Rigidbody>();

        return rigidbody;
    }

    void OnEnable()
    {
        colliders = GetRigidbody().GetComponentsInChildren<Collider>();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
            return;
#endif
        foreach (var platform in OneSidedPlatform.instances)
        {
            var ignore = transform.position.y < platform.Top.y;
            foreach (var collider in colliders)
            {
                Physics.IgnoreCollision(collider, platform.boxCollider, ignore);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach (var platform in OneSidedPlatform.instances)
        {
            var ignore = transform.position.y < platform.Top.y;
            Gizmos.color = ignore ? Color.red : Color.green;
            Gizmos.DrawWireCube(platform.boxCollider.bounds.center, platform.boxCollider.bounds.size);
        }
    }
}