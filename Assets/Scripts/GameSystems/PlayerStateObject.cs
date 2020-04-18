using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateObject : MonoBehaviour
{
    public Vector2 OriginalPosition { get; private set; }

    private void Awake()
    {
        OriginalPosition = transform.localPosition;
    }
}
