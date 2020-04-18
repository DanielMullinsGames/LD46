using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : MonoBehaviour
{
    [SerializeField]
    private List<SpriteRenderer> lights = default;

    [Header("Fade Params")]
    [SerializeField]
    private float speed = default;

    [SerializeField]
    private float minAlpha = default;

    [SerializeField]
    private float maxAlpha = default;

    [SerializeField]
    private float offsetTime = default;

    [SerializeField]
    private bool snapAlpha = false;

    [SerializeField]
    private float snapIncrement = 0f;

    private void Update()
    {
        float sine = (Mathf.Sin((Time.time * speed) + offsetTime) * 0.5f) + 0.5f;
        float alpha = minAlpha + ((maxAlpha - minAlpha) * sine);

        if (snapAlpha)
        {
            alpha = Mathf.Round(alpha / snapIncrement) * snapIncrement;
        }

        foreach (SpriteRenderer light in lights)
        {
            Color newColor = light.color;
            newColor.a = alpha;
            light.color = newColor;
        }
    }
}
