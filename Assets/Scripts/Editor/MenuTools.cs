using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public static class MenuTools
{
    [MenuItem("LPDC/Resize UI Image")]
    public static void ResizeUIImage()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Select an image, connard.");
            return;
        }

        var ok = Selection.activeGameObject.TryGetComponent<Image>(out var currentImage);

        if (ok == false)
        {
            Debug.LogWarning("Select an image, connard.");
            return;
        }

        if (currentImage.sprite == null)
        {
            Debug.LogWarning("An image, ok, but no sprite in it, connard.");
            return;
        }

        var width = currentImage.sprite.rect.width;
        var height = currentImage.sprite.rect.height;
        // var ppu = currentImage.sprite.pixelsPerUnit;

        currentImage.rectTransform.sizeDelta = new(width, height);
    }
}
