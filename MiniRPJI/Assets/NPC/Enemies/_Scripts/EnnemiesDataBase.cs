using UnityEngine;

[CreateAssetMenu(fileName = "EnnemiesDataBase", menuName = "ScriptableObjects/Ennemies/DataBase", order = 1)]
public class EnnemiesDataBase : ScriptableObject
{
    [SerializeField] GameObject[] ennemiesPrefabs;

    public GameObject GetEnemyPrefabByID(int _ID)
    {
        for (int i = 0; i < ennemiesPrefabs.Length; i++)
        {
            if (ennemiesPrefabs[i].GetComponent<AI_Stats>())
            {
                if (ennemiesPrefabs[i].GetComponent<AI_Stats>().enemyID == _ID)
                {
                    return ennemiesPrefabs[i];
                }
            }
        }

        return null;
    }
}
