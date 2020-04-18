using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingConveyor : MonoBehaviour
{
    [SerializeField]
    private GameObject repeatingObject = default;

    [SerializeField]
    private float span = default;

    [SerializeField]
    private float objectSpacing = default;

    [SerializeField]
    private float moveSpeed = default;

    private List<Transform> objects = new List<Transform>();

    private float RightBound { get { return span * 0.5f; } }
    private float LeftBound { get { return span * -0.5f; } }

    private void Start()
    {
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        int numObjects = Mathf.RoundToInt(span / objectSpacing);
        for (int i = 0; i < numObjects; i++)
        {
            var obj = Instantiate(repeatingObject, transform);

            float xPos = RightBound + (objectSpacing * i);
            obj.transform.localPosition = new Vector3(xPos, obj.transform.localPosition.y, 0f);

            objects.Add(obj.transform);
        }

        repeatingObject.SetActive(false);
    }

    void Update()
    {
        foreach (Transform obj in objects)
        {
            obj.localPosition += Vector3.left * moveSpeed * Time.deltaTime;
            if (obj.localPosition.x < LeftBound)
            {
                obj.localPosition = new Vector3(RightBound, obj.localPosition.y, obj.localPosition.z);
            }
        }
    }
}
