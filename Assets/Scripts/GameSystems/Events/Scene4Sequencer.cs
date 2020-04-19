using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene4Sequencer : MonoBehaviour
{
    private bool firstInclineStarted;
    private bool firstInclineEnded;
    private bool secondInclineStarted;
    private bool secondInclineEnded;
    private bool declineStarted;
    private bool declineEnded;

    private bool trainEventStarted;

    private void Update()
    {
        if (TrainProgressManager.Instance.DestinationProgress > 0.1f && !firstInclineStarted)
        {
            firstInclineStarted = true;
            Scene2Sequencer.SetIncline(2f);
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.15f && !firstInclineEnded)
        {
            firstInclineEnded = true;
            Scene2Sequencer.SetIncline(0f);
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.25f && !secondInclineStarted)
        {
            secondInclineStarted = true;
            Scene2Sequencer.SetIncline(3f);
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.3f && !secondInclineEnded)
        {
            secondInclineEnded = true;
            Scene2Sequencer.SetIncline(0f);
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.5f && !trainEventStarted)
        {
            trainEventStarted = true;
            StartCoroutine(TrainEvent());
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.8f && !declineStarted)
        {
            declineStarted = true;
            Scene2Sequencer.SetIncline(-5f);
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.85f && !declineEnded)
        {
            declineEnded = true;
            Scene2Sequencer.SetIncline(0f);
        }
    }

    private IEnumerator TrainEvent()
    {
        yield break;
    }
}
