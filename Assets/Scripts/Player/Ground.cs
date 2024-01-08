using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class Ground : MonoBehaviour
{
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

    [Range(0, 1)]
    public float offsetY = 0.9f;
    public float maxDistance = 3;
    public bool forceDown;

    GroundRaycastInfo[] raycastInfos = { };

    int currentLayerIndex;
    bool hasGroundPoint;
    int groundPointLayerIndex;
    Vector3 groundPoint;

    public bool HasGroundPoint(out Vector3 groundPoint)
    {
        groundPoint = this.groundPoint;
        return hasGroundPoint;
    }

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
                var wallRayOrigin = new Vector3(x, y - offsetY, z);
                var wallRayDirection = zDelta > 0 ? Vector3.forward : Vector3.back;
                var wallRay = new Ray(wallRayOrigin, wallRayDirection);
                var wallHit = zDeltaAbs > 0.5f && Physics.Raycast(wallRay, out wallInfo, maxDistance: zDeltaAbs, layerMask, QueryTriggerInteraction.Ignore);

                var groundOrigin = new Vector3(x, y, layerZPosition);
                var groundRay = new Ray(groundOrigin, Vector3.down);
                var groundHit = wallHit == false && Physics.Raycast(groundRay, out groundInfo, maxDistance + offsetY, layerMask, QueryTriggerInteraction.Ignore);

                return new GroundRaycastInfo(wallHit, wallRay, wallInfo, groundHit, groundRay, groundInfo);
            })
            .ToArray();
    }

    void UpdateNearestGroundPoint()
    {
        hasGroundPoint = false;
        for (int index = 0; index < layerZPositions.Length; index++)
        {
            var info = raycastInfos[index];
            if (info.groundHit)
            {
                if (hasGroundPoint == false)
                {
                    hasGroundPoint = true;
                    groundPoint = info.groundInfo.point;
                    groundPointLayerIndex = index;
                }
                else
                {
                    if (groundPoint.y < info.groundInfo.point.y)
                    {
                        groundPoint = info.groundInfo.point;
                        groundPointLayerIndex = index;
                    }
                }
            }
        }

        if (Input.GetAxis("Vertical") < -0.1f || forceDown)
        {
            if (hasGroundPoint)
            {
                if (groundPointLayerIndex > 0 && Mathf.Abs(layerZPositions[groundPointLayerIndex] - transform.position.z) < 1.1f)
                {
                    var previous = raycastInfos[groundPointLayerIndex - 1];
                    if (previous.groundHit)
                    {
                        groundPoint = previous.groundInfo.point;
                    }
                }
            }
        }
    }

    void Update()
    {
        currentLayerIndex = Mathf.FloorToInt(transform.position.z);
        UpdateHitInfos();
        UpdateNearestGroundPoint();
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
                    Gizmos.DrawRay(groundRay.origin, groundRay.direction * (maxDistance + offsetY));
                }
            }
        }

        if (hasGroundPoint)
        {
            Gizmos.color = Color.yellow;
            GizmosUtils.DrawCircle(groundPoint, Vector3.up, 0.2f);
            GizmosUtils.DrawCircle(groundPoint, Vector3.up, 0.3f);
        }
    }
}
