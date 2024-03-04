using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class jnc_UISprite : MonoBehaviour
{
    [Range(0, 1)]
    public float overrideAlpha = 0;

    public Color overrideColor = Color.white;

    void OnEnable()
    {
        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            spriteRenderer.material = new Material(spriteRenderer.material);
        }

        if (TryGetComponent<Image>(out var image))
        {
            image.material = new Material(image.material);
        }
    }

    void Update()
    {
        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            spriteRenderer.material.SetColor("_OverrideColor", overrideColor);
            spriteRenderer.material.SetFloat("_OverrideAlpha", overrideAlpha);
        }

        if (TryGetComponent<Image>(out var image))
        {
            image.material.SetColor("_OverrideColor", overrideColor);
            image.material.SetFloat("_OverrideAlpha", overrideAlpha);
        }
    }
}
