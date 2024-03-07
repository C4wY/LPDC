using UnityEngine;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ToggleMeshRenderer : MonoBehaviour
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ToggleMeshRenderer))]
    public class AvatarEditor : Editor
    {
        ToggleMeshRenderer Target =>
            (ToggleMeshRenderer)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Toggle MeshRenderer"))
            {
                bool? enabled = null;
                foreach (var mr in target.GetComponentsInChildren<MeshRenderer>())
                {
                    enabled ??= !mr.enabled;
                    mr.enabled = enabled.Value;
                }
            }
        }
    }
#endif
}
