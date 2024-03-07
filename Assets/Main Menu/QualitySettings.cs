using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetQualityLevel : MonoBehaviour
{
    public void LowQ()
    {
        QualitySettings.SetQualityLevel(0, true); //Fastest Quality
    }
    public void MedQ()
    {
        QualitySettings.SetQualityLevel(2, true); //Simple Graphics
    }
    public void HighQ()
    {
        QualitySettings.SetQualityLevel(5, true); //Fantastic Graphics
    }


    void NotHappening()
    {
        //use these for later if you want

        QualitySettings.SetQualityLevel(1, true); //Fast Quality

        QualitySettings.SetQualityLevel(3, true); //Good Graphics

        QualitySettings.SetQualityLevel(4, true); //Beautiful Graphics

    }

}