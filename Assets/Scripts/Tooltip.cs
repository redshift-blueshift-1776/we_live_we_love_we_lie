using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private RectTransform backgroundRect;
    private Vector2 cursorOffset = new Vector2(160f, 40f);

    private void Awake()
    {
        Instance = this;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        gameObject.SetActive(false);
    }

    public void ShowTooltip(string text)
    {
        gameObject.SetActive(true);
        tooltipText.text = text;

        tooltipText.enableAutoSizing = true;
        tooltipText.fontSizeMin = 10f;
        tooltipText.fontSizeMax = 72f;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.transform as RectTransform;
        RectTransform tooltipRect = GetComponent<RectTransform>();

        // Convert mouse position to canvas-local coordinates
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localMousePos
        );

        Vector2 tooltipSize = tooltipRect.sizeDelta;
        Vector2 canvasSize = canvasRect.sizeDelta;

        // Default pivot: bottom-left
        Vector2 pivot = new Vector2(0f, 0f);

        // Check right edge
        if (localMousePos.x + cursorOffset.x + tooltipSize.x > canvasSize.x / 2f)
        {
            pivot.x = 1f;
        }

        // Check top edge
        if (localMousePos.y + cursorOffset.y + tooltipSize.y > canvasSize.y / 2f)
        {
            pivot.y = 1f;
        }

        // Check left edge
        if (localMousePos.x - cursorOffset.x < -canvasSize.x / 2f)
        {
            pivot.x = 0f;
        }

        // Check bottom edge
        if (localMousePos.y - cursorOffset.y < -canvasSize.y / 2f)
        {
            pivot.y = 0f;
        }

        tooltipRect.pivot = pivot;

        // Simple offset based on pivot
        Vector2 finalOffset = new Vector2(
            pivot.x == 0f ? cursorOffset.x : -cursorOffset.x,
            pivot.y == 0f ? cursorOffset.y : -cursorOffset.y - 30f
        );

        transform.localPosition = localMousePos + finalOffset;
    }
}