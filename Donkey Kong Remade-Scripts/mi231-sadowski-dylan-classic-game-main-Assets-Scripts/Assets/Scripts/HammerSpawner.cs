using UnityEngine;

public class HammerSpawner : MonoBehaviour
{
    public GameObject hammerPrefab; 
    private GameObject currentHammerInstance; 
    void Start()
    {
        SpawnHammer();
    }

    void SpawnHammer()
    {
        if (currentHammerInstance == null)
        {
            currentHammerInstance = Instantiate(hammerPrefab, transform.position, Quaternion.identity);
        }
    }

    public void RespawnHammer()
    {
        Destroy(currentHammerInstance);
        currentHammerInstance = Instantiate(hammerPrefab, transform.position, Quaternion.identity); 
    }
}
