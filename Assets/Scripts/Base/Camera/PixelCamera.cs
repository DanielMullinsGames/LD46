using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PixelCamera : MonoBehaviour 
{
    [SerializeField]
    private int heightMin = 450;

    [SerializeField]
    private int heightMax = 550;

    private const float MIN_ASPECT_RATIO = 1.75f;

	public const int PIXELS_PER_UNIT = 100;

	private int renderWidth;
	private int renderHeight;
	private int actualWidth;
	private int actualHeight;
	
	private Camera cam;

	void Awake ()
    {
		cam = GetComponent<Camera>();
	}

	void Update()
    {
        int referenceHeight = GetReferenceHeight();
        renderHeight = MakeEven(referenceHeight);

		cam.orthographicSize = (renderHeight / 2) / (float)PIXELS_PER_UNIT;

        int scale = Mathf.Max(1, Mathf.Max(Screen.height / renderHeight, 1));
        actualHeight = (int)(renderHeight * scale);
		
		renderWidth = (int)(Screen.width / scale);
        renderWidth = MakeEven(renderWidth);

		actualWidth = (int)(renderWidth * scale);

		Rect rect = cam.rect;
		rect.width = (float)actualWidth / Screen.width;
		rect.height = (float)actualHeight / Screen.height;
		rect.x = (1 - rect.width) / 2;
		rect.y = (1 - rect.height) / 2;
		cam.rect = rect;
	}

    private int GetReferenceHeight()
    {
        int height = Screen.height;

        // Limit height to prevent an aspect ratio smaller than the min
        float aspectRatio = Screen.width / (float)Screen.height;
        if (aspectRatio < MIN_ASPECT_RATIO)
        {
            height = Mathf.RoundToInt(Screen.width / MIN_ASPECT_RATIO);
        }

        // Downscale height
        while (height > heightMax)
        {
            height = height / 2;
        }

        // Clamp height between the min and max
        int clampedHeight = Mathf.Clamp(height, heightMin, heightMax);

        // If we aren't downscaling at all, choose the max of the screen height or max reference height 
        int scale = Mathf.Max(Screen.height / clampedHeight, 1);
        if (scale == 1)
        {
            clampedHeight = Mathf.Min(heightMax, Screen.height);
        }

        // Make height even
        int evenHeight = MakeEven(clampedHeight);

        return evenHeight;
    }

    private int MakeEven(int num)
    {
        return num - (num % 2 != 0 ? 1 : 0);
    }
	
	void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (renderWidth > 0 && renderHeight > 0)
        {
            RenderTexture buffer = RenderTexture.GetTemporary(renderWidth, renderHeight, -1);

            buffer.filterMode = FilterMode.Point;
            source.filterMode = FilterMode.Point;

            Graphics.Blit(source, buffer);
            Graphics.Blit(buffer, destination);

            RenderTexture.ReleaseTemporary(buffer);
        }
	}
}