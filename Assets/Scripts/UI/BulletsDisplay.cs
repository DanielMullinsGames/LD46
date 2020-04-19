using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class BulletsDisplay : Singleton<BulletsDisplay>
{
    [SerializeField]
    private List<SpriteRenderer> bulletIcons;

    private Vector3 basePos;

    private void Start()
    {
        basePos = transform.localPosition;
        UpdateDisplay(tween: false);
    }

    public void UpdateDisplay(bool tween = true)
    {
        for (int i = 0; i < bulletIcons.Count; i++)
        {
            bulletIcons[i].color = new Color(1f, 1f, 1f, i + 1 > RunState.bullets ? 0.33f : 1f);
        }

        if (tween)
        {
            Tween.LocalPosition(transform, basePos + (Vector3.up * 0.03f), 0.1f, 0f, Tween.EaseOut);
            Tween.LocalPosition(transform, basePos, 0.1f, 0.15f, Tween.EaseIn);
        }
    }
}
