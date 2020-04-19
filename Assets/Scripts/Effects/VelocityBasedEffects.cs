using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityBasedEffects : MonoBehaviour
{
    [SerializeField]
    private List<AutoRotate> wheels;

    [SerializeField]
    private Animator trainShakeAnim;

    [SerializeField]
    Animator bgAnim;

    [SerializeField]
    private RepeatingConveyor fgConveyor;

    [SerializeField]
    private AnimationCurve apparentVelocityCurve;

    [SerializeField]
    private AudioSource loop;

    private float baseWheelSpeed;

    private void Start()
    {
        baseWheelSpeed = wheels[0].rotateSpeed;    
    }

    private void Update()
    {
        float modifier = apparentVelocityCurve.Evaluate(TrainProgressManager.Instance.NormalizedVelocity);

        fgConveyor.MoveSpeedModifier = modifier;
        trainShakeAnim.speed = modifier * 2f;
        bgAnim.speed = modifier * 2f;
        wheels.ForEach(x => x.rotateSpeed = modifier * baseWheelSpeed);

        loop.pitch = 0.75f + (modifier * 0.5f);
    }
}
