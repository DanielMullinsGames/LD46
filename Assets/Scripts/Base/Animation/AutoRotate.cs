using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoRotate : MonoBehaviour {

    public float rotateSpeed;
    public bool rotateClockwise;
    public float slowSpeed;

    public bool unscaledTime;

    [SerializeField]
    private bool local;

    [Header("Incremental")]
    public bool byIncrement;
    public float increment;

    [Header("Rotate Between")]
    public bool rotateBetween;
    public float minRotation;
    public float maxRotation;

    private float tickTimer;

    void Update()
    {
        if (byIncrement)
        {
            tickTimer += unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (tickTimer > rotateSpeed)
            {
                SetZRotation(transform.rotation.eulerAngles.z + increment * (rotateClockwise ? 1f : -1f));
                tickTimer = 0f;
            }
        }
        else
        {
            rotateSpeed = Mathf.Max(rotateSpeed - (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * slowSpeed, 0f);

            if (rotateSpeed != 0f)
            {
                SetZRotation(transform.rotation.eulerAngles.z + Time.deltaTime * rotateSpeed * (rotateClockwise ? 1f : -1f));
            }

            if (rotateBetween)
            {
                if (transform.rotation.eulerAngles.z > maxRotation)
                {
                    rotateClockwise = false;
                }
                else if (transform.rotation.eulerAngles.z < minRotation)
                {
                    rotateClockwise = true;
                }
            }
        }
    }

    private void SetZRotation(float zrot)
    {
        if (local)
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, zrot);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, zrot);
        }
    }
}
