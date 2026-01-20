using UnityEngine;

public class UITrackCameraPerspective : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform target; // UI group to move
    [SerializeField] private Canvas canvas;        // parent canvas
    [SerializeField] private float depth = 10f;    // "as if UI sits this far in front of the camera"
    [SerializeField] private bool invert = false;  // toggle if direction feels opposite
    [SerializeField] private bool ignoreZCameraMove = true; // usually yes for 2D side/top cams

    private Vector3 startCamPos;
    private Vector2 startAnchoredPos;
    private Vector2 startScreenPos;

    void Reset()
    {
        cam = Camera.main;
        target = transform as RectTransform;
        canvas = GetComponentInParent<Canvas>();
    }

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (!target) target = transform as RectTransform;
        if (!canvas) canvas = GetComponentInParent<Canvas>();

        startCamPos = cam.transform.position;
        startAnchoredPos = target.anchoredPosition;
        startScreenPos = ScreenPosAtDepth();
    }

    void LateUpdate()
    {
        if (!cam || !target || !canvas) return;

        Vector2 nowScreen = ScreenPosAtDepth();
        Vector2 deltaPixels = nowScreen - startScreenPos;

        if (invert) deltaPixels = -deltaPixels;

        // Convert screen pixels into canvas units (important if you use Canvas Scaler)
        float sf = canvas.scaleFactor <= 0f ? 1f : canvas.scaleFactor;
        target.anchoredPosition = startAnchoredPos + (deltaPixels / sf);
    }

    private Vector2 ScreenPosAtDepth()
    {
        // Anchor a reference point in front of the camera
        Vector3 camPos = cam.transform.position;

        if (ignoreZCameraMove)
            camPos.z = startCamPos.z;

        Vector3 worldPoint = camPos + cam.transform.forward * depth;
        Vector3 screen = cam.WorldToScreenPoint(worldPoint);
        return new Vector2(screen.x, screen.y);
    }
}
