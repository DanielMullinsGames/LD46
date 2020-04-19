using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoelacesStation : Singleton<ShoelacesStation>
{
    public float UntieChance { get; private set; }

    public float TieProgress { get; private set; }

    [SerializeField]
    private float tieSpeed = 1f;

    [SerializeField]
    private GameObject untieAnimationObject;

    private void Start()
    {
        PlayerStateManager.Instance.StateChanged += OnStateChanged;
    }

    public void Untie()
    {
        UntieChance = 0f;
        PlayerStateManager.Instance.ShoesUntied = true;
        var untieAnim = Instantiate(untieAnimationObject);
        untieAnim.SetActive(true);
        untieAnim.transform.position = untieAnimationObject.transform.position;
        Destroy(untieAnim, 1f);
        AudioController.Instance.PlaySound2D("shoes_untied");
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
                        Untie();
                    }

                    UntieChance += 0.1f;
                }
                break;
            case PlayerState.Fallen:
                TrainProgressManager.Instance.Anim.Play("boost", 0, 0f);
                break;
            case PlayerState.TyingShoes:
                TieProgress = 0f;
                PlayerStateManager.learnedShoeTying = true;
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
                UntieChance = 0f;
                PlayerStateManager.Instance.ShoesUntied = false;
                PlayerStateManager.Instance.SwitchToState(PlayerState.Idle, 0f);
                AudioController.Instance.PlaySound2D("success");
            }
        }
    }
}
