using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartMachine : Singleton<HeartMachine>
{
    public bool Paused { get; set; }
    public bool AudioPaused { get; set; }
    public float Vitality { get;  set; }

    public bool Dead { get; private set; }

    public bool HasHeart { get; set; }

    public float vitalityDecayRate;

    [SerializeField]
    private float deathTime = 5f;

    [SerializeField]
    private AudioSource flatlineSource;

    [SerializeField]
    private AnimatingSprite pumpingAnim;

    [SerializeField]
    private Sprite deadHeart;

    private float beepTimer;

    private float deathTimer;

    private void Start()
    {
        HasHeart = true;
        Vitality = 1f;
        PlayerStateManager.Instance.StateChanged += OnStateChanged;
    }

    void Update()
    {
        if (!Paused)
        {
            Vitality = Mathf.Clamp(Vitality - (vitalityDecayRate * Time.deltaTime), 0f, 1f);
        }

        if (Vitality <= 0f && !Paused)
        {
            flatlineSource.enabled = true;

            deathTimer += Time.deltaTime;
            if (deathTimer > deathTime)
            {
                Die();
            }
        }
        else
        {
            deathTimer = 0f;
            flatlineSource.enabled = false;

            if (!AudioPaused)
            {
                beepTimer += Time.deltaTime;
                if (beepTimer > Mathf.Max(0.1f, Vitality * 2f))
                {
                    beepTimer = 0f;
                    AudioController.Instance.PlaySound2D("short_beep", volume: 0.1f);
                }
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.H))
        {
            Boost();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vitality = 0f;
        }
#endif
    }

    public void TakeHeart()
    {
        pumpingAnim.gameObject.SetActive(false);
        HasHeart = false;
    }

    public void Boost()
    {
        if (!Dead)
        {
            Vitality = Mathf.Clamp(Vitality + 0.1f, 0f, 1f);
        }
    }

    private void Die()
    {
        AudioController.Instance.PlaySound2D("crunch_blip");
        flatlineSource.enabled = false;
        enabled = false;

        pumpingAnim.enabled = false;
        pumpingAnim.GetComponent<SpriteRenderer>().sprite = deadHeart;
        RunState.lostHeart = true;

        Dead = true;
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
