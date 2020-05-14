using UnityEngine;

public class SnapToPixelGrid : MonoBehaviour
{
    private const float PIXELS_PER_UNIT = 100f;

    private void LateUpdate()
    {
        if (transform.parent != null)
        {
            Vector3 newLocalPosition = Vector3.zero;

            newLocalPosition.x = (Mathf.Round(transform.parent.position.x * PIXELS_PER_UNIT) / PIXELS_PER_UNIT) - transform.parent.position.x;
            newLocalPosition.y = (Mathf.Round(transform.parent.position.y * PIXELS_PER_UNIT) / PIXELS_PER_UNIT) - transform.parent.position.y;
            newLocalPosition.z = transform.localPosition.z;

            transform.localPosition = newLocalPosition;
        }
    }
}