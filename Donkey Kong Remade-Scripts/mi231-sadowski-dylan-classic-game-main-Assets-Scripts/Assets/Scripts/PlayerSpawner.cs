using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; 
    public HammerSpawner hammerSpawner; 

    private bool playerSpawned = false;

    void Start()
    {
        if (!playerSpawned)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        Instantiate(playerPrefab, transform.position, Quaternion.identity);
        playerSpawned = true;

        hammerSpawner.RespawnHammer();
    }
}
