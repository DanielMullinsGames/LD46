using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : Singleton<GameFlowManager>
{
    public bool ArrivingAtDestination { get; private set; }

    [SerializeField]
    private GameObject fullScreenBlock;

    [SerializeField]
    private GameObject mostUIBlock;

    [SerializeField]
    private string nextSceneName;

    [SerializeField]
    private GameObject stationObj;

    public void OnPlayerKilled()
    {
        fullScreenBlock.SetActive(true);
        CustomCoroutine.WaitThenExecute(0f, () => SceneManager.LoadScene("GameOver"));
    }

    public void OnReachedDestination()
    {
        if (!ArrivingAtDestination)
        {
            ArrivingAtDestination = true;

            RaiderSpawner.Instance.OnReachedDestination();
            HeartMachine.Instance.Paused = true;
            HeartMachine.Instance.Vitality += 0.2f;

            StartCoroutine(EndSceneSequence());
        }
    }

    private IEnumerator EndSceneSequence()
    {
        mostUIBlock.SetActive(true);
        stationObj.gameObject.SetActive(true);
        yield return new WaitForSeconds(10f);

        //stop sound
        TrainProgressManager.Instance.GetComponent<AudioSource>().enabled = false;
        VelocityBasedEffects.Instance.Freeze();
        yield return new WaitForSeconds(2.5f);

        fullScreenBlock.SetActive(true);
        SceneManager.LoadScene(nextSceneName);
    }
}
