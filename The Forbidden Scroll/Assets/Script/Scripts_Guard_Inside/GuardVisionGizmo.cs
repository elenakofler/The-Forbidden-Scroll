using UnityEngine;

public class GuardVisionGizmo : MonoBehaviour
{
    public float viewDistance = 12.0f;
    public float viewAngle = 90.0f;

    private void OnDrawGizmos()
    {
        //1. Red circle for showing the maximum visibility range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.0f, viewDistance);

        //2. Light blue lines for the precise viewing angle
        Gizmos.color = Color.cyan;
        Vector3 forward = transform.forward;

        Quaternion leftRayRotation = Quaternion.AngleAxis(-viewAngle / 2f, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(viewAngle / 2f, Vector3.up);

        Vector3 leftDir = leftRayRotation * forward;
        Vector3 rightDir = rightRayRotation * forward;

        Vector3 startPos = transform.position + Vector3.up * 1.0f;
        Gizmos.DrawRay(startPos, leftDir * viewDistance);
        Gizmos.DrawRay(startPos, rightDir * viewDistance);
    }
}