using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class UIPortraitPanel : MonoBehaviour
{
    public Transform healthLeader, healthFollower;
    public Sprite fullHeart, emptyHeart;

    void UpdateHealth(Transform healthTransform, LPDC.Avatar avatar)
    {
        if (avatar == null)
        {
            for (var i = 0; i < healthTransform.childCount; i++)
            {
                var heart = healthTransform.GetChild(i).gameObject;
                heart.GetComponent<Animator>().SetTrigger("HP-Lost");
            }
        }
        else
        {
            var pv = avatar.Santé.PV;
            for (var i = 0; i < healthTransform.childCount; i++)
            {
                var heart = healthTransform.GetChild(i).gameObject;
                var triggerName = i < pv ? "HP-Recover" : "HP-Lost";
                heart.GetComponent<Animator>().SetTrigger(triggerName);
            }
        }
    }

    void DisplayFirstPortraitImage()
    {
        var animator = GetComponentInChildren<Animator>();
        var clips = animator.runtimeAnimatorController.animationClips;
        if (clips.Length == 0) return;
        animator.Play(clips[0].name);
        animator.Update(0);
    }

    void Update()
    {
        var (leader, follower) = LPDC.Avatar.GetLeaderFollower();
        UpdateHealth(healthLeader, leader);
        UpdateHealth(healthFollower, follower);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UIPortraitPanel))]
    public class UIPortraitPanelEditor : Editor
    {
        void DebugAndTest()
        {
            GUILayout.Space(10);
            GUILayout.Label("Debug / Test", EditorStyles.boldLabel);

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

            if (GUILayout.Button("Display First Portrait Image"))
            {
                var panel = (UIPortraitPanel)target;
                panel.DisplayFirstPortraitImage();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DebugAndTest();
        }
    }
#endif
}
