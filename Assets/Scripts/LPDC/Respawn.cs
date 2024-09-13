using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LPDC
{
    [ExecuteAlways]
    public class Respawn : MonoBehaviour
    {
        public Vector3 respawnPoint;

        List<CheckPoint> checkPoints = new();

        Avatar avatar;
        Avatar Avatar => avatar == null ? avatar = GetComponentInParent<Avatar>() : avatar;

        public void DoRespawn()
        {
            Avatar.Rigidbody.position = respawnPoint;
            Avatar.Rigidbody.velocity = Vector3.zero;
        }

        void TryToSaveCheckpoint(CheckPoint candidate)
        {
            foreach (var current in checkPoints)
            {
                if (current.checkPointOrder > candidate.checkPointOrder)
                    return;
            }

            checkPoints.Add(candidate);
            respawnPoint = candidate.transform.position;
        }

        void OnTriggerEnter(Collider other)
        {
            var checkpoint = other.GetComponentInParent<CheckPoint>();
            if (checkpoint != null)
                TryToSaveCheckpoint(checkpoint);
        }

        void OnEnable()
        {
            respawnPoint = transform.position;
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Respawn))]
        public class RespawnEditor : Editor
        {
            Respawn Target =>
                (Respawn)target;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Trigger Respawn"))
                    Target.DoRespawn();

            }
        }
#endif
    }
}
