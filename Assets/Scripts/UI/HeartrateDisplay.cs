using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartrateDisplay : MonoBehaviour
{
    [SerializeField]
    private float scrollSpeed;

    [SerializeField]
    private float pieceWidth;

    [SerializeField]
    private List<Transform> pieces;

    [SerializeField]
    private Transform leftMarker;

    [SerializeField]
    private Transform rightMarker;

    [SerializeField]
    private List<Sprite> rateSprites;

    public bool noSpriteChange;

    private void Update()
    {
        foreach (Transform piece in pieces)
        {
            float x = piece.localPosition.x + (Time.deltaTime * scrollSpeed);
            piece.localPosition = new Vector2(x, piece.localPosition.y);
            if (piece.localPosition.x > rightMarker.localPosition.x + (pieceWidth / 2f))
            {
                piece.localPosition = new Vector2(leftMarker.localPosition.x - (pieceWidth / 2f), piece.localPosition.y);
                SetRateSprite(piece);
            }
        }
    }

    private void SetRateSprite(Transform piece)
    {
        if (!noSpriteChange)
        {
            piece.GetComponentInChildren<SpriteRenderer>().sprite = rateSprites[Mathf.RoundToInt(HeartMachine.Instance.Vitality * (rateSprites.Count -1))];
        }
    }
}
