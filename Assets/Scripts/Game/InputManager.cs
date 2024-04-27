using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    void OnEnable()
    {
        Instance = this;
    }

    public bool CompetenceFront()
    {
        return Input.GetKey(KeyCode.E);
    }

    public bool TheSwitch()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }

    public bool DebugFollowerRespawn()
    {
        return Input.GetKey(KeyCode.U);
    }

    public bool DebugBothRespawn()
    {
        return DebugFollowerRespawn() || Input.GetKey(KeyCode.I);
    }
}
