using UnityEngine;


public class Ability_Decoy : MonoBehaviour
{
    [SerializeField] float timerBeforeDestroy = 120f; // in seconds

    AI_Enemy_Combat[] ai_combats;

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

        // Now set it in ennemies to not have to search for a decoy every frame but just the distance between decoy and enemy
        // Optimization performance
        ai_combats = FindObjectsOfType<AI_Enemy_Combat>();

        for (int i = 0; i < ai_combats.Length; i++)
        {
            if (ai_combats[i])
                ai_combats[i].currentDecoy = this;
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < ai_combats.Length; i++)
        {
            if (ai_combats[i] && ai_combats[i].currentDecoy == this)
                ai_combats[i].currentDecoy = null;
        }
    }
}
