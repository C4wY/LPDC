using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Son : MonoBehaviour
{
    public void SonO ()
    {
        SceneManager.LoadSceneAsync(3);
    }
    
}