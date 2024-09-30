using UnityEngine;

public class InvisibleBarrierSpawner : MonoBehaviour
{
    public GameObject InvisibleBarrierPrefab; 

    private bool InvisibleBarrierSpawned = false;

    void Start()
    {
        if (!InvisibleBarrierSpawned)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        Instantiate(InvisibleBarrierPrefab, transform.position, Quaternion.identity);
        InvisibleBarrierSpawned = true;
    }
}
