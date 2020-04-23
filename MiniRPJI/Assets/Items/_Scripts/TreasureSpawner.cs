/* TreasureSpawner.cs
 * Permet de spawn des coffres au trésor à un endroit défini ou aléatoire.
 * 
 * 
 * */

using UnityEngine;

public class TreasureSpawner : MonoBehaviour
{
    [SerializeField] GameObject treasurePrefab;

    [SerializeField] int treasuresNumb; // Numbers of treasure who'll try to spawn. It must be less than spawnPoses length!
    [SerializeField] float spawnRate; // Rate / 100

    Transform[] spawnPoses; // array that contains spawn poses, set in Start() taking children of this gameobject.
    bool[] posesAlreadyUsed; // Same size as spawnPoses array, to know if there is already a treasure on the index pose.

    // Start is called before the first frame update
    void Start()
    {
        spawnPoses = new Transform[transform.childCount];
        posesAlreadyUsed = new bool[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoses[i] = transform.GetChild(i);
            posesAlreadyUsed[i] = false;
        }

        SpawnTreasure();
    }

    void SpawnTreasure()
    {
        // Loop trough treasures number set
        for (int i = 0; i < treasuresNumb; i++)
        {
            bool willSpawn = spawnRate > Random.Range(0, 101); // Determine if treasure will spawn
            if (willSpawn)
            {
                // Security to know if there is a spawn pose available
                for (int j = 0; j < posesAlreadyUsed.Length; j++)
                {
                    if (posesAlreadyUsed[j] == false)
                    {
                        break; // There is a spawn pose available so go out of this loop.
                    }

                    // If we're at the last index and we still in the loop, check if its true, if yes return because no more spawn poses available.
                    if (j == posesAlreadyUsed.Length - 1)
                    {
                        if (posesAlreadyUsed[j] == true)
                        {
                            return; // Go out of the method.
                        }
                    }
                }

                // Get a spawnIndex
                int spawnIndex = Random.Range(0, spawnPoses.Length);
                // While the current spawnIndex isnt available, try to get an other.
                while (posesAlreadyUsed[spawnIndex] == true)
                {
                    spawnIndex = Random.Range(0, spawnPoses.Length);
                }

                // Spawn treasure
                GameObject treasureGO = Instantiate(treasurePrefab, spawnPoses[spawnIndex].position, Quaternion.identity);
                posesAlreadyUsed[spawnIndex] = true;

                if (GameObject.Find("Treasures"))
                {
                    treasureGO.transform.parent = GameObject.Find("Treasures").transform;
                }
                else
                {
                    GameObject treasuresParent = new GameObject("Treasures");
                    treasureGO.transform.parent = treasuresParent.transform;
                }
            }
        }

        // Clean useless gameobject
        Destroy(gameObject);
    }
}
