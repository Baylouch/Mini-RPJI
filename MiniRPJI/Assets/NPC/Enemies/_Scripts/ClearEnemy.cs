/* ClearEnemy.cs
 * 
 * Travail en cooperation avec RememberEnemyData.cs
 * 
 * A attaché sur chaque enemy du jeu afin que lorsque le joueur est trop loin, ses données soient transférées dans RememberEnemyData
 * afin de pouvoir supprimer le gameobject du jeu et libérer de la mémoire.
 * 
 * */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AI_Enemy_Movement))] // To know when AI reach its startPos to destroy him only when its not finding player
[RequireComponent(typeof(AI_Stats))]
public class ClearEnemy : MonoBehaviour
{
    float distanceToRecycle = 30f;

    Transform playerTransform;

    AI_Enemy_Movement movement;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<AI_Enemy_Movement>();

        if (Player_Stats.instance)
            playerTransform = Player_Stats.instance.transform;

        StartCoroutine(CheckDistanceToRecycle());
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

    // Coroutine to check if player is far enough to recycle with amount of time instead of each frame (initally as condition in Update()).
    IEnumerator CheckDistanceToRecycle()
    {
        // Ennemies goes to pos 0 with a coroutine starting in Start() but not when i put the same code in Update().
        // Its because when running in Update() player's scripts got time to be set with their Start() but not when we call this in Start() because of execution order.
        // To fix this i'll ask to wait the end of this frame before entering in the infinite loop
        yield return new WaitForEndOfFrame();

        while (true)
        {
            while (playerTransform == null)
            {
                if (Player_Stats.instance)
                    playerTransform = Player_Stats.instance.transform;

                yield return new WaitForSeconds(.5f);
            }

            if (Vector3.Distance(transform.position, playerTransform.position) > distanceToRecycle)
            {
                if (movement.reachedStartPos)
                {
                    // Clear the enemy
                    ClearTheEnemy();
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
