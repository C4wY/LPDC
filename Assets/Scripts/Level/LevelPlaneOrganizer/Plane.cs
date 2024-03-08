using UnityEngine;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Level.LevelPlaneOrganizer
{
    [ExecuteAlways]
    public class Plane : MonoBehaviour
    {
        [Range(-20, 40)]
        public int index = 0;

        SpriteRenderer spriteRenderer;

        void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Plane)), CanEditMultipleObjects]
        public class PlaneEditor : Editor
        {
            Plane Target =>
                (Plane)target;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                var renderers = Selection.gameObjects
                    .Select(go => go.GetComponent<SpriteRenderer>())
                    .Where(renderer => renderer != null)
                    .ToArray();

                EditorGUI.BeginChangeCheck();
                var cast = EditorGUILayout.Toggle("Sprite Casts Shadows", renderers[0].shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects(renderers, "Toggle Sprite Cast Shadows");
                    foreach (var r in renderers)
                        r.shadowCastingMode = cast
                            ? UnityEngine.Rendering.ShadowCastingMode.TwoSided
                            : UnityEngine.Rendering.ShadowCastingMode.Off;
                }

                // Do not work?
                // EditorGUI.BeginChangeCheck();
                // var receive = EditorGUILayout.Toggle("Sprite Receives Shadows", renderers[0].receiveShadows);
                // if (EditorGUI.EndChangeCheck())
                // {
                //     Undo.RecordObjects(renderers, "Toggle Sprite Receives Shadows");
                //     foreach (var r in renderers)
                //         r.receiveShadows = receive;
                // }
            }
        }
#endif
    }
}
