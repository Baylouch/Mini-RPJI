/* ClearEnemy.cs
 * 
 * Travail en cooperation avec RememberEnemyData.cs
 * 
 * A attaché sur chaque enemy du jeu afin que lorsque le joueur est trop loin, ses données soient transférées dans RememberEnemyData
 * afin de pouvoir supprimer le gameobject du jeu et libérer de la mémoire.
 * 
 * */

using UnityEngine;

[RequireComponent(typeof(AI_Enemy_Movement))] // To know when AI reach its startPos to destroy him only when its not finding player
[RequireComponent(typeof(AI_Stats))]
public class ClearEnemy : MonoBehaviour
{
    float distanceToRecycle = 23f;

    Transform playerTransform;

    AI_Enemy_Movement movement;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<AI_Enemy_Movement>();

        if (Player_Stats.instance)
            playerTransform = Player_Stats.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null)
        {
            if (Player_Stats.instance)
                playerTransform = Player_Stats.instance.transform;

            return;
        }

        if (Vector3.Distance(transform.position, playerTransform.position) > distanceToRecycle)
        {
            if (movement.reachedStartPos)
            {
                // Clear the enemy
                ClearTheEnemy();
            }
        }
    }

    void ClearTheEnemy()
    {
        GameObject EnemyDataGO = new GameObject(transform.name + " data");

        if (GameObject.Find("Enemies"))
            EnemyDataGO.transform.parent = GameObject.Find("Enemies").transform;
        else
        {
            GameObject enemiesFolder = new GameObject("Enemies");
            enemiesFolder.transform.position = Vector3.zero;
            EnemyDataGO.transform.parent = GameObject.Find("Enemies").transform;
        }

        RememberEnemyData enemyData = EnemyDataGO.AddComponent<RememberEnemyData>();

        enemyData.transform.position = movement.GetStartPos();

        enemyData.spawnPosition = movement.GetStartPos();
        enemyData.enemyID = GetComponent<AI_Stats>().enemyID;
        enemyData.enemyLevel = GetComponent<AI_Stats>().GetLevel();
        enemyData.enemyCurrentHealth = GetComponent<AI_Stats>().GetCurrentHealthPoints();

        Destroy(gameObject);
    }
}
