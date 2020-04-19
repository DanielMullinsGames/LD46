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
    TyingShoes,
    AimingGun,
    NUM_STATES,
}

public class PlayerStateManager : Singleton<PlayerStateManager>
{
    public System.Action<PlayerState> StateChanged;
    public PlayerState CurrentState { get; private set; }

    public bool OnlyShoes { get; set; }

    public bool ShoesUntied { get; set; }
    private int standUpProgress = 0;
    private float lastStandupInput;
    private const float STANDUP_ATTEMPT_DURATION = 0.5f;
   
    [SerializeField]
    private List<PlayerStateObject> playerObjects;

    [SerializeField]
    private GameObject gunflare;

    [SerializeField]
    private float tripChance = 0.25f;

    [SerializeField]
    private int standupCount = 4;

    private bool switchingState;

    public Transform GetCurrentPlayerTransform()
    {
        return GetPlayerObject(CurrentState).transform;
    }

    public void SwitchToState(PlayerState state, float shakeMagnitude)
    {
        if (!switchingState)
        {
            if (state == PlayerState.Idle && CurrentState != PlayerState.Fallen && ShoesUntied && Random.value < tripChance)
            {
                standUpProgress = 0;
                lastStandupInput = Time.time;
                GetPlayerObject(PlayerState.Fallen).transform.localScale = new Vector2(shakeMagnitude > 0f ? -1f : 1f, 1f);
                StartCoroutine(SwitchStateSequence(PlayerState.Fallen, 2f));
            }
            else
            {
                if (state == PlayerState.Idle && Mathf.Abs(shakeMagnitude) > 0f)
                {
                    GetPlayerObject(PlayerState.Idle).transform.localScale = new Vector2(shakeMagnitude > 0f ? -1f : 1f, 1f);
                    GetPlayerObject(PlayerState.TyingShoes).transform.localScale = new Vector2(shakeMagnitude > 0f ? -1f : 1f, 1f);
                }
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
            case PlayerState.TyingShoes:
                if (Input.GetButtonDown("PlayerLeft"))
                {
                    SwitchToState(PlayerState.PumpUp, -1f);
                }
                if (Input.GetButtonDown("PlayerRight"))
                {
                    SwitchToState(PlayerState.ShovelTake, 1f);
                }
                break;
            case PlayerState.Idle:
                if (Input.GetButtonDown("PlayerLeft") && !OnlyShoes)
                {
                    SwitchToState(PlayerState.PumpUp, -1f);
                }
                if (Input.GetButtonDown("PlayerRight") && !OnlyShoes)
                {
                    SwitchToState(PlayerState.ShovelTake, 1f);
                }
                if (Input.GetButtonDown("PlayerUp") && !OnlyShoes)
                {
                    SwitchToState(PlayerState.AimingGun, 1f);
                }
                if (ShoesUntied && Input.GetButtonDown("PlayerDown"))
                {
                    SwitchToState(PlayerState.TyingShoes, 0f);
                }
                break;
            case PlayerState.PumpUp:
                if (Input.GetButtonDown("PlayerUp"))
                {
                    SwitchToState(PlayerState.AimingGun, 1f);
                }
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
                if (Input.GetButtonDown("PlayerUp") || Input.GetButtonDown("PlayerDown"))
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
                if (Input.GetButtonDown("PlayerUp"))
                {
                    SwitchToState(PlayerState.AimingGun, 1f);
                }
                if (ShoesUntied && Input.GetButtonDown("PlayerDown"))
                {
                    SwitchToState(PlayerState.TyingShoes, 0f);
                }
                break;
            case PlayerState.ShovelPlace:
                if (Input.GetButtonDown("PlayerLeft"))
                {
                    SwitchToState(PlayerState.ShovelTake, -1f);
                }
                break;
            case PlayerState.Fallen:
                if (Input.GetButtonDown("PlayerLeft") || Input.GetButtonDown("PlayerRight") || Input.GetButtonDown("PlayerUp"))
                {
                    if (Time.time - STANDUP_ATTEMPT_DURATION > lastStandupInput)
                    {
                        lastStandupInput = Time.time;
                        if (standUpProgress > standupCount)
                        {
                            SwitchToState(PlayerState.Idle, -GetPlayerObject(CurrentState).transform.localScale.x);
                        }
                        else
                        {
                            standUpProgress++;
                            AudioController.Instance.PlaySound2D("crunch_blip");
                            StartCoroutine(ShakePlayer(0.75f));
                        }
                    }
                }
                break;
            case PlayerState.AimingGun:
                if (RaiderSpawner.Instance.CurrentRaider != null && RaiderSpawner.Instance.CurrentRaider.CanBeShot && RunState.bullets > 0)
                {
                    if (Input.GetButtonDown("PlayerUp"))
                    {
                        ShootGun();
                    }
                }
                else
                {
                    if (Input.GetButtonDown("PlayerUp"))
                    {
                        AudioController.Instance.PlaySound2D("crunch_blip");
                    }
                    if (Input.GetButtonDown("PlayerLeft"))
                    {
                        SwitchToState(PlayerState.PumpUp, -1f);
                    }
                    if (Input.GetButtonDown("PlayerRight"))
                    {
                        SwitchToState(PlayerState.ShovelTake, 1f);
                    }
                    if (Input.GetButtonDown("PlayerDown"))
                    {
                        SwitchToState(PlayerState.Idle, 0.5f);
                    }
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

        yield return ShakePlayer(shakeMagnitude);
        switchingState = false;
    }

    private IEnumerator ShakePlayer(float shakeMagnitude)
    {
        var currentObject = GetPlayerObject(CurrentState);

        Tween.LocalPosition(currentObject.transform, currentObject.transform.localPosition + Vector3.right * 0.03f * shakeMagnitude, 0.025f, 0f, Tween.EaseOut);
        yield return new WaitForSeconds(CurrentState == PlayerState.Fallen ? 0.1f : 0.025f);
        Tween.LocalPosition(currentObject.transform, currentObject.transform.localPosition + Vector3.left * 0.03f * shakeMagnitude, 0.025f, 0f, Tween.EaseIn);
        yield return new WaitForSeconds(CurrentState == PlayerState.Fallen ? 0.1f : 0.025f);
    }

    private void PlaySoundForState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.ShovelTake:
                if (RunState.coal > 0)
                {
                    AudioController.Instance.PlaySound2D("crunch_short_2");
                }
                else
                {
                    AudioController.Instance.PlaySound2D("crunch_blip");
                }
                break;
            case PlayerState.ShovelPlace:
                if (RunState.coal > 0)
                {
                    AudioController.Instance.PlaySound2D("crunch_short_2", pitch: new AudioParams.Pitch(1.25f));
                }
                else
                {
                    AudioController.Instance.PlaySound2D("crunch_blip");
                }
                break;
            case PlayerState.AimingGun:
            case PlayerState.TyingShoes:
            case PlayerState.PumpUp:
                AudioController.Instance.PlaySound2D("crunch_blip");
                break;
            case PlayerState.Fallen:
                AudioController.Instance.PlaySound2D("crunch_short_1", pitch: new AudioParams.Pitch(0.5f));
                break;
        }
    }

    private void ShootGun()
    {
        gunflare.SetActive(true);
        CustomCoroutine.WaitThenExecute(0.05f, () => gunflare.SetActive(false));
        AudioController.Instance.PlaySound2D("gunshot_2");
        RaiderSpawner.Instance.OnRaiderShot();

        RunState.bullets--;
        BulletsDisplay.Instance.UpdateDisplay();
    }

    private PlayerStateObject GetPlayerObject(PlayerState state)
    {
        return playerObjects[Mathf.Clamp((int)state, 0, (int)PlayerState.NUM_STATES - 1)];
    }
}
