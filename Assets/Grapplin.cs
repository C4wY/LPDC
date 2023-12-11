using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
public class Grapplin : MonoBehaviour
{
    public float radius = 10;
    public float grabVelocity = 7.5f;

    public Vector3 grabDirection = Vector3.right;

    bool isGrapplin = false;

    Transform[] snapPoints = { };
    (Transform tr, float score, Vector3 dir)[] candidates = { };
    (Transform tr, float score, Vector3 dir) FirstCandidate() => candidates[0];

    void UpdateSnapPoints()
    {
        snapPoints = GameObject.FindGameObjectsWithTag("GPE")
            .Select(go => go.transform)
            .ToArray();

        candidates = snapPoints
            .Where(tr =>
            {
                var d = (transform.position - tr.position).magnitude;
                return d < radius;
            })
            .Select(tr =>
            {
                var currentDirection = (tr.position - transform.position).normalized;
                var score = Vector3.Dot(currentDirection, grabDirection);
                return (tr, score, currentDirection);
            })
            .OrderByDescending(entry => entry.score)
            .ToArray();
    }

    void TryStartGrapplin()
    {
        if (isGrapplin || candidates.Length == 0)
            return;

        isGrapplin = true;
        GetComponent<Leader>().moveMode = Leader.MoveMode.NoFreeMove;

        var (tr, _, dir) = FirstCandidate();
        GetComponent<Rigidbody>().velocity = dir * grabVelocity;

        var joint = gameObject.AddComponent<SpringJoint>();
        joint.anchor = tr.position;
        joint.maxDistance = (transform.position - tr.position).magnitude * 0.75f;
        joint.minDistance = (transform.position - tr.position).magnitude * 0.75f;
    }

    void Update()
    {
        UpdateSnapPoints();

        if (Input.GetKeyDown(KeyCode.R))
            TryStartGrapplin();

    }

    public Color gizmoColor = Color.blue;
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        foreach (var (tr, _, _) in candidates)
        {
            Gizmos.DrawSphere(tr.position, 0.2f);
        }

        Gizmos.DrawRay(transform.position, grabDirection * 2);

        if (candidates.Length > 0)
        {
            Gizmos.DrawWireSphere(FirstCandidate().tr.position, 0.3f);
        }
    }
}
