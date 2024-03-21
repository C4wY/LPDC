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
        return Input.GetKeyDown(KeyCode.LeftShift);
    }
}
