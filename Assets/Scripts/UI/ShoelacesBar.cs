using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoelacesBar : UIBar
{
    private void Update()
    {
        ShowNormalizedValue(ShoelacesStation.Instance.TieProgress);
    }
}
