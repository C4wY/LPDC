using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Level
{
    [ExecuteAlways]
    public class LevelBlockHandler : MonoBehaviour
    {
        public enum Toggle
        {
            Ignore,
            Visible,
            Hidden,
        }

        public Toggle onPlay = Toggle.Hidden;
        public bool blockAreVisible = true;
        public bool backFaceAreVisible = true;

        Material blockMaterial;

        void UpdateBlocks()
        {
            if (blockMaterial == null)
                blockMaterial = Resources.Load<Material>("Materials/jnc_LevelBlock");

            blockMaterial.SetFloat("_HideBackFace", backFaceAreVisible ? 0 : 1);

            var blocks = GetComponentsInChildren<MeshRenderer>();
            var layer = LayerMask.NameToLayer("LevelBlock");
            foreach (var block in blocks)
            {
                block.enabled = blockAreVisible;
                block.material = blockMaterial;
                block.receiveShadows = false;
                block.gameObject.layer = layer;
            }
        }

        void ToggleMeshRenderers(bool? enabled = null)
        {
            foreach (var mr in GetComponentsInChildren<MeshRenderer>())
            {
                enabled ??= !mr.enabled;
                mr.enabled = enabled.Value;
            }
        }

        void OnEnable()
        {
            if (Application.isPlaying)
            {
                if (onPlay != Toggle.Ignore)
                {
                    blockAreVisible = onPlay == Toggle.Visible;
                    UpdateBlocks();
                }
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            EditorApplication.delayCall += () =>
            {
                if (this != null)
                    UpdateBlocks();
            };
        }
#endif
    }
}
