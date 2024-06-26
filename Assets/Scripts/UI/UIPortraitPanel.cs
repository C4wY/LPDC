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

    void UpdateHealth(Transform healthTransform, Avatar.Avatar avatar)
    {
        var pv = avatar.Santé.PV;
        for (var i = 0; i < healthTransform.childCount; i++)
        {
            var heart = healthTransform.GetChild(i).gameObject;
            heart.GetComponent<Image>().sprite = i < pv ? fullHeart : emptyHeart;
        }
    }

    void DisplayFirstPortraitImage()
    {
        var animator = GetComponentInChildren<Animator>();
        var clips = animator.runtimeAnimatorController.animationClips;
        animator.Play(clips[0].name);
        animator.Update(0);
    }

    void Update()
    {
        if (!Application.isPlaying) DisplayFirstPortraitImage();

        var (leader, follower) = Avatar.Avatar.GetLeaderFollower();
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
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DebugAndTest();
        }
    }
#endif
}
