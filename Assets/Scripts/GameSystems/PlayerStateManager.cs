using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public enum PlayerState
{
    Idle,
    ShovelTake,
    ShovelPlace,
    PumpUp,
    PumpDown,
    NUM_STATES,
}

public class PlayerStateManager : Singleton<PlayerStateManager>
{
    public System.Action<PlayerState> StateChanged;
    public PlayerState CurrentState { get; private set; }
   
    [SerializeField]
    private List<PlayerStateObject> playerObjects;

    public void SwitchToState(PlayerState state, float shakeMagnitude)
    {
        StartCoroutine(SwitchStateSequence(state, shakeMagnitude));
    }

    private void Update()
    {
        PollInputForStateChange();
    }

    private void PollInputForStateChange()
    {
        switch (CurrentState)
        {
            case PlayerState.Idle:
                if (Input.GetButtonDown("PlayerLeft"))
                {
                    SwitchToState(PlayerState.PumpUp, -1f);
                }
                if (Input.GetButtonDown("PlayerRight"))
                {
                    SwitchToState(PlayerState.ShovelTake, 1f);
                }
                break;
            case PlayerState.PumpUp:
                if (Input.GetButtonDown("PlayerDown"))
                {
                    SwitchToState(PlayerState.PumpDown, 0f);
                }
                if (Input.GetButtonDown("PlayerRight"))
                {
                    SwitchToState(PlayerState.Idle, 1f);
                }
                break;
            case PlayerState.PumpDown:
                if (Input.GetButtonDown("PlayerUp"))
                {
                    SwitchToState(PlayerState.PumpUp, 0f);
                }
                if (Input.GetButtonDown("PlayerRight"))
                {
                    SwitchToState(PlayerState.Idle, 1f);
                }
                break;
            case PlayerState.ShovelTake:
                if (Input.GetButtonDown("PlayerLeft"))
                {
                    SwitchToState(PlayerState.Idle, -1f);
                }
                if (Input.GetButtonDown("PlayerRight"))
                {
                    SwitchToState(PlayerState.ShovelPlace, 1f);
                }
                break;
            case PlayerState.ShovelPlace:
                if (Input.GetButtonDown("PlayerLeft"))
                {
                    SwitchToState(PlayerState.ShovelTake, -1f);
                }
                break;
        }
    }

    private IEnumerator SwitchStateSequence(PlayerState state, float shakeMagnitude)
    {
        var currentObject = GetPlayerObject(CurrentState);
        Tween.LocalPosition(currentObject.transform, currentObject.transform.localPosition + Vector3.left * 0.04f * shakeMagnitude, 0.03f, 0f);
        yield return new WaitForSeconds(0.03f);
        currentObject.gameObject.SetActive(false);

        CurrentState = state;
        StateChanged?.Invoke(state);
        currentObject = GetPlayerObject(CurrentState);
        currentObject.gameObject.SetActive(true);
        currentObject.transform.localPosition = currentObject.OriginalPosition;

        Tween.LocalPosition(currentObject.transform, currentObject.transform.localPosition + Vector3.right * 0.03f * shakeMagnitude, 0.025f, 0f, Tween.EaseOut);
        yield return new WaitForSeconds(0.025f);
        Tween.LocalPosition(currentObject.transform, currentObject.transform.localPosition + Vector3.left * 0.03f * shakeMagnitude, 0.025f, 0f, Tween.EaseIn);
        yield return new WaitForSeconds(0.025f);
    }

    private PlayerStateObject GetPlayerObject(PlayerState state)
    {
        return playerObjects[Mathf.Clamp((int)CurrentState, 0, (int)PlayerState.NUM_STATES - 1)];
    }
}
