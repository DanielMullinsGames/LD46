using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceMachine : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer shovelFuel;

    [SerializeField]
    private GameObject furnaceFuelObj;

    [SerializeField]
    private List<Sprite> pileSprites;

    [SerializeField]
    private SpriteRenderer pileRenderer;

    private void Start()
    {
        PlayerStateManager.Instance.StateChanged += OnStateChanged;
        UpdatePile();
    }

    private void OnStateChanged(PlayerState state)
    {
        switch(state)
        {
            case PlayerState.ShovelPlace:
                if (RunState.coal > 0)
                {
                    var fuel = Instantiate(furnaceFuelObj);
                    fuel.transform.position = furnaceFuelObj.transform.position;
                    fuel.SetActive(true);
                    TrainProgressManager.Instance.AddFuel();
                    RunState.coal--;
                    CoalDisplay.Instance.UpdateDisplay();
                    UpdatePile();
                }
                break;
            case PlayerState.ShovelTake:
                shovelFuel.enabled = RunState.coal > 0;
                break;
        }
    }

    private void UpdatePile()
    {
        if (RunState.coal > 10)
        {
            pileRenderer.sprite = pileSprites[0];
        }
        else if (RunState.coal > 5)
        {
            pileRenderer.sprite = pileSprites[1];
        }
        else if (RunState.coal > 0)
        {
            pileRenderer.sprite = pileSprites[2];
        }
        else
        {
            pileRenderer.sprite = pileSprites[3];
        }
    }
}
