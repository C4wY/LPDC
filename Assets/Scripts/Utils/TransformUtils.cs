using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformUtils
{
    /// <summary>
    /// Swap the RectTransform properties of two RectTransforms.
    /// 
    /// The RectTransforms must have the same parent (otherwise an ArgumentException is thrown). 
    /// </summary>
    public static void SwapRectTransform(RectTransform a, RectTransform b)
    {
        if (a.parent != b.parent)
            throw new System.ArgumentException("The RectTransforms must have the same parent.");

        (a.anchorMax, b.anchorMax) = (b.anchorMax, a.anchorMax);
        (a.anchorMin, b.anchorMin) = (b.anchorMin, a.anchorMin);
        (a.pivot, b.pivot) = (b.pivot, a.pivot);
        (a.sizeDelta, b.sizeDelta) = (b.sizeDelta, a.sizeDelta);
        (a.localPosition, b.localPosition) = (b.localPosition, a.localPosition);
        (a.localRotation, b.localRotation) = (b.localRotation, a.localRotation);
        (a.localScale, b.localScale) = (b.localScale, a.localScale);
    }

    /// <summary>
    /// Swap the RectTransform properties of two RectTransforms.
    /// 
    /// The RectTransforms must have the same parent (otherwise an ArgumentException is thrown). 
    /// </summary>
    public static void SwapRectTransform(Transform a, Transform b) =>
        SwapRectTransform(a as RectTransform, b as RectTransform);
}
