using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controles : MonoBehaviour
{
    public void ShowControl ()
    {
        SceneManager.LoadSceneAsync(2);
    }
    
}