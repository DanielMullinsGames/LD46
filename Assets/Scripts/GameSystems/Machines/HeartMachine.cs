using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartMachine : Singleton<HeartMachine>
{
    public float Vitality { get; private set; }

    [SerializeField]
    private float vitalityDecayRate;

    private void Start()
    {
        Vitality = 1f;
        PlayerStateManager.Instance.StateChanged += OnStateChanged;
    }

    void Update()
    {
        Vitality = Mathf.Clamp(Vitality - (vitalityDecayRate * Time.deltaTime), 0f, 1f);

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.H))
        {
            Boost();
        }
#endif
    }

    public void Boost()
    {
        Vitality = Mathf.Clamp(Vitality + 0.1f, 0f, 1f);
    }

    private void OnStateChanged(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.PumpDown:
                TrainProgressManager.Instance.Anim.Play("pump", 0, 0f);
                AudioController.Instance.PlaySound2D("crunch_short_1", pitch: new AudioParams.Pitch(0.6f + (Vitality * 0.6f)));
                Boost();
                break;
        }
    }
}
