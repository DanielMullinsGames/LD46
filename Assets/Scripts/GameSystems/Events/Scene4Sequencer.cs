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
    private bool trainEventEnded;

    public GameObject trainAnim;

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
        if (TrainProgressManager.Instance.DestinationProgress > 0.525f && !trainEventStarted)
        {
            trainEventStarted = true;
            StartCoroutine(TrainEvent());
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.8f && !declineStarted)
        {
            declineStarted = true;
            Scene2Sequencer.SetIncline(-5f);
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.9f && !declineEnded)
        {
            declineEnded = true;
            Scene2Sequencer.SetIncline(0f);
        }
    }

    private IEnumerator TrainEvent()
    {
        trainAnim.gameObject.SetActive(true);
        AudioController.Instance.PlaySound2D("hornblast");
        yield return new WaitForSeconds(0.5f);
        TrainProgressManager.Instance.Anim.Play("big_shake", 0, 0f);
        PlayerStateManager.Instance.enabled = false;
        yield return new WaitForSeconds(1f);
        PlayerStateManager.Instance.enabled = true;
        yield return new WaitForSeconds(2f);

        while (TrainProgressManager.Instance.DestinationProgress < 0.8f)
        {
            if (RaiderSpawner.Instance.CurrentRaider == null)
            {
                yield return new WaitForSeconds(1f);
                if (RaiderSpawner.Instance.CurrentRaider == null)
                {
                    RaiderSpawner.Instance.SpawnRaider();
                }
            }
            yield return new WaitForEndOfFrame();
        }
        trainAnim.GetComponent<Animator>().SetTrigger("leave");
    }
}
