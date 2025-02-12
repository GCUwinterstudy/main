using UnityEngine;
using UnityEngine.UI;

public class BGScrollUI : MonoBehaviour
{
    public RawImage rawImage;  // Inspector에서 할당
    public float scrollSpeed = 1f;

    void Update()
    {
        if (rawImage != null)
        {
            Rect uvRect = rawImage.uvRect;
            uvRect.x += Time.deltaTime * scrollSpeed;
            rawImage.uvRect = uvRect;
        }
    }
}