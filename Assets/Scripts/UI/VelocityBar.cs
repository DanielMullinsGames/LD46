using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityBar : UIBar
{
    private void Update()
    {
        ShowNormalizedValue(TrainProgressManager.Instance.NormalizedVelocity);
    }
}
