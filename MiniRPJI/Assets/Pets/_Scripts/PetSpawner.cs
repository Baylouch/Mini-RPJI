using UnityEngine;

public class PetSpawner : MonoBehaviour
{
    [Tooltip("Create 2 empty gameobjects and place them at the limit of the zone you want to be spawnable")]
    [SerializeField] Transform[] spawnZone = new Transform[2]; // [0] = top left, [1] = bottom right

    [SerializeField] GameObject petToSpawn;

    [SerializeField] Color gizmosColor = Color.yellow;

    [SerializeField] float percentChanceToSpawn;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;

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

        SpawnPet();
    }

    void SpawnPet()
    {
        bool willSpawn = percentChanceToSpawn > Random.Range(0, 100);

        if (willSpawn)
        {
            // First get spawn position
            Vector3 spawnPos = new Vector3(Random.Range(spawnZone[0].position.x, spawnZone[1].position.x), Random.Range(spawnZone[0].position.y, spawnZone[1].position.y), 0f);

            GameObject currentPet = Instantiate(petToSpawn, spawnPos, Quaternion.identity);

            // Set in the hierarchy
            if (GameObject.Find("Pets"))
            {
                currentPet.transform.parent = GameObject.Find("Pets").transform;
            }
            else
            {
                GameObject petsFolder = new GameObject("Pets");
                petsFolder.transform.position = Vector3.zero;
                currentPet.transform.parent = GameObject.Find("Pets").transform;
            }
        }

        // Then finaly we can destroy gameObject because no more use for it.
        Destroy(gameObject);
    }
}
