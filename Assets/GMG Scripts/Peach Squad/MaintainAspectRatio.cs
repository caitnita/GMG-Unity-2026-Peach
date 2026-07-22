using UnityEngine;

public class MaintainAspectRatio : MonoBehaviour
{
    void Start()
    {
        // Set your target resolution
        float targetWidth = 1920.0f;
        float targetHeight = 1080.0f;
        float targetAspect = targetWidth / targetHeight;

        // Determine the game window's current aspect ratio
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera camera = GetComponent<Camera>();

        // If the window is taller/wider than the target aspect ratio, scale the viewport
        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            camera.rect = rect;
        }
        else // Add pillarbox
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = camera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            camera.rect = rect;
        }
    }
}