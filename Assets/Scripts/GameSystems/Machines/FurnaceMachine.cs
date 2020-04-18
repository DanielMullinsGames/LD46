using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceMachine : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer shovelFuel;

    [SerializeField]
    private GameObject furnaceFuelObj;

    private void Start()
    {
        PlayerStateManager.Instance.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(PlayerState state)
    {
        switch(state)
        {
            case PlayerState.ShovelPlace:
                var fuel = Instantiate(furnaceFuelObj);
                fuel.transform.position = furnaceFuelObj.transform.position;
                fuel.SetActive(true);
                TrainProgressManager.Instance.AddFuel();
                break;
        }
    }
}
