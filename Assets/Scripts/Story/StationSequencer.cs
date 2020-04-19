using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationSequencer : MonoBehaviour
{
    [SerializeField]
    private SequentialText text;

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

    private void Start()
    {
        StartCoroutine(StationSequence());
    }

    private IEnumerator StationSequence()
    {
        yield return new WaitForSeconds(1f);

        foreach (string line in mainLines)
        {
            yield return PlayMessage(line);
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
