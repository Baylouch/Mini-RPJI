/* RememberEnemyData.cs
 * 
 * Permet de mémoriser un enemy avec ses statistiques.
 * 
 * Ce script est utilisé lorsque le joueur est éloigné suffisemment d'un enemy, le gameObject et certaines stats seront mémorisées afin de pouvoir
 * détruire l'ennemi dans le jeu et ne retenir qu'un petit ensemble de données.
 * 
 * Le but est d'économiser de la mémoire et de rendre le jeu plus performant.
 * 
 * 
 * */

using UnityEngine;

public class RememberEnemyData : MonoBehaviour
{
    public int enemyID; 

    public int enemyLevel;
    public int enemyCurrentHealth;

    public Vector3 spawnPosition;

    public float distanceToActivate = 27f; // We want it public to check via Enemy_Activator.cs on the player. Maybe create a getter, maybe f*** it.

    //Transform playerTransform;

    bool isSet = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, 2f);
    }

    // TODO DELETE
    // Start is called before the first frame update
    //void Start()
    //{
    //    if (Player_Stats.instance)
    //        playerTransform = Player_Stats.instance.transform;

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (playerTransform == null)
    //    {
    //        if (Player_Stats.instance)
    //            playerTransform = Player_Stats.instance.transform;

    //        return;
    //    }

    //    // TODO Change it because when we got 30+ of this searching each frames it's cpu burning
    //    if (Vector3.Distance(transform.position, playerTransform.position) <= distanceToActivate)
    //    {
    //        // Respawn the enemy with registered stats
    //        if (!isSet)
    //        {
    //            isSet = true;
    //            RespawnEnemy();
    //        }
    //    }
    //}

    // This method is public because we want to acces via Enemy_Activator.cs on the player.
    public void RespawnEnemy()
    {
        if (EnnemiesDataBaseGameObject.instance)
        {
            if (EnnemiesDataBaseGameObject.instance.database.GetEnemyPrefabByID(enemyID) == null)
            {
                Debug.Log("Wrong enemy ID.");
                return;
            }
        }

        GameObject enemyGO = Instantiate(EnnemiesDataBaseGameObject.instance.database.GetEnemyPrefabByID(enemyID), spawnPosition, Quaternion.identity);

        if (GameObject.Find("Enemies"))
            enemyGO.transform.parent = GameObject.Find("Enemies").transform;
        else
        {
            GameObject enemiesFolder = new GameObject("Enemies");
            enemiesFolder.transform.position = Vector3.zero;
            enemyGO.transform.parent = GameObject.Find("Enemies").transform;
        }

        enemyGO.GetComponent<AI_Stats>().InitializeEnemy(enemyLevel, enemyCurrentHealth);

        Destroy(gameObject);
    }
}
