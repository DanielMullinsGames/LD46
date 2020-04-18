using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveForPlayerStates : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToActivate;

    [SerializeField]
    private bool activeFor;

    [SerializeField]
    private List<PlayerState> states;

    private void Update()
    {
        bool isState = states.Contains(PlayerStateManager.Instance.CurrentState);

        if (activeFor)
        {
            objectToActivate.SetActive(isState);
        }
        else
        {
            objectToActivate.SetActive(!isState);
        }
    }
}
