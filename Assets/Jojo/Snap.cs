using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Snap : MonoBehaviour
{
    public enum Step 
    {
        Free,
        Unit,
        Half,
        Quarter,
    }

    public static float StepToFloat(Step step) => step switch
    {

    };

    public Step step = Step.Unit;

    void SnapUpdate()
    {
        var p = transform.localPosition;
        var s = transform.localScale;
        var min = Vector3Int.RoundToInt(p - s / 2);
        var max = Vector3Int.RoundToInt(p + Vector3.one * 0.01f + s / 2);
        var bounds = new BoundsInt(min, max - min);
        transform.localPosition = bounds.center;
        transform.localScale = bounds.size;
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying == false)
        {
            SnapUpdate();
        }
    }
}
