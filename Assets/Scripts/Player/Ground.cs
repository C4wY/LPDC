using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class Ground : MonoBehaviour
{
	public static float[] layerZs = { 0.5f, 1.5f, 2.5f };
	
	public float offsetY = -0.9f;
	public float maxDistance = 10;

	(Ray ray, bool hit, RaycastHit info)[] hitInfos = {};

	void Update() {
		var rigidbody = GetComponent<Rigidbody>();

		hitInfos = layerZs
			.Select(z => 
			{
				var x = rigidbody.position.x;
				var y = rigidbody.position.y;
				var origin = new Vector3(x, y + offsetY, z);
				var direction = Vector3.down;
				var ray = new Ray(origin, direction);

				var layerMask = ~LayerMask.GetMask("MC");

				if (Physics.Raycast(ray, out var info, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
				{
					return (ray, true, info);
				}
				else
				{
					return (ray, false, default);
				}
			})
			.ToArray();
	}

	void OnDrawGizmos()
	{
		foreach (var (ray, hit, info) in hitInfos)
		{
			if (hit)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(ray.origin, 0.1f);
				Gizmos.DrawLine(ray.origin, info.point);
				Gizmos.DrawSphere(info.point, 0.05f);
			}
			else
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(ray.origin, 0.1f);
				Gizmos.DrawRay(ray.origin, ray.direction * maxDistance);
			}
		}
	}
}
