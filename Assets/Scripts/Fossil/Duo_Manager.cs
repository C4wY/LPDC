using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

namespace Fossils
{
    public class Duo_Manager : MonoBehaviour
    {
        public enum AvatarID
        {
            Sora,
            Doom,
        }
        public AvatarID currentAvatarID = AvatarID.Sora;
        public int baseSortingOrder = 100;

        Transform sora, doom;
        Transform leader, follower;

        new Camera camera;
        Vector3 cameraOffset;

        void OnEnable()
        {
            sora = transform.Find("Sora");
            doom = transform.Find("Doom");

            sora.GetComponent<Follower>().target = doom;
            doom.GetComponent<Follower>().target = sora;

            camera = GetComponentInChildren<Camera>();
            cameraOffset = camera.transform.localPosition;

            UpdateAvatars();
        }

        void UpdateAvatars()
        {
            switch (currentAvatarID)
            {
                case AvatarID.Sora:
                    ActiveAvatar(sora);
                    UnActiveAvatar(doom);
                    leader = sora;
                    follower = doom;
                    break;

                case AvatarID.Doom:
                    ActiveAvatar(doom);
                    UnActiveAvatar(sora);
                    leader = doom;
                    follower = sora;
                    break;
            }

        }

        void ActiveAvatar(Transform avatar)
        {
            var sr = avatar.GetComponentInChildren<SpriteRenderer>();
            sr.sortingOrder = baseSortingOrder + 1;
            sr.color = Color.white;

            avatar.GetComponent<Leader>().enabled = true;
            avatar.GetComponent<Follower>().enabled = false;
        }

        void UnActiveAvatar(Transform avatar)
        {
            var sr = avatar.GetComponentInChildren<SpriteRenderer>();
            sr.sortingOrder = baseSortingOrder;
            sr.color = new Color(0.5f, 0.5f, 0.5f);

            avatar.GetComponent<Leader>().enabled = false;
            avatar.GetComponent<Follower>().enabled = true;
        }

        void UpdateCamera()
        {
            camera.transform.position += (leader.position + cameraOffset - camera.transform.position) * (1f - Mathf.Pow(0.01f, Time.deltaTime));
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                currentAvatarID = currentAvatarID == AvatarID.Sora
                    ? AvatarID.Doom
                    : AvatarID.Sora;

                UpdateAvatars();
            }
        }

        void LateUpdate()
        {
            UpdateCamera();
        }
    }
}
