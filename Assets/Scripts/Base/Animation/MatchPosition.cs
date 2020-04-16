using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchPosition : MonoBehaviour 
{
    public Transform target;
    public bool x = true;
    public bool y = true;
    public bool z = true;
	public bool destroyIfTargetNull;

    void LateUpdate()
    {
		if (target != null)
		{
			Vector3 pos = new Vector3 (
				              x ? target.transform.position.x : transform.position.x,
				              y ? target.transform.position.y : transform.position.y,
				              z ? target.transform.position.z : transform.position.z
			              );
            transform.position = pos;
        }
		else if (destroyIfTargetNull)
		{
			Destroy (gameObject);
		}
    }
}