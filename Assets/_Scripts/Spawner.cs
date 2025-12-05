using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] tetrominos;

    void Start()
    {
        SpawnNext();
    }

    public void SpawnNext()
    {
        // Random Index
        int i = Random.Range(0, tetrominos.Length);

        // Spawn it at (5, 20) which is Top-Center
        Instantiate(tetrominos[i], transform.position, Quaternion.identity);
    }
}