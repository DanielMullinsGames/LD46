using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedRotation : MonoBehaviour
{
    public float rotation;

	void LateUpdate ()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, rotation);
	}
}
