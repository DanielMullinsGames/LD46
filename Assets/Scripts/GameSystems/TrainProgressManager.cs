using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainProgressManager : Singleton<TrainProgressManager>
{
    public float Velocity { get; private set; }
    public float NormalizedVelocity { get { return (Velocity - MIN_VELOCITY) / (MAX_VELOCITY - MIN_VELOCITY); } }

    public float DestinationProgress { get; private set; }

    [SerializeField]
    private float baseSpeedModifier = 1f;

    private const float MIN_VELOCITY = 25f;
    private const float MAX_VELOCITY = 125f;

    private void Start()
    {
        Velocity = 75f;
    }

    private void Update()
    {
        Velocity -= Time.deltaTime * 0.5f; //TODO: based on properties of fuel.
        DestinationProgress += Time.deltaTime * Velocity * 0.0001f * baseSpeedModifier;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddCoal();
        }
#endif
    }

    public void AddCoal()
    {
        Velocity += 10f;

        Velocity = Mathf.Min(Velocity, MAX_VELOCITY);
    }
}
