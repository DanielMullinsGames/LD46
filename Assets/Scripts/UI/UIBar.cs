using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBar : MonoBehaviour
{
    [SerializeField]
    private Transform arrow;

    [SerializeField]
    private Transform leftEndMarker;

    [SerializeField]
    private Transform rightEndMarker;

    protected void ShowNormalizedValue(float value)
    {
        float x = leftEndMarker.localPosition.x + ((rightEndMarker.localPosition.x - leftEndMarker.localPosition.x) * value);
        x = Mathf.Clamp(x, leftEndMarker.localPosition.x, rightEndMarker.localPosition.x);
        arrow.transform.localPosition = new Vector2(x, arrow.transform.localPosition.y);
    }
}
