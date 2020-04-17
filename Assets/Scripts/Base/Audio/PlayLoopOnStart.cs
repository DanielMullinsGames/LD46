using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLoopOnStart : MonoBehaviour
{
    [SerializeField]
    private string loopId = default;

    private void Start()
    {
        AudioController.Instance.SetLoopAndPlay(loopId);
    }
}
