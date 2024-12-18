using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAvatarPortraitHandler : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.GetInstanceOrThrow().dualPortraitEvent.AddListener(OnDualPortraitEvent);
    }

    void OnDisable()
    {
        if (GameManager.TryGetInstance(out var gameManager))
            gameManager.dualPortraitEvent.RemoveListener(OnDualPortraitEvent);
    }

    void OnDualPortraitEvent(DualPortraitEventProps arg0)
    {
        Debug.Log($"Main avatar: {arg0.mainAvatar}");
    }
}
