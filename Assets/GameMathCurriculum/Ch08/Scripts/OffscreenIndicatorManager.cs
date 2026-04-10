using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicatorManager : MonoBehaviour
{
    public Camera mainCamera;

    public Transform[] targets;
    public RectTransform[] indicators;

    public float edgePadding = 50f;

    void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        int count = Mathf.Min(targets.Length, indicators.Length);
        for (int i = 0; i < count; i++)
        {
            if (targets[i] == null || indicators[i] == null) continue;
            UpdateIndicator(targets[i], indicators[i]);
        }
    }

    void UpdateIndicator(Transform target, RectTransform indicator)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        bool isBehind = screenPos.z < 0f;
        bool inBoundsX = screenPos.x >= 0f && screenPos.x <= Screen.width;
        bool inBoundsY = screenPos.y >= 0f && screenPos.y <= Screen.height;
        bool isOnScreen = !isBehind && inBoundsX && inBoundsY;

        if (isOnScreen)
        {
            indicator.gameObject.SetActive(false);
            return;
        }

        indicator.gameObject.SetActive(true);

        if (isBehind)
        {
            screenPos.x = Screen.width / 2 - screenPos.x;
            screenPos.y = Screen.height / 2 - screenPos.y;
        }

        float clampedX = Mathf.Clamp(screenPos.x, edgePadding, Screen.width - edgePadding);
        float clampedY = Mathf.Clamp(screenPos.y, edgePadding, Screen.height - edgePadding);
        indicator.position = new Vector2(clampedX, clampedY);

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 dir = new Vector2(screenPos.x, screenPos.y) - screenCenter;
        // Calculate angle in radians, then convert to degrees
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Apply rotation (subtract 90 if your sprite faces up instead of right)
        indicator.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}