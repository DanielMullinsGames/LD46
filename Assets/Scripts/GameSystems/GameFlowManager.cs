using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : Singleton<GameFlowManager>
{
    [SerializeField]
    private GameObject fullScreenBlock;

    public void OnPlayerKilled()
    {
        fullScreenBlock.SetActive(true);
        CustomCoroutine.WaitThenExecute(0f, () => SceneManager.LoadScene("GameOver"));
    }
}
