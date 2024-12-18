using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeUtils
{
    public static IEnumerator WaitUnscaledTime(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null; // Wait until the next frame
        }
    }

}
