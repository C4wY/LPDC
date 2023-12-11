using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
public class Grapplin : MonoBehaviour
{
    public float radius = 10;
    public float grabVelocity = 7.5f;
    public float grapplinDuration = 0.75f;
    public float grapplinDistance = 2f;
    public float grapplinForce = 100f;
    public Sprite ropeSprite;

    public Vector3 grabDirection = Vector3.right;

    bool isGrapplin = false;
    float grapplinStartTime = 0f;
    float grapplinStartDistance = 0f;
    Transform grapplinAnchor;
    SpringJoint joint;
    SpriteRenderer rope;

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
        grapplinStartTime = Time.time;
        GetComponent<Leader>().moveMode = Leader.MoveMode.NoFreeMove;

        grapplinAnchor = FirstCandidate().tr;
        grapplinStartDistance = (transform.position - grapplinAnchor.position).magnitude;
        // GetComponent<Rigidbody>().velocity = dir * grabVelocity;

        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.spring = grapplinForce;
        joint.connectedAnchor = grapplinAnchor.position;
        joint.maxDistance = grapplinStartDistance;
        joint.minDistance = grapplinStartDistance;


        var go = new GameObject("rope");
        rope = go.AddComponent<SpriteRenderer>();
        rope.sprite = ropeSprite;
        rope.drawMode = SpriteDrawMode.Tiled;
        rope.sortingOrder = 90;
    }

    void TryStopGrapplin()
    {
        if (isGrapplin == false)
            return;

        isGrapplin = false;
        Destroy(joint);
        Destroy(rope.gameObject);
        GetComponent<Leader>().moveMode = Leader.MoveMode.FreeMove;

    }

    void GrapplinUpdate()
    {
        var time = Time.time - grapplinStartTime;
        var alpha = Mathf.Clamp01(time / grapplinDuration);
        var distance = Mathf.Lerp(grapplinStartDistance, grapplinDistance, alpha);
        joint.maxDistance = distance;
        joint.minDistance = distance;

        rope.transform.position = (transform.position + grapplinAnchor.position) / 2;
        var v = grapplinAnchor.position - transform.position;
        rope.size = new Vector2(v.magnitude, 0.1f);
        var angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        rope.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        UpdateSnapPoints();

        if (Input.GetKeyDown(KeyCode.R))
            TryStartGrapplin();

        if (Input.GetKeyUp(KeyCode.R))
            TryStopGrapplin();

        if (isGrapplin)
            GrapplinUpdate();

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
