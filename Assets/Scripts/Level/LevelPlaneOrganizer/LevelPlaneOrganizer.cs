using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Level.LevelPlaneOrganizer
{
    [ExecuteAlways, Tooltip("Positions the children according to their index. The index is defined on a component automatically added to the children.")]
    public class LevelPlaneOrganizer : MonoBehaviour
    {
        [Tooltip("Global z offset for preventing z-fighting.")]
        public float zOffset = -0.1f;

        void ToggleSpriteRenderers(bool? enabled = null)
        {
            foreach (var mr in GetComponentsInChildren<SpriteRenderer>())
            {
                enabled ??= !mr.enabled;
                mr.enabled = enabled.Value;
            }
        }

        bool FirstSpriteRendererEnabled()
        {
            var mr = GetComponentInChildren<SpriteRenderer>();

            if (mr != null)
                return mr.enabled;

            return false;
        }

        void Setup()
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<Plane>(out var plane) == false)
                {
                    plane = child.gameObject.AddComponent<Plane>();
                    var regex = new Regex(@"\d+$");
                    var match = regex.Match(child.name);
                    if (match.Success)
                        plane.index = int.Parse(match.Value);
                }

                var (x, y, _) = child.position;
                child.position = new Vector3(x, y, plane.index + zOffset);
            }
        }

        void OnEnable()
        {
            if (Application.isPlaying)
            {
                ToggleSpriteRenderers(true);
                enabled = false;
            }
        }

        void Update()
        {
            Setup();
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(LevelPlaneOrganizer))]
        public class AvatarEditor : Editor
        {
            LevelPlaneOrganizer Target =>
                (LevelPlaneOrganizer)target;

            void ToggleVisible()
            {
                EditorGUI.BeginChangeCheck();
                var visible = EditorGUILayout.Toggle("Sprite Visible", Target.FirstSpriteRendererEnabled());
                if (EditorGUI.EndChangeCheck())
                {
                    var mrs = Target.GetComponentsInChildren<MeshRenderer>();
                    Undo.RecordObjects(mrs, "Toggle Sprite Renderers");
                    Target.ToggleSpriteRenderers(visible);
                }
            }

            public override void OnInspectorGUI()
            {
                var attributes = typeof(LevelPlaneOrganizer).GetCustomAttributes(typeof(TooltipAttribute), false)
                    .Cast<TooltipAttribute>()
                    .ToArray();

                if (attributes.Length > 0)
                    EditorGUILayout.HelpBox(string.Join("\n", attributes.Select(a => a.tooltip)), MessageType.Info);

                base.OnInspectorGUI();

                ToggleVisible();

                if (GUILayout.Button("Select Sprites"))
                    Selection.objects = Target.GetComponentsInChildren<SpriteRenderer>().Select(mr => mr.gameObject).ToArray();
            }
#endif
        }
    }
}
