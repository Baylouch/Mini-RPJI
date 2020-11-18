/* Enemy_Activator.cs
 * 
 * Va permettre d'activer les ennemies sous forme de "RememberEnemyData" estimé assez proche du joueur.
 * 
 * Ce script a été crée car l'ancienne utilisation de RememberEnemyData.cs consisté à : chaque frame checker si le joueur est assez proche.
 * Etant donné le nombre d'ennemies sous cette forme présent par scène, j'ai finalement trouvé plus optimisé de centraliser ces appels,
 * afin qu'ils ne soient fait qu'une fois par frame / temps donné.
 * 
 * 
 * */


using UnityEngine;
using System.Collections;

public class Enemy_Activator : MonoBehaviour
{
    Coroutine currentCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(CheckForEnemyActivation());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        currentCoroutine = null;
    }

    private void OnEnable()
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(CheckForEnemyActivation());
    }

    IEnumerator CheckForEnemyActivation()
    {
        while (true)
        {
            RememberEnemyData[] ennemiesData = FindObjectsOfType<RememberEnemyData>();

            for (int i = 0; i < ennemiesData.Length; i++)
            {
                if (Vector3.Distance(transform.position, ennemiesData[i].transform.position) <= ennemiesData[i].distanceToActivate)
                {
                    // Respawn the enemy with registered stats
                    ennemiesData[i].RespawnEnemy();
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
