using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelBlockHandler : MonoBehaviour
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
            mr.receiveShadows = false;
        }
    }

    bool FirstMeshRendererEnabled()
    {
        var mr = GetComponentInChildren<MeshRenderer>();

        if (mr != null)
            return mr.enabled;

        return false;
    }

    void OnEnable()
    {
        if (Application.isPlaying)
        {
            if (toggleOnPlay != Toggle.Ignore)
                ToggleMeshRenderers(toggleOnPlay == Toggle.Enable);

            enabled = false;
        }
    }

    void Update()
    {

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LevelBlockHandler))]
    public class AvatarEditor : Editor
    {
        LevelBlockHandler Target =>
            (LevelBlockHandler)target;

        void ToggleVisible()
        {
            EditorGUI.BeginChangeCheck();
            var visible = EditorGUILayout.Toggle("Mesh Visible", Target.FirstMeshRendererEnabled());
            if (EditorGUI.EndChangeCheck())
            {
                var mrs = Target.GetComponentsInChildren<MeshRenderer>();
                Undo.RecordObjects(mrs, "Toggle Mesh Renderers");
                Target.ToggleMeshRenderers(visible);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ToggleVisible();
        }
    }
#endif
}
