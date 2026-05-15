using UnityEngine;

public class AlienPartyBroadcastVisual : MonoBehaviour
{
    public LineRenderer line;

    public void Setup(
        Vector3 start,
        Vector3 atmospherePoint,
        Vector3 shiftedPoint,
        Vector3 alienPoint,
        Color c
    )
    {
        line.positionCount = 4;

        line.SetPosition(0, start);
        line.SetPosition(1, atmospherePoint);
        line.SetPosition(2, shiftedPoint);
        line.SetPosition(3, alienPoint);

        line.startColor = c;
        line.endColor = c;
    }
}