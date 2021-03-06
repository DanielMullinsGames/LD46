﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainProgressManager : Singleton<TrainProgressManager>
{
    public bool Paused { get; set; }
    public float Velocity { get; private set; }
    public float NormalizedVelocity { get { return (Velocity - MIN_VELOCITY) / (MAX_VELOCITY - MIN_VELOCITY); } }
    public float VelocityDecayModifier { get; set; }

    public float DestinationProgress { get; private set; }

    [SerializeField]
    private float baseSpeedModifier = 1f;

    public Animator Anim { get { return trainAnim; } }
    [SerializeField]
    private Animator trainAnim;

    [SerializeField]
    private float baseVelocityDecay = 3f;

    private const float MIN_VELOCITY = 25f;
    private const float MAX_VELOCITY = 125f;

    private void Start()
    {
        VelocityDecayModifier = 1f;
        Velocity = 75f;
    }

    private void Update()
    {
        if (!Paused)
        {
            if (DestinationProgress >= 1f)
            {
                Velocity = Mathf.Max(Velocity - (Time.deltaTime * 10f), 0f);
                GameFlowManager.Instance.OnReachedDestination();
            }
            else
            {
                float velocityDecay = baseVelocityDecay * VelocityDecayModifier;
                if (Velocity > 100f)
                {
                    velocityDecay *= 3f;
                }

                Velocity = Mathf.Clamp(Velocity - (Time.deltaTime * velocityDecay), 0f, MAX_VELOCITY);
                DestinationProgress += Time.deltaTime * Mathf.Max(0.1f, Velocity) * 0.0001f * baseSpeedModifier;
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddFuel();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            SlowTrain(15f);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            DestinationProgress += 0.2f;
        }
#endif
    }

    public void AddFuel(float value = 25f)
    {
        Velocity += value;
        Velocity = Mathf.Min(Velocity, MAX_VELOCITY);

        trainAnim.Play("boost", 0, 0f);
    }

    public void SlowTrain(float amount)
    {
        Velocity -= amount;
    }
}
