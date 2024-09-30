using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float initialDelay = 3f;
    public float minTime = 2f;
    public float maxTime = 4f;

    private void Start()
    {
        Invoke(nameof(StartSpawning), initialDelay);
    }

    private void StartSpawning()
    {
        Spawn();
        InvokeRepeating(nameof(Spawn), Random.Range(minTime, maxTime), Random.Range(minTime, maxTime));
    }

    private void Spawn()
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
