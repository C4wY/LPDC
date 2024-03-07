using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class FollowerControllerParameters
    {
        public float distanceToLeaderMin = 1.0f;
        public float distanceToLeaderMax = 6.0f;
    }

    public class FollowerController : MonoBehaviour
    {
        Player player, leaderPlayer;

        void FixedUpdate()
        {

        }
    }
}

