using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationSequencer : MonoBehaviour
{
    [SerializeField]
    private SequentialText text;

    [SerializeField]
    private string nextScene;

    [SerializeField]
    private GameObject ui;

    [Header("Resources")]
    [SerializeField]
    private int baseCoalGain;

    [SerializeField]
    private bool replenishCoal;

    [SerializeField]
    private int baseBulletsGain;

    [Header("Dialogue")]
    [TextArea]
    [SerializeField]
    private List<string> deadHeartLines;

    [TextArea]
    [SerializeField]
    private List<string> noCoalLines;

    [TextArea]
    [SerializeField]
    private List<string> noBulletsLines;

    [TextArea]
    [SerializeField]
    private List<string> mainLines;

    [TextArea]
    [SerializeField]
    private List<string> partingLines;


    public bool gunshotEvent;
    public SequentialText redText;

    public bool noGunshotNewHeart;


    private void Start()
    {
        StartCoroutine(StationSequence());
    }

    private IEnumerator StationSequence()
    {
        AudioController.Instance.SetLoopAndPlay("rest_stop");
        //AudioController.Instance.SetLoopVolume(0.5f, 0f);

        ui.SetActive(false);
        yield return new WaitForSeconds(1f);
        ui.SetActive(true);

        if (RunState.coal < 5 && noCoalLines != null && noCoalLines.Count > 0)
        {
            foreach (string line in noCoalLines)
            {
                yield return PlayMessage(line);
            }
            yield return new WaitForSeconds(0.2f);
            AddCoal(10);
            yield return new WaitForSeconds(0.5f);
        }

        if (RunState.bullets < 1 && noBulletsLines != null && noBulletsLines.Count > 0)
        {
            foreach (string line in noBulletsLines)
            {
                yield return PlayMessage(line);
            }
            yield return new WaitForSeconds(0.2f);
            AddBullets(1);
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);

        if (RunState.lostHeart && deadHeartLines !=null && deadHeartLines.Count > 0)
        {
            foreach (string line in deadHeartLines)
            {
                yield return PlayMessage(line);
            }
            yield return new WaitForSeconds(0.1f);
            if (!noGunshotNewHeart)
            {
                AudioController.Instance.PlaySound2D("gunshot_2");
            }
            yield return new WaitForSeconds(1f);
            RunState.lostHeart = false;
            RunState.harvestedHeart = true;
        }
        else
        {
            foreach (string line in mainLines)
            {
                yield return PlayMessage(line);
            }
            yield return new WaitForSeconds(0.1f);
        }

        #region GunshotEvent
        if (gunshotEvent)
        {
            yield return PlayMessage("i said... get outta the-", endAbrupt: true);
            AudioController.Instance.PlaySound2D("gunshot_2");
            yield return new WaitForSeconds(1f);
            redText.PlayMessage("take these bullets... and RUN!");
            yield return new WaitUntil(() => !redText.PlayingMessage);
        }
        #endregion

        int coalGain = baseCoalGain;
        if (replenishCoal)
        {
            coalGain += RunState.STARTING_COAL - RunState.coal;
        }
        AddCoal(coalGain);
        yield return new WaitForSeconds(0.1f);

        if (baseBulletsGain > 0)
        {
            AddBullets(baseBulletsGain);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        foreach (string line in partingLines)
        {
            yield return PlayMessage(line);
        }
        ui.SetActive(false);

        yield return new WaitForSeconds(0.2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }

    private void AddCoal(int amount)
    {
        RunState.coal += amount;
        CoalDisplay.Instance.UpdateDisplay();
        AudioController.Instance.PlaySound2D("crunch_blip");
    }

    private void AddBullets(int amount)
    {
        RunState.bullets = Mathf.Min(RunState.bullets + amount, 3);
        BulletsDisplay.Instance.UpdateDisplay();
        AudioController.Instance.PlaySound2D("crunch_blip");
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
