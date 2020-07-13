/* PetCatSpecialAbility.cs
 * 
 * Permet au chat de soigner plus vite le joueur lorsque celui-ci n'est pas en mode combat.
 * 
 * 
 * */


using UnityEngine;

public class PetCatSpecialAbility : MonoBehaviour
{
    Player_Health playerHealth;
    Player_Combat playerCombat;

    bool processingAbility = false;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = FindObjectOfType<Player_Health>();
        playerCombat = FindObjectOfType<Player_Combat>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth && playerCombat)
        {
            if (!playerCombat.isInCombat)
            {
                if (playerHealth.GetCurrentHealthPoints() < playerHealth.GetTotalHealthPoints())
                {
                    if (!processingAbility)
                    {
                        processingAbility = true;

                        InvokeRepeating("RegenPlayerHealth", 0f, .5f);
                    }             
                }
                else
                {
                    if (processingAbility)
                    {
                        CancelInvoke();
                        processingAbility = false;
                    }
                }
            }
            else
            {
                if (processingAbility)
                {
                    CancelInvoke();
                    processingAbility = false;
                }
            }
        }
    }

    void RegenPlayerHealth()
    {
        playerHealth.SetCurrentHealthPoints(playerHealth.GetCurrentHealthPoints() + 5f);
    }
}
