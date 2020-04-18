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
    Fallen,
    NUM_STATES,
}

public class PlayerStateManager : Singleton<PlayerStateManager>
{
    public System.Action<PlayerState> StateChanged;
    public PlayerState CurrentState { get; private set; }

    public bool ShoesUntied { get; set; }
   
    [SerializeField]
    private List<PlayerStateObject> playerObjects;

    [SerializeField]
    private float tripChance = 0.25f;

    private bool switchingState;

    private void SwitchToState(PlayerState state, float shakeMagnitude)
    {
        if (!switchingState)
        {
            if (state == PlayerState.Idle && ShoesUntied && Random.value < tripChance)
            {
                GetPlayerObject(PlayerState.Fallen).transform.localScale = new Vector2(shakeMagnitude > 0f ? -1f : 1f, 1f);
                StartCoroutine(SwitchStateSequence(PlayerState.Fallen, 4f));
            }
            else
            {
                StartCoroutine(SwitchStateSequence(state, shakeMagnitude));
            }
        }
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
                    AudioController.Instance.PlaySound2D("crunch_blip");
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
        switchingState = true;

        var currentObject = GetPlayerObject(CurrentState);
        Tween.LocalPosition(currentObject.transform, currentObject.transform.localPosition + Vector3.left * 0.04f * shakeMagnitude, 0.03f, 0f);
        yield return new WaitForSeconds(state == PlayerState.Fallen ? 0.2f : 0.03f);
        currentObject.gameObject.SetActive(false);

        CurrentState = state;
        StateChanged?.Invoke(state);
        currentObject = GetPlayerObject(CurrentState);
        currentObject.gameObject.SetActive(true);
        currentObject.transform.localPosition = currentObject.OriginalPosition;
        PlaySoundForState(state);

        Tween.LocalPosition(currentObject.transform, currentObject.transform.localPosition + Vector3.right * 0.03f * shakeMagnitude, 0.025f, 0f, Tween.EaseOut);
        yield return new WaitForSeconds(state == PlayerState.Fallen ? 0.1f : 0.025f);
        Tween.LocalPosition(currentObject.transform, currentObject.transform.localPosition + Vector3.left * 0.03f * shakeMagnitude, 0.025f, 0f, Tween.EaseIn);
        yield return new WaitForSeconds(state == PlayerState.Fallen ? 0.1f : 0.025f);

        switchingState = false;
    }

    private void PlaySoundForState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.ShovelTake:
                AudioController.Instance.PlaySound2D("crunch_short_2");
                break;
            case PlayerState.ShovelPlace:
                AudioController.Instance.PlaySound2D("crunch_short_2", pitch: new AudioParams.Pitch(1.25f));
                break;
        }
    }

    private PlayerStateObject GetPlayerObject(PlayerState state)
    {
        return playerObjects[Mathf.Clamp((int)state, 0, (int)PlayerState.NUM_STATES - 1)];
    }
}
