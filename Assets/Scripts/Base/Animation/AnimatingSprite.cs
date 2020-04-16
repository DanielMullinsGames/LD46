using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatingSprite : MonoBehaviour {

    [Header("Frames")]
	public List<Sprite> frames = new List<Sprite>();

    [SerializeField]
    private bool randomizeSprite;

    [Header("Animation")]
    [SerializeField]
    private float animSpeed = 0.033f;

    [SerializeField]
    private float animOffset = 0f;

    [SerializeField]
    private bool randomOffset;

    [SerializeField]
	private bool stopAfterSingleIteration = false;

	float timer;
	SpriteRenderer sR;
	[HideInInspector]
	public int frameIndex = 0;

	private bool stopOnNextFrame;

    void Awake()
    {
        sR = GetComponent<SpriteRenderer>();
    }

	void Start()
    {
        if (randomOffset)
        {
            animOffset = -Random.value * (frames.Count * animSpeed);
        }
		timer = animOffset;
	}

	public void StartAnimatingWithDecrementedIndex()
    {
		frameIndex--;
		StartAnimating ();
	}

	public void StartAnimating()
    {
		this.enabled = true;
		stopOnNextFrame = false;
	}

	public void StopAnimating()
    {
		stopOnNextFrame = true;
	}

    public void StopImmediate()
    {
        Stop();
    }

	public void StartFromBeginning()
    {
		this.enabled = true;
		frameIndex = 0;
	}

    public void IterateFrame()
    {
        if (stopOnNextFrame)
        {
            Stop();
            return;
        }

        timer = 0f;

        if (randomizeSprite)
        {
            int randomFrame = Random.Range(0, frames.Count);
            frameIndex = randomFrame;

            sR.sprite = frames[randomFrame];
        }
        else
        {
            frameIndex++;
            if (frameIndex >= frames.Count)
            {
                if (stopAfterSingleIteration)
                {
                    stopAfterSingleIteration = false;
                    this.enabled = false;
                    frameIndex--;
                }
                else {
                    frameIndex = 0;
                }
            }

            sR.sprite = frames[frameIndex];
        }
    }

	public void Clear()
    {
		this.enabled = false;
		sR.sprite = null;
	}

	private void Stop()
    {
		stopOnNextFrame = false;
		this.enabled = false;
        if (sR != null)
        {
            sR.sprite = frames[0];
        }
	}

	void Update()
    {
		timer += Time.deltaTime;
		if (timer > animSpeed)
        {
            IterateFrame();
		}
	}
}
