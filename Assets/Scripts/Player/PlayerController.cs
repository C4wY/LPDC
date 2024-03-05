using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerParameters
    {
        public float jumpHeight = 1.33f;
        public float speed = 5;

        [Tooltip("The time in seconds to wait before being able to go backward again (backward in Unity, is going foreground in a theater).")]
        public float goBackwardCooldown = 0.33f;
    }

    public PlayerParameters parameters = new();

    public bool forceDown;

    new Rigidbody rigidbody;
    Ground ground;

    public float GoBackwardTime { get; private set; } = -1;

    /// <summary>
    /// Returns the velocity required to reach the jump height.
    /// </summary>
    public float GetJumpVelocityY()
    {
        return Mathf.Sqrt(Physics.gravity.magnitude * 2f * parameters.jumpHeight);
    }

    void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody>();
        ground = GetComponent<Ground>();
    }

    void GoDownUpdate()
    {
        var cooldown = Time.time < GoBackwardTime + parameters.goBackwardCooldown;

        if (cooldown == false)
        {
            ground.lockLayerIndex = -1;

            var wants = forceDown || Input.GetAxis("Vertical") < -0.1f;
            if (wants && ground.TryGetReachableForegroundLayerIndex(out var layerIndex))
            {
                GoBackwardTime = Time.time;
                ground.lockLayerIndex = layerIndex; // Lock the layer to avoid going foreground.
            }
        }
    }

    void JumpUpdate()
    {
        if (Input.GetButtonDown("Jump"))
        {
            var velocity = rigidbody.velocity;
            velocity.y = GetJumpVelocityY();
            rigidbody.velocity = velocity;
        }
    }

    void Update()
    {
        JumpUpdate();
        GoDownUpdate();
    }

    void VelocityUpdate()
    {
        var x = Input.GetAxis("Horizontal") * parameters.speed;
        var y = rigidbody.velocity.y;
        rigidbody.velocity = new(x, y, 0);
    }

    void UpdateGroundPoint()
    {
        if (ground.HasGroundPoint)
        {
            var (x, y, z) = rigidbody.position;
            z = Mathf.Lerp(z, ground.GroundPoint.z, 0.33f);
            rigidbody.position = new(x, y, z);
        }
    }

    void FixedUpdate()
    {
        VelocityUpdate();
        UpdateGroundPoint();
    }
}
