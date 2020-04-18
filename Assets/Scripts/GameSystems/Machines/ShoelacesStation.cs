using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoelacesStation : Singleton<ShoelacesStation>
{
    public float UntieChance { get; private set; }

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
        }
    }
}
