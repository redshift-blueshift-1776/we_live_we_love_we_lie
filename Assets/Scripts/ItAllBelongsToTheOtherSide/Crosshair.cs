using UnityEngine;
using UnityEngine.UI;


public class Crosshair : MonoBehaviour
{
    public PlayerMovement7 playerMovement;

    public Image x1;
    public Image x2;
    public Image y1;
    public Image y2;

    private Vector3 x1OriginalPos;
    private Vector3 x2OriginalPos;
    private Vector3 y1OriginalPos;
    private Vector3 y2OriginalPos;

    private float crosshairGapDistanceGrowth = 30f;
    void Start()
    {
        x1OriginalPos = x1.rectTransform.localPosition;
        x2OriginalPos = x2.rectTransform.localPosition;
        y1OriginalPos = y1.rectTransform.localPosition;
        y2OriginalPos = y2.rectTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = playerMovement.getVelocity();
        float speed = velocity.magnitude;

        if (speed < 0.1f)
        {
            updateCrosshair(0);
            return;
        }
        speed = Mathf.Min(0.9f * playerMovement.getCurrentMaxSpeed(), speed);
        float displacement = speed * crosshairGapDistanceGrowth;

        updateCrosshair(displacement);
    }

    private void updateCrosshair(float displacement)
    {
        x1.rectTransform.localPosition = x1OriginalPos - new Vector3(displacement, 0, 0);
        x2.rectTransform.localPosition = x2OriginalPos + new Vector3(displacement, 0, 0);
        y1.rectTransform.localPosition = y1OriginalPos - new Vector3(0, displacement, 0);
        y2.rectTransform.localPosition = y2OriginalPos + new Vector3(0, displacement, 0);
    }
}
