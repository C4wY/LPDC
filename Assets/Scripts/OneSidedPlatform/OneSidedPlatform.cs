using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class OneSidedPlatform : MonoBehaviour
{
    public static readonly HashSet<OneSidedPlatform> instances = new();

    public bool snap = false;

    public new Collider collider;

    public Vector3 anchorOffset = Vector3.zero;

    public Vector3 Anchor =>
        transform.position + anchorOffset;

    void Snap()
    {
        var scale = transform.localScale.Abs();
        var min = transform.position - scale / 2;
        var max = transform.position + scale / 2;

        min.x = Mathf.Round(min.x);
        min.y = Mathf.Round(min.y * 8) / 8;
        min.z = Mathf.Round(min.z);

        max.x = Mathf.Round(max.x);
        max.y = Mathf.Round(max.y * 8) / 8;
        max.z = Mathf.Round(max.z);

        var position = (min + max) / 2;
        var size = max - min;
        position.y = Mathf.Round(position.y + size.y / 2) - size.y / 2;

        transform.position = position;
        transform.localScale = size;
    }

    void OnEnable()
    {
        instances.Add(this);
        collider = GetComponent<Collider>();
        gameObject.layer = LayerMask.NameToLayer("OneSidedPlatform");
    }

    void OnDisable()
    {
        instances.Remove(this);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
            if (snap)
                Snap();
#endif
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        DrawOneSidedPlatformGizmos();
    }

    public void DrawOneSidedPlatformGizmos()
    {
        Gizmos.DrawSphere(Anchor, 0.05f);

        var meshCollider = collider as MeshCollider;
        if (meshCollider != null)
        {
            Gizmos.matrix = meshCollider.transform.localToWorldMatrix;
            Gizmos.DrawWireMesh(meshCollider.sharedMesh);
        }
        else
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }
}