using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderAimingArm : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(PlayerStateManager.Instance.GetCurrentPlayerTransform(), Vector3.up);       
    }
}
