using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene3Sequencer : MonoBehaviour
{
    private bool firstBulletsEvent;
    private bool secondBulletsEvent;
    private bool declineEvent;
    private bool declineEventEnded;

    public List<SpriteRenderer> firstBulletCovers;
    public List<SpriteRenderer> secondBulletCovers;

    public ParticleSystem bloodParticles;

    private void Update()
    {
        if (TrainProgressManager.Instance.DestinationProgress > 0.25f && !firstBulletsEvent)
        {
            firstBulletsEvent = true;
            StartCoroutine(FirstBulletsEventSequence());
        }
        if (TrainProgressManager.Instance.DestinationProgress > 0.6f && !secondBulletsEvent)
        {
            secondBulletsEvent = true;
            StartCoroutine(SecondBulletsEventSequence());
        }

        if (TrainProgressManager.Instance.DestinationProgress > 0.75f && !declineEvent)
        {
            declineEvent = true;
            Scene2Sequencer.SetIncline(-2f);
        }

        if (TrainProgressManager.Instance.DestinationProgress > 0.95f && !declineEventEnded)
        {
            declineEventEnded = true;
            Scene2Sequencer.SetIncline(0f);
        }
    }

    private IEnumerator FirstBulletsEventSequence()
    {
        yield return ShootHoles(firstBulletCovers);
        yield return new WaitForSeconds(0.6f);
        RaiderSpawner.Instance.SpawnRaider();
    }

    private IEnumerator SecondBulletsEventSequence()
    {
        yield return ShootHoles(secondBulletCovers);

        if (HeartMachine.Instance.HasHeart)
        {
            AudioController.Instance.PlaySound2D("gunshot_2");
            yield return new WaitForSeconds(0.05f);
            HeartMachine.Instance.Vitality = 0f;
            HeartMachine.Instance.vitalityDecayRate *= 1.25f;
            bloodParticles.gameObject.SetActive(true);
            AudioController.Instance.PlaySound2D("crunch_blip");
        }

        yield return new WaitForSeconds(1f);
        RaiderSpawner.Instance.SpawnRaider();
    }

    private IEnumerator ShootHoles(List<SpriteRenderer> covers)
    {
        AudioController.Instance.PlaySound2D("hornblast");
        yield return new WaitForSeconds(0.5f);
        TrainProgressManager.Instance.Anim.Play("big_shake", 0, 0f);
        PlayerStateManager.Instance.enabled = false;
        yield return new WaitForSeconds(1f);
        PlayerStateManager.Instance.enabled = true;
        yield return new WaitForSeconds(1.5f);

        foreach (SpriteRenderer s in covers)
        {
            s.enabled = false;
            AudioController.Instance.PlaySound2D("gunshot_2");
            yield return new WaitForSeconds(0.25f);

            if (PlayerStateManager.Instance.CurrentState != PlayerState.Fallen)
            {
                PlayerStateManager.Instance.SwitchToState(PlayerState.Fallen, 2f);
            }
        }

    }
}
