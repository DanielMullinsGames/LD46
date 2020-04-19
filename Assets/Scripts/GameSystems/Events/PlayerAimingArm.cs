using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingArm : MonoBehaviour
{
    private void Start()
    {
        transform.LookAt(Vector3.up * 100f + (Vector3.right * 75f), Vector3.up);
    }

    void Update()
    {
        if (RaiderSpawner.Instance.CurrentRaider != null)
        {
            transform.LookAt(RaiderSpawner.Instance.CurrentRaider.transform.position, Vector3.up);
        }
    }
}
