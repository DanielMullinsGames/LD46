using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class CoalDisplay : Singleton<CoalDisplay>
{
    [SerializeField]
    private TMPro.TextMeshProUGUI text;

    [SerializeField]
    private Color textColor;

    private void Start()
    {
        UpdateDisplay(flicker: false);
    }

    public void UpdateDisplay(bool flicker = true)
    {
        text.SetText("x" + RunState.coal.ToString());

        text.color = default;
        CustomCoroutine.WaitThenExecute(0.1f, () => text.color = textColor);
    }
}
