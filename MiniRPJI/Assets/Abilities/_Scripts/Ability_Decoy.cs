using UnityEngine;


public class Ability_Decoy : MonoBehaviour
{
    [SerializeField] float timerBeforeDestroy = 120f; // in seconds

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timerBeforeDestroy);

        // Check if there is another decoy in the scene, destroy it
        Ability_Decoy[] decoysInGame = FindObjectsOfType<Ability_Decoy>();

        foreach (Ability_Decoy decoy in decoysInGame)
        {
            if (decoy.gameObject != this.gameObject)
            {
                Destroy(decoy.gameObject);
            }
        }
    }
}
