using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        // Store the original position of the camera
        Vector3 originalPOS = transform.localPosition;

        float elapsed = 0.0f;  // Corrected elapsed initialization to 0.0f

        while (elapsed < duration)
        {
            // Randomize the x and y positions for shake
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Apply the new shake position while keeping the z position fixed
            transform.localPosition = new Vector3(originalPOS.x + x, originalPOS.y + y, originalPOS.z);

            elapsed += Time.deltaTime;

            yield return null;  // Wait for the next frame
        }

        // After the shake is done, reset the camera's position to the original
        transform.localPosition = originalPOS;
    }
}