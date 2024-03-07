using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ToggleMeshRenderer : MonoBehaviour
{
    public enum Toggle
    {
        Ignore,
        Enable,
        Disable,
    }

    public Toggle toggleOnPlay = Toggle.Disable;

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
            if (toggleOnPlay != Toggle.Ignore)
                ToggleMeshRenderers(toggleOnPlay == Toggle.Enable);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ToggleMeshRenderer))]
    public class AvatarEditor : Editor
    {
        ToggleMeshRenderer Target =>
            (ToggleMeshRenderer)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Toggle MeshRenderers"))
                Target.ToggleMeshRenderers();
        }
    }
#endif
}
