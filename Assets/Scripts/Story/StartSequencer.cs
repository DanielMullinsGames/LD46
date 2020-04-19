using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSequencer : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(BeepingSlow());
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad < 1f)
        {
            return;
        }

        if (Input.GetButton("PlayerLeft") || Input.GetButton("PlayerRight") || Input.GetButton("PlayerUp") || Input.GetButton("PlayerDown"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene1");
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
}
