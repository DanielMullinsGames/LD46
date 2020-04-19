using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene2Sequencer : MonoBehaviour
{
    private bool firstInclineStarted;
    private bool firstInclineEnded;
    private bool secondInclineStarted;
    private bool secondInclineEnded;
    private bool declineStarted;
    private bool declineEnded;

    private void Update()
    {
        if (TrainProgressManager.Instance.DestinationProgress > 0.25f && !firstInclineStarted)
        {
            firstInclineStarted = true;
            SetIncline(2f);
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.35f && !firstInclineEnded)
        {
            firstInclineEnded = true;
            SetIncline(0f);
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.55f && !secondInclineStarted)
        {
            secondInclineStarted = true;
            SetIncline(3.5f);
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.7f && !secondInclineEnded)
        {
            secondInclineEnded = true;
            SetIncline(0f);
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.8f && !declineStarted)
        {
            declineStarted = true;
            SetIncline(-4f);
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.9f && !declineEnded)
        {
            declineEnded = true;
            SetIncline(0f);
        }
    }

    private void SetIncline(float magnitude)
    {
        TrainProgressManager.Instance.VelocityDecayModifier = magnitude + 1f;
        Camera.main.transform.eulerAngles = new Vector3(0f, 0f, -magnitude);
        AudioController.Instance.PlaySound2D("blast");
    }

}
