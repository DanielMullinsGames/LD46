using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoelacesStation : Singleton<ShoelacesStation>
{
    public float UntieChance { get; private set; }

    public float TieProgress { get; private set; }

    [SerializeField]
    private float tieSpeed = 1f;

    private void Start()
    {
        PlayerStateManager.Instance.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                if (!PlayerStateManager.Instance.ShoesUntied)
                {
                    if (Random.value < UntieChance)
                    {
                        UntieChance = 0f;
                        PlayerStateManager.Instance.ShoesUntied = true;
                    }

                    UntieChance += 0.1f;
                }
                break;
            case PlayerState.Fallen:
                TrainProgressManager.Instance.Anim.Play("boost", 0, 0f);
                break;
            case PlayerState.TyingShoes:
                TieProgress = 0f;
                break;
        }
    }

    private void Update()
    {
        if (PlayerStateManager.Instance.CurrentState == PlayerState.TyingShoes)
        {
            TieProgress += Time.deltaTime * tieSpeed;
            if (TieProgress > 1f)
            {
                PlayerStateManager.Instance.ShoesUntied = false;
                PlayerStateManager.Instance.SwitchToState(PlayerState.Idle, 0f);
            }
        }
    }
}
