using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public enum PlayerState
{
    Idle,
    ShovelTake,
    ShovelPlace,
    NUM_STATES,
}

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField]
    private List<PlayerStateObject> playerObjects;

    private PlayerState currentState = PlayerState.Idle;

    public void SwitchToState(PlayerState state, float shakeMagnitude)
    {
        StartCoroutine(SwitchStateSequence(state, shakeMagnitude));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchToState(currentState + 1, 1f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchToState(currentState - 1, -1f);
        }
    }

    private IEnumerator SwitchStateSequence(PlayerState state, float shakeMagnitude)
    {
        var currentObject = GetPlayerObject(currentState);
        Tween.LocalPosition(currentObject.transform, currentObject.transform.localPosition + Vector3.left * 0.04f * shakeMagnitude, 0.03f, 0f);
        yield return new WaitForSeconds(0.03f);
        currentObject.gameObject.SetActive(false);

        currentState = state;
        currentObject = GetPlayerObject(currentState);
        currentObject.gameObject.SetActive(true);
        currentObject.transform.localPosition = currentObject.OriginalPosition;

        Tween.LocalPosition(currentObject.transform, currentObject.transform.localPosition + Vector3.right * 0.03f * shakeMagnitude, 0.025f, 0f, Tween.EaseOut);
        yield return new WaitForSeconds(0.025f);
        Tween.LocalPosition(currentObject.transform, currentObject.transform.localPosition + Vector3.left * 0.03f * shakeMagnitude, 0.025f, 0f, Tween.EaseIn);
        yield return new WaitForSeconds(0.025f);
    }

    private PlayerStateObject GetPlayerObject(PlayerState state)
    {
        return playerObjects[Mathf.Clamp((int)currentState, 0, (int)PlayerState.NUM_STATES - 1)];
    }
}
