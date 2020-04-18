using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationProgressBar : UIBar
{
    private void Update()
    {
        ShowNormalizedValue(TrainProgressManager.Instance.DestinationProgress);
    }
}
