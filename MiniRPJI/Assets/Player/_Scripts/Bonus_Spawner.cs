using UnityEngine;

public class Bonus_Spawner : MonoBehaviour
{
    [SerializeField] GameObject[] bonusUnlockerPrefab;

    [SerializeField] int bonusNumb; // Numbers of treasure who'll try to spawn. It must be less than spawnPoses length!
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

        SpawnBonus();
    }

    void SpawnBonus()
    {
        // Loop trough bonus number set
        for (int i = 0; i < bonusNumb; i++)
        {
            bool willSpawn = spawnRate > Random.Range(0, 101); // Determine if bonus will spawn
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

                // Spawn bonus
                // Determine which one will be spawn in the bonus unlocker array
                int bonusIndex = 0;
                if (bonusUnlockerPrefab.Length > 1)
                {
                    bonusIndex = Random.Range(0, bonusUnlockerPrefab.Length);
                }

                GameObject bonusGO = Instantiate(bonusUnlockerPrefab[bonusIndex], spawnPoses[spawnIndex].position, Quaternion.identity);
                posesAlreadyUsed[spawnIndex] = true;

                if (GameObject.Find("Bonus"))
                {
                    bonusGO.transform.parent = GameObject.Find("Bonus").transform;
                }
                else
                {
                    GameObject bonusParent = new GameObject("Bonus");
                    bonusParent.transform.parent = gameObject.transform.parent;
                    bonusGO.transform.parent = bonusParent.transform;
                }
            }
        }

        // Clean useless gameobject
        Destroy(gameObject);
    }
}
