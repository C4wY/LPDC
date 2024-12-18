using System.Collections;
using System.Collections.Generic;
using LPDC;
using UnityEngine;

public class DebugCheat : MonoBehaviour
{
    public static DebugCheat Instance { get; private set; } = null;

    public static void EnsureSpawn()
    {
        if (Instance == null)
        {
            Instantiate(MainSettings.Instance.cheatScreen);
        }
    }

    public static void EnsureDestroy()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
    }

    public static void Toggle()
    {
        if (Instance == null)
        {
            EnsureSpawn();
        }
        else
        {
            EnsureDestroy();
        }
    }

    public void GameOver()
    {
        Instantiate(MainSettings.Instance.gameOverScreen);
        EnsureDestroy();
    }

    void OnEnable()
    {
        Instance = this;
    }

    void Start()
    {
        foreach (var child in SceneUtils.AllGameObjectsInScene())
        {
            if (child.name == "Player")
            {
                Debug.Log("Player found!");
            }
        }
    }
}
