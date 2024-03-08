using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collection : MonoBehaviour
{
    public void goCollection ()
    {
        SceneManager.LoadSceneAsync(4);
    }
}
