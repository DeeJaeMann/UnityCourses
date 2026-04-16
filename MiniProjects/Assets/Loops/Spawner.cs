using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private int spawnCount = 10;
    [SerializeField] private float spawnOffset = 1.5f;

    void Start()
    {
        // Instantiate(spawnPrefab, Vector3.zero, Quaternion.identity);
        Spawn();
    }

    void Spawn()
    {
        for (int index = 0; index < spawnCount; index++)
        {
            Vector3 spawnPosition = new Vector3(index * spawnOffset, 0, 0);
            Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
