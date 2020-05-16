using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    [Tooltip("Create 2 empty gameobjects and place them at the limit of the zone you want to be spawnable")]
    [SerializeField] Transform[] spawnZone = new Transform[2]; // [0] = top left, [1] = bottom right

    [SerializeField] int levelMin = 1;
    [SerializeField] int levelMax = 3;

    [SerializeField] int ennemiesMin = 2;
    [SerializeField] int ennemiesMax = 4;

    [SerializeField] GameObject[] ennemiesToSpawn;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Calculation line by line to draw a cube who show spawn zone limits
        Vector3 TopRightCorner = new Vector3(spawnZone[1].position.x, spawnZone[0].position.y, 0f);
        Vector3 BottomLeftCorner = new Vector3(spawnZone[0].position.x, spawnZone[1].position.y, 0f);

        Vector3 BottomRightCorner = spawnZone[1].position;
        Vector3 TopLeftCorner = spawnZone[0].position;

        Gizmos.DrawLine(TopLeftCorner, TopRightCorner);
        Gizmos.DrawLine(TopLeftCorner, BottomLeftCorner);
        Gizmos.DrawLine(TopRightCorner, BottomRightCorner);
        Gizmos.DrawLine(BottomLeftCorner, BottomRightCorner);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (spawnZone.Length != 2)
        {
            Debug.LogWarning("There is no spawnZone available. Please fix before continue.");
            return;
        }

        SpawnEnnemies();
    }

    void SpawnEnnemies()
    {
        // Get how many ennemies will spawn
        int ennemiesNumber = Random.Range(ennemiesMin, ennemiesMax + 1);

        // Now we can loop to create and set ennemies
        for (int i = 0; i < ennemiesNumber; i++)
        {
            // First get spawn position
            Vector3 spawnPos = new Vector3(Random.Range(spawnZone[0].position.x, spawnZone[1].position.x), Random.Range(spawnZone[0].position.y, spawnZone[1].position.y), 0f);

            // Get the index of the enemy who'll spawn
            int enemyIndex = Random.Range(0, ennemiesToSpawn.Length);

            // Instantiate the enemy
            GameObject currentEnemy = Instantiate(ennemiesToSpawn[enemyIndex], spawnPos, Quaternion.identity);

            // Set in the hierarchy
            if (GameObject.Find("Enemies"))
            {
                currentEnemy.transform.parent = GameObject.Find("Enemies").transform;
            }
            else
            {
                GameObject enemiesFolder = new GameObject("Enemies");
                enemiesFolder.transform.position = Vector3.zero;
                currentEnemy.transform.parent = GameObject.Find("Enemies").transform;
            }

            // Get its AI_Stats component to set his level
            AI_Stats enemyStats = currentEnemy.GetComponent<AI_Stats>();
            if (enemyStats)
            {
                // Get the enemy level
                int enemyLevel = Random.Range(levelMin, levelMax + 1);
                enemyStats.InitializeEnemy(enemyLevel);
            }
            else
            {
                Debug.LogWarning("Enemy has no AI_Stats ! Fix it.");
                return;
            }
        }
        // Then finaly we can destroy gameObject because no more use for it.
        Destroy(gameObject);
    }
}
