/* Player_Activator.cs
 * 
 * Permet d'activer les ennemies quand le joueur se trouve à une certaine distance
 * 
 * Ce script est lié aux scripts AI_Activator mis sur chaque ennemies activable / desactivable dans la scene.
 * 
 * */

using System.Collections;
using UnityEngine;

public class Player_Activator : MonoBehaviour
{
    [SerializeField] float activationDistance = 30f; // TODO Link to desactivation distance


    private void Start()
    {
        StartCoroutine(CheckForEnnemiesActivation());
    }

    // TODO Find a way to check if there is no more AI_Activator in the scene to StopCoroutine,
    // then when a new level is load (so maybe new AI_Activator), StartCoroutine again.
    IEnumerator CheckForEnnemiesActivation()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            AI_Activator[] activators = FindObjectsOfType<AI_Activator>();

            for (int i = 0; i < activators.Length; i++)
            {
                // Get the distance between player and current AI_Activator
                float distance = Vector3.Distance(transform.position, activators[i].transform.position);

                if (distance <= activationDistance && activators[i].IsAIActivate() == false)
                {
                    activators[i].ActiveBehaviours();
                    continue; // If AI is in the activationRange, active it, then move to the next iteration
                }
            }
        }
    }

    private void OnEnable()
    {
        StartCoroutine(CheckForEnnemiesActivation());
    }

    // Security methods, not really used.
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
