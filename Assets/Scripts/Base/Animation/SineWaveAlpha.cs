using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineWaveAlpha : MonoBehaviour
{
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

    void Update()
    {
        float sine = (Mathf.Sin((Time.time * speed) + offsetTime) * 0.5f) + 0.5f;
        float alpha = minAlpha + ((maxAlpha - minAlpha) * sine);

        if (snapAlpha)
        {
            alpha = Mathf.Round(alpha / snapIncrement) * snapIncrement;
        }

        Color newColor = GetComponent<Renderer>().material.color;
        newColor.a = alpha;
        GetComponent<Renderer>().material.color = newColor;
    }
}
