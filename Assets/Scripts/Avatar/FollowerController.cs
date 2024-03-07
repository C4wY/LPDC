using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Avatar
{
    public class FollowerControllerParameters
    {
        public float distanceToLeaderMin = 1.0f;
        public float distanceToLeaderMax = 6.0f;
    }

    public class FollowerController : MonoBehaviour
    {
        Avatar player, leaderPlayer;

        void FixedUpdate()
        {

        }
    }
}

