using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class UIPortraitPanel : MonoBehaviour
{
    void Update()
    {
        var animator = GetComponentInChildren<Animator>();
        var clips = animator.runtimeAnimatorController.animationClips;
        animator.Play(clips[0].name);
        animator.Update(0);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UIPortraitPanel))]
    public class UIPortraitPanelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Play transition Sora > Dooms"))
            {
                var panel = (UIPortraitPanel)target;
                var animator = panel.GetComponentInChildren<Animator>();
                animator.SetTrigger("Sora > Dooms");
            }

            if (GUILayout.Button("Play transition Dooms > Sora"))
            {
                var panel = (UIPortraitPanel)target;
                var animator = panel.GetComponentInChildren<Animator>();
                animator.SetTrigger("Dooms > Sora");
            }
        }
    }
#endif
}
