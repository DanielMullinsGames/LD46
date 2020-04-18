using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateObject : MonoBehaviour
{
    public Vector2 OriginalPosition { get; private set; }

    [SerializeField]
    private SpriteRenderer untiedLaces;

    private void Awake()
    {
        OriginalPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        untiedLaces.enabled = PlayerStateManager.Instance.ShoesUntied;
    }
}
