using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unj_ChangementQualite : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeQuality(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeQuality(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeQuality(2);
        }
    }

    public void ChangeQuality(int qualityIndex)
    {
        Debug.Log("La qualité a changé en : " + qualityIndex.ToString());
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
