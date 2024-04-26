using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    internal static object GetInstance()
    {
        throw new NotImplementedException();
    }
}
