using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSequencer : MonoBehaviour
{
    [SerializeField]
    private SequentialText text;

    [SerializeField]
    private GameObject choices;

    private void Start()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(4f);
        text.PlayMessage("end of the line...    start over from seattle?");
        yield return new WaitUntil(() => !text.PlayingMessage);
        yield return new WaitForSeconds(0.2f);

        choices.SetActive(true);
        AudioController.Instance.PlaySound2D("crunch_blip");

        yield return new WaitUntil(() => Input.GetButton("PlayerUp") || Input.GetButton("PlayerDown"));

        if (Input.GetButton("PlayerUp"))
        {
            text.gameObject.SetActive(false);
            choices.SetActive(false);
            AudioController.Instance.PlaySound2D("crunch_blip");
            RunState.Reset();
            yield return new WaitForSeconds(0.5f);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene2");
        }
        else
        {
            text.gameObject.SetActive(false);
            choices.SetActive(false);
            AudioController.Instance.PlaySound2D("crunch_blip");
            yield return new WaitForSeconds(0.5f);
            Application.Quit();
        }
    }
}
