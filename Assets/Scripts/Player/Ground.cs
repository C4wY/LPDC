using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class Ground : MonoBehaviour
{
    [System.Serializable]
    public class GroundParameters
    {
        [Range(0, 1)]
        public float pivotOffsetY = 0.95f;

        [Tooltip("The maximum distance of the raycast from the offseted pivot point.")]
        public float rayMaxDistance = 3;

        [Tooltip("The maximum distance from the ground to be considered as a being on the ground.")]
        public float maxDistance = 0.1f;
    }

    public struct GroundRaycastInfo
    {
        public bool wallHit;
        public Ray wallRay;
        public RaycastHit wallInfo;
        public bool groundHit;
        public Ray groundRay;
        public RaycastHit groundInfo;

        public GroundRaycastInfo(bool wallHit, Ray wallRay, RaycastHit wallInfo, bool groundHit, Ray groundRay, RaycastHit groundInfo)
        {
            this.wallHit = wallHit;
            this.wallRay = wallRay;
            this.wallInfo = wallInfo;
            this.groundHit = groundHit;
            this.groundRay = groundRay;
            this.groundInfo = groundInfo;
        }

        public readonly void Deconstruct(out bool wallHit, out Ray wallRay, out RaycastHit wallInfo, out bool groundHit, out Ray groundRay, out RaycastHit groundInfo)
        {
            wallHit = this.wallHit;
            wallRay = this.wallRay;
            wallInfo = this.wallInfo;
            groundHit = this.groundHit;
            groundRay = this.groundRay;
            groundInfo = this.groundInfo;
        }
    }

    public static float[] layerZPositions = { 0.5f, 1.5f, 2.5f, 3.5f };

    public GroundParameters parameters = new();

    public int lockLayerIndex = -1;

    GroundRaycastInfo[] raycastInfos = { };

    public int CurrentLayerIndex { get; private set; } = -1;
    public bool HasGroundPoint { get; private set; } = false;
    public int GroundPointLayerIndex { get; private set; } = -1;
    public Vector3 GroundPoint { get; private set; }
    public float GroundDistance { get; private set; }
    public bool IsGrounded { get; private set; }

    void UpdateHitInfos()
    {
        var x = transform.position.x;
        var y = transform.position.y;
        var z = transform.position.z;

        var layerMask = ~LayerMask.GetMask("MC");

        raycastInfos = layerZPositions
            .Select(layerZPosition =>
            {
                RaycastHit wallInfo = default;
                RaycastHit groundInfo = default;

                var zDelta = layerZPosition - z;
                var zDeltaAbs = Mathf.Abs(zDelta);
                var wallRayOrigin = new Vector3(x, y - parameters.pivotOffsetY, z);
                var wallRayDirection = zDelta > 0 ? Vector3.forward : Vector3.back;
                var wallRay = new Ray(wallRayOrigin, wallRayDirection);
                var wallHit = zDeltaAbs > 0.5f && Physics.Raycast(wallRay, out wallInfo, maxDistance: zDeltaAbs, layerMask, QueryTriggerInteraction.Ignore);

                var groundOrigin = new Vector3(x, y, layerZPosition);
                var groundRay = new Ray(groundOrigin, Vector3.down);
                var groundHit = wallHit == false && Physics.Raycast(groundRay, out groundInfo, parameters.rayMaxDistance + parameters.pivotOffsetY, layerMask, QueryTriggerInteraction.Ignore);

                return new GroundRaycastInfo(wallHit, wallRay, wallInfo, groundHit, groundRay, groundInfo);
            })
            .ToArray();
    }

    void UpdateNearestGroundPoint()
    {
        HasGroundPoint = false;
        for (int index = 0; index < layerZPositions.Length; index++)
        {
            var info = raycastInfos[index];
            if (info.groundHit)
            {
                if (HasGroundPoint == false)
                {
                    HasGroundPoint = true;
                    GroundPoint = info.groundInfo.point;
                    GroundPointLayerIndex = index;
                }
                else
                {
                    switch (MathUtils.Compare(GroundPoint.y, info.groundInfo.point.y))
                    {
                        case MathUtils.CompareResult.Equal:
                            // If the ground points are at the same height, choose the nearest one.
                            if (Mathf.Abs(GroundPoint.z - transform.position.z) > Mathf.Abs(info.groundInfo.point.z - transform.position.z))
                            {
                                GroundPoint = info.groundInfo.point;
                                GroundPointLayerIndex = index;
                            }
                            break;

                        case MathUtils.CompareResult.Less:
                            GroundPoint = info.groundInfo.point;
                            GroundPointLayerIndex = index;
                            break;
                    }
                }

                if (HasGroundPoint && lockLayerIndex == GroundPointLayerIndex && lockLayerIndex == index)
                    break; // Don't go further (to the next layer, to the background).
            }
        }
    }

    /// <summary>
    /// Tries to go down to the nearest foreground layer. Returns true if it's possible.
    /// </summary>
    public bool TryGetReachableForegroundLayerIndex(out int layerIndex)
    {
        layerIndex = -1;
        if (HasGroundPoint == false)
            // Currently in the air.
            return false;

        if (GroundPointLayerIndex == 0)
            // Already at the most foreground layer (0).
            return false;

        var previous = raycastInfos[GroundPointLayerIndex - 1];
        if (previous.groundHit == false)
            // No ground at the foreground layer.
            return false;

        layerIndex = GroundPointLayerIndex - 1;
        return true;
    }

    void Update()
    {
        CurrentLayerIndex = Mathf.FloorToInt(transform.position.z);
        UpdateHitInfos();
        UpdateNearestGroundPoint();

        if (HasGroundPoint)
        {
            GroundDistance = transform.position.y + parameters.pivotOffsetY - GroundPoint.y;
            IsGrounded = GroundDistance < parameters.maxDistance;
        }
        else
        {
            GroundDistance = float.PositiveInfinity;
            IsGrounded = false;
        }
    }

    void OnDrawGizmos()
    {
        foreach (var (wallHit, wallRay, wallInfo, groundHit, groundRay, groundInfo) in raycastInfos)
        {
            if (wallHit)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(wallRay.origin, 0.1f);
                Gizmos.DrawSphere(wallInfo.point, 0.05f);
                Gizmos.DrawLine(wallRay.origin, wallInfo.point);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(groundRay.origin, 0.1f);

                if (groundHit)
                {
                    Gizmos.DrawLine(groundRay.origin, groundInfo.point);
                    Gizmos.DrawSphere(groundInfo.point, 0.05f);
                }
                else
                {
                    Gizmos.DrawRay(groundRay.origin, groundRay.direction * (parameters.rayMaxDistance + parameters.pivotOffsetY));
                }
            }
        }

        if (HasGroundPoint)
        {
            Gizmos.color = Color.yellow;
            GizmosUtils.DrawCircle(GroundPoint, Vector3.up, 0.2f);
            GizmosUtils.DrawCircle(GroundPoint, Vector3.up, 0.3f);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Ground))]
    public class GroundEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var Target = (Ground)target;
            GUILayout.Label($"CurrentLayerIndex: {Target.CurrentLayerIndex}");
            GUILayout.Label($"HasGroundPoint: {Target.HasGroundPoint}");
            GUILayout.Label($"GroundPointLayerIndex: {Target.GroundPointLayerIndex}");
            GUILayout.Label($"GroundPoint: {Target.GroundPoint}");
        }
    }
#endif
}
