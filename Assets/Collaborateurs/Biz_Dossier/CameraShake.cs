using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public CinemachineCameraOffset offset;
    public bool trigger;

    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;  // Corrected elapsed initialization to 0.0f

        while (elapsed < duration)
        {
            // Randomize the x and y positions for shake
            float x = Random.Range(-0.3f, 0.3f) * magnitude;
            float y = Random.Range(-0.3f, 0.3f) * magnitude;

            // Apply the new shake position while keeping the z position fixed
            offset.Offset = new Vector3(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null;  // Wait for the next frame
        }

        // After the shake is done, reset the camera's position to the original
        offset.Offset = new Vector3(0, 0, 0);
    }

    void Start()
    {
        offset = GetComponent<CinemachineCameraOffset>();
        if (offset == null)
        {
            Debug.LogWarning("Pas d'offset, pas de camshake");
        }
    }

    public void Trigger(float duration = 0.3f, float amplitude = 0.2F)
    {
        StartCoroutine(Shake(duration, amplitude));
    }

    void Update()
    {
        if (trigger)
        {
            trigger = false;
            Trigger();
        }
    }
}