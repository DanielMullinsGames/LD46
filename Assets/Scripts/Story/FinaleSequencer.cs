using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinaleSequencer : MonoBehaviour
{
    public GameObject bgBlock;
    public GameObject allBlock;
    public GameObject controlsHint;

    public SequentialText text;

    public GameObject playerFrame1;
    public GameObject playerFrame2;
    public List<GameObject> heartGood;
    public List<GameObject> heartBad;
    public GameObject flatline;

    private void Start()
    {
        StartCoroutine(Sequence());
    }

    public IEnumerator Sequence()
    {
        heartGood.ForEach(x => x.SetActive(!RunState.lostHeart));
        heartBad.ForEach(x => x.SetActive(RunState.lostHeart));

        StartCoroutine("Beeping");

        bgBlock.SetActive(true);
        allBlock.SetActive(true);
        yield return new WaitForSeconds(1f);

        yield return PlayMessage("right this way! hurry!");
        yield return PlayMessage("you have the cargo right?");

        yield return new WaitForSeconds(0.5f);
        allBlock.SetActive(false);
        AudioController.Instance.PlaySound2D("crunch_blip");
        yield return new WaitForSeconds(0.5f);

        yield return PlayMessage("hurry!");
        yield return PlayMessage("here! give it here!", clear: false);

        yield return new WaitForSeconds(0.5f);
        bgBlock.SetActive(false);
        AudioController.Instance.PlaySound2D("crunch_blip");
        yield return new WaitForSeconds(1.5f);

        controlsHint.SetActive(true);
        AudioController.Instance.PlaySound2D("crunch_blip");

        float timer = 0f;
        int messageIndex = 0;
        while (!Input.GetButton("PlayerRight"))
        {
            timer += Time.deltaTime;

            if (timer > 4f && messageIndex == 0)
            {
                messageIndex++;
                StartCoroutine(PlayMessage("what are you doing?"));
            }
            else if (timer > 10f && messageIndex == 1)
            {
                messageIndex++;
                StartCoroutine(PlayMessage("g- give it here!"));
            }
            else if (timer > 20f && messageIndex == 2 && !RunState.lostHeart)
            {
                messageIndex++;
                AudioController.Instance.PlaySound2D("crunch_blip");
                heartGood.ForEach(x => x.SetActive(false));
                heartBad.ForEach(x => x.SetActive(true));
                RunState.lostHeart = true;
            }

            yield return new WaitForEndOfFrame();
        }

        AudioController.Instance.PlaySound2D("crunch_blip");
        text.Clear();
        controlsHint.SetActive(false);
        playerFrame1.SetActive(false);
        playerFrame2.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        AudioController.Instance.PlaySound2D("crunch_short_1");
        allBlock.SetActive(true);

        if (RunState.lostHeart)
        {
            yield return PlayMessage("w-what");
            yield return PlayMessage("this won't work");
            yield return PlayMessage("this is nothing!");
            flatline.SetActive(true);
            StopCoroutine("Beeping");
            yield return PlayMessage("...");
            yield return PlayMessage("goodbye... my sweet queen...");
            yield return PlayMessage("the one hope this tattered planet had left");
            yield return PlayMessage("...");
            yield return PlayMessage("THANK YOU FOR PLAYING", clear: false);
            yield return new WaitForSeconds(3f);
        }
        else
        {
            yield return PlayMessage("oh llord");
            StopCoroutine("Beeping");
            StartCoroutine("BeepingSlow");
            yield return PlayMessage("i think this will work");
            yield return PlayMessage("she will take some time to recover");
            yield return PlayMessage("but her soul is strong");
            yield return PlayMessage("she will endure");
            yield return PlayMessage("...");
            yield return PlayMessage("WE will endure");
            yield return PlayMessage("...");
            yield return PlayMessage("THANK YOU FOR PLAYING", clear: false);
        }
    }

    private IEnumerator Beeping()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            AudioController.Instance.PlaySound2D("short_beep", volume: 0.1f);
        }
    }

    private IEnumerator BeepingSlow()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            AudioController.Instance.PlaySound2D("short_beep", volume: 0.1f);
        }
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
