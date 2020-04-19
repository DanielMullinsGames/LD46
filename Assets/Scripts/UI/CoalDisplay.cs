using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class CoalDisplay : Singleton<CoalDisplay>
{
    [SerializeField]
    private TMPro.TextMeshProUGUI text;

    private Vector3 basePos;

    private void Start()
    {
        basePos = transform.localPosition;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        text.SetText("x" + RunState.coal.ToString());

        Tween.LocalPosition(transform, basePos + (Vector3.up * 0.03f), 0.1f, 0f, Tween.EaseOut);
        Tween.LocalPosition(transform, basePos, 0.1f, 0.15f, Tween.EaseIn);
    }
}
