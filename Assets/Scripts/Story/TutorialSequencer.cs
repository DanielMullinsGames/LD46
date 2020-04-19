using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSequencer : MonoBehaviour
{
    [SerializeField]
    private SequentialText text;

    [SerializeField]
    private GameObject heartMachineBlocker;

    [SerializeField]
    private GameObject furnaceBlocker;

    [SerializeField]
    private GameObject speedUIBlocker;

    [SerializeField]
    private GameObject ammoUIBlocker;

    [SerializeField]
    private GameObject coalUIBlocker;

    [SerializeField]
    private List<GameObject> finalBlockers;

    [SerializeField]
    private Animator trainAnim;

    [SerializeField]
    private GameObject shoelacePrompt;

    [SerializeField]
    private List<GameObject> controlsPrompts = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(Tutorial());
        PlayerStateManager.Instance.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.PumpUp:
                controlsPrompts[0].SetActive(false);
                break;
            case PlayerState.AimingGun:
                controlsPrompts[1].SetActive(false);
                break;
            case PlayerState.ShovelTake:
                controlsPrompts[2].SetActive(false);
                break;
            case PlayerState.ShovelPlace:
                controlsPrompts[3].SetActive(false);
                break;
            case PlayerState.PumpDown:
                controlsPrompts[4].SetActive(false);
                break;
        }
    }

    private IEnumerator Tutorial()
    {
        trainAnim.enabled = false;
        TrainProgressManager.Instance.Paused = true;
        HeartMachine.Instance.Paused = true;
        PlayerStateManager.Instance.enabled = false;

        yield return PlayMessage("you didn't forget. did you now. old timer?");
        yield return PlayMessage("this is the most important haul in your whole long-ass life");
        yield return new WaitForSeconds(0.25f);

        ShoelacesStation.Instance.Untie();
        PlayerStateManager.Instance.GetCurrentPlayerTransform().Find("laces").GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.5f);

        yield return PlayMessage("oh for the love of-", endAbrupt: true);
        yield return PlayMessage("tie your damn shoes man", clear:false);

        PlayerStateManager.Instance.enabled = true;
        PlayerStateManager.Instance.OnlyShoes = true;

        yield return new WaitForSeconds(0.2f);
        shoelacePrompt.SetActive(true);
        AudioController.Instance.PlaySound2D("crunch_blip");

        yield return new WaitUntil(() => !PlayerStateManager.Instance.ShoesUntied);
        shoelacePrompt.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        yield return PlayMessage("christ... anyways...");
        yield return new WaitForSeconds(0.3f);

        heartMachineBlocker.SetActive(false);
        AudioController.Instance.PlaySound2D("crunch_blip");
        yield return new WaitForSeconds(1f);

        yield return PlayMessage("you gotta keep that beating heart alive all the way from vancouver to tijuana");
        yield return PlayMessage("but that ain't it");
        yield return new WaitForSeconds(0.3f);

        furnaceBlocker.SetActive(false);
        speedUIBlocker.SetActive(false);
        AudioController.Instance.PlaySound2D("crunch_blip");
        yield return new WaitForSeconds(1.5f);

        yield return PlayMessage("i know you're used to running slow. but that ain't gonna fly");
        yield return PlayMessage("you run too slow and those RAILWORKERS are gonna latch right onto you");
        yield return PlayMessage("they don't know your freight ain't worth shit to 'em. save for the clothes off your back and a bit of protein");
        yield return PlayMessage("hey and you probably know this too. but don't run too fast neither");

        yield return new WaitForSeconds(0.3f);
        coalUIBlocker.SetActive(false);
        AudioController.Instance.PlaySound2D("crunch_blip");
        yield return new WaitForSeconds(1f);

        yield return PlayMessage("run too fast and you'll waste coal. you run out... well i did mention the RAILWORKERS...");

        yield return new WaitForSeconds(0.3f);
        ammoUIBlocker.SetActive(false);
        AudioController.Instance.PlaySound2D("crunch_blip");
        yield return new WaitForSeconds(1f);

        yield return PlayMessage("your first stop is seattle. once you get not far out 'a vancouver's cell tower i'm gonna cut ou-", endAbrupt: true);
        AudioController.Instance.PlaySound2D("crunch_blip");
        finalBlockers.ForEach(x => x.SetActive(false));

        trainAnim.enabled = true;
        TrainProgressManager.Instance.Paused = false;
        HeartMachine.Instance.Paused = false;

        yield return new WaitForSeconds(1f);
        controlsPrompts.ForEach(x => x.SetActive(true));
        AudioController.Instance.PlaySound2D("crunch_blip");

        PlayerStateManager.Instance.enabled = true;
        PlayerStateManager.Instance.OnlyShoes = false;
    }

    private IEnumerator PlayMessage(string message, bool clear = true, bool endAbrupt = false)
    {
        text.PlayMessage(message);
        yield return new WaitUntil(() => !text.PlayingMessage);
        if (!endAbrupt)
        {
            yield return new WaitForSeconds(0.6f);
        }
        if (clear)
        {
            text.Clear();
        }
        yield return new WaitForSeconds(0.1f);
    }
}
