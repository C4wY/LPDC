using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways, Tooltip("Organizes the children of the game object by their name and adds a plane component to them. The plane component allows to define the \"depth\" of the associated plane.")]
public class LevelPlaneOrganizer : MonoBehaviour
{
    public class Plane : MonoBehaviour
    {
        [Range(-5, 5)]
        public int index = 0;
    }

    public float zOffset = -0.1f;

    void OnEnable()
    {
        if (Application.isPlaying)
        {
            Destroy(this);
        }
    }

    void Update()
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

#if UNITY_EDITOR
    [CustomEditor(typeof(LevelPlaneOrganizer))]
    public class AvatarEditor : Editor
    {
        LevelPlaneOrganizer Target =>
            (LevelPlaneOrganizer)target;

        public override void OnInspectorGUI()
        {
            var attributes = typeof(LevelPlaneOrganizer).GetCustomAttributes(typeof(TooltipAttribute), false)
                .Cast<TooltipAttribute>()
                .ToArray();

            if (attributes.Length > 0)
                EditorGUILayout.HelpBox(string.Join("\n", attributes.Select(a => a.tooltip)), MessageType.Info);

            base.OnInspectorGUI();
        }
#endif
    }
}
