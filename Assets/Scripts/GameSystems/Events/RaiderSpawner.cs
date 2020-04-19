using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderSpawner : Singleton<RaiderSpawner>
{
    [SerializeField]
    private GameObject raiderPrefab;

    [SerializeField]
    private Transform leftWindowMin;

    [SerializeField]
    private Transform leftWindowMax;

    [SerializeField]
    private Transform rightWindowMin;

    [SerializeField]
    private Transform rightWindowMax;

    [SerializeField]
    private float timeToSpawn = 2f;

    public Raider CurrentRaider { get; private set; }

    private float spawnTimer;

    private void Update()
    {
        if (TrainProgressManager.Instance.NormalizedVelocity < 0.25f && CurrentRaider == null)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer > timeToSpawn)
            {
                spawnTimer = 0f;
                SpawnRaider();
            }
        }
    }

    public void OnReachedDestination()
    {
        enabled = false;
        if (CurrentRaider != null)
        {
            StartCoroutine(KillOffRemainingRaider());
        }
    }

    private IEnumerator KillOffRemainingRaider()
    {
        yield return new WaitUntil(() => CurrentRaider.CanBeShot);
        AudioController.Instance.PlaySound2D("gunshot_2", volume: 0.5f);
        OnRaiderShot();
    }

    public void OnRaiderShot()
    {
        CurrentRaider.Die();
        CurrentRaider = null;
        spawnTimer -= 2f;
    }

    private void SpawnRaider()
    {
        var raiderObj = Instantiate(raiderPrefab);
        CurrentRaider = raiderObj.GetComponent<Raider>();

        bool leftWindow = Random.value > 0.5f;
        float xMin = leftWindow ? leftWindowMin.localPosition.x : rightWindowMin.localPosition.x;
        float xMax = leftWindow ? leftWindowMax.localPosition.x : rightWindowMax.localPosition.x;
        float x = xMin + ((xMax - xMin) * Random.value);
        float y = leftWindowMin.localPosition.y;
        CurrentRaider.transform.parent = transform;
        CurrentRaider.transform.localPosition = new Vector2(x, y);
    }
}
