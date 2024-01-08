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
        Step.Unit => 1,
        Step.Half => .5f,
        Step.Quarter => .25f,
        _ => throw new("Invalid step!"),
    };

    public Step step = Step.Unit;

    void SnapUpdate()
    {
        if (step != Step.Free)
        {
            ApplySnap();
        }
    }

    void ApplySnap()
    {
        var scalar = StepToFloat(step);

        var position = transform.localPosition / scalar;
        var scale = transform.localScale / scalar;

        var size = scale;
        var min = Vector3Int.RoundToInt(position - size / 2);
        var sizeInt = Vector3Int.Max(Vector3Int.RoundToInt(size), Vector3Int.one);
        var bounds = new BoundsInt(min, sizeInt);

        transform.localScale = (Vector3)bounds.size * scalar;
        transform.SetLocalPositionAndRotation(
            bounds.center * scalar,
            Quaternion.identity);
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
