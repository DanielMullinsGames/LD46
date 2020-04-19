using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderSpawner : MonoBehaviour
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

    private Raider currentRaider;

    private float spawnTimer;

    private void Update()
    {
        if (TrainProgressManager.Instance.NormalizedVelocity < 0.25f && currentRaider == null)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer > timeToSpawn)
            {
                SpawnRaider();
            }
        }
    }

    private void SpawnRaider()
    {
        var raiderObj = Instantiate(raiderPrefab);
        currentRaider = raiderObj.GetComponent<Raider>();

        bool leftWindow = Random.value > 0.5f;
        float xMin = leftWindow ? leftWindowMin.localPosition.x : rightWindowMin.localPosition.x;
        float xMax = leftWindow ? leftWindowMax.localPosition.x : rightWindowMax.localPosition.x;
        float x = xMin + ((xMax - xMin) * Random.value);
        float y = leftWindowMin.localPosition.y;
        currentRaider.transform.parent = transform;
        currentRaider.transform.localPosition = new Vector2(x, y);
    }
}
