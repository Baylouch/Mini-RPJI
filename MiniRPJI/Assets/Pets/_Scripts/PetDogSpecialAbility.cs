/* PetDogSpecialAbility.cs
 * 
 * Permet au chien d'aléatoirement quand le joueur s'arrête plus de 30 secondes de déterrer un objet.
 * 
 * */

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PetMovement))]
[RequireComponent(typeof(ItemDroper))]
public class PetDogSpecialAbility : MonoBehaviour
{
    [SerializeField] float minTimerToDig = 30f; // in seconds, represent the timer when player stop movement before dog start to dig
    [SerializeField] float maxTimerToDig = 120f;

    [SerializeField] float digTimer = 5f;

    [SerializeField] GameObject diggingEffect;
    [SerializeField] GameObject holeEffect;

    PetMovement petMovement;
    ItemDroper itemDroper;
    Vector3 currentPlayerPosition;
    Player_Combat playerCombat;

    float currentTimerToDig; // Set random between min and max timer to dig
    float timeWhenPlayerStopMoving; // Set with Time.time
    bool dogStartedToDig = false;

    // Start is called before the first frame update
    void Start()
    {
        petMovement = GetComponent<PetMovement>();
        itemDroper = GetComponent<ItemDroper>();

        playerCombat = FindObjectOfType<Player_Combat>();

        if (playerCombat)
        {
            currentPlayerPosition = playerCombat.transform.position;
        }

        currentTimerToDig = Random.Range(minTimerToDig, maxTimerToDig);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCombat)
        {
            // Check player position to set the right timer to start to dig
            if (currentPlayerPosition != playerCombat.transform.position)
            {
                currentPlayerPosition = playerCombat.transform.position;
                timeWhenPlayerStopMoving = Time.time;
            }
            else
            {
                // Before all check if player isn't in combat mode
                if (!playerCombat.isInCombat)
                {
                    // Check if the time set when player stopped movements plus current timer become less than the current time game.
                    if (timeWhenPlayerStopMoving + currentTimerToDig < Time.time)
                    {
                        // Dog start to dig
                        // First we must check if action started to not flood it. A simple bool could make the job
                        if (!dogStartedToDig)
                        {
                            dogStartedToDig = true;

                            // Then we must create a random position around the player.
                            // The processus isnt that simple because we must set a target to PetMovement.cs to make the pet moveable.
                            // So we'll just create a new gameobject to the random determined pos
                            Vector3 randomPos = new Vector3(Random.Range(playerCombat.transform.position.x - 7f, playerCombat.transform.position.x + 7f),
                                                            Random.Range(playerCombat.transform.position.y - 7f, playerCombat.transform.position.y + 7f),
                                                            0);

                            GameObject digPositionGO = new GameObject("DigPosition");
                            digPositionGO.transform.position = randomPos;

                            // Now we go our gameobject representing the new pet target.
                            petMovement.SetTarget(digPositionGO.transform);

                            // Now we need a way to wait until pet reach the dig position to start to dig.
                            // Coroutine seems be the solution.
                            StartCoroutine(StartDigProcess(randomPos));
                        }
                    }
                }
            }
        }
    }

    // Method to know when pet reach the dig position
    bool PetReachPosition(Vector3 posToReach)
    {
        return Vector3.Distance(transform.position, posToReach) < 1.3f; // To see if 1.3f is a good number. After some testing seems good. Carefull if changing
        // StoppingDistance from PetMovement.cs, can make trouble here.
    }

    IEnumerator StartDigProcess(Vector3 digPosition)
    {
        // Just a security because of the random position, it's possible sometimes dig position spawn in a unreachable position.
        // To fix it i put a destroy timer on this script -> 1min seems great.
        Destroy(this, 60f);

        // We wait until pet reached the dig position
        yield return new WaitUntil(() => PetReachPosition(digPosition) == true);

        // Debug.Log("Dog reached the dig position !");

        // Instantiate Particles Effect
        GameObject digEffect = Instantiate(diggingEffect, transform.position, transform.rotation);
        Destroy(digEffect, 5f);

        yield return new WaitForSeconds(digTimer);

        // After digTimer instantiate a random item
        int itemsLevel = playerCombat.GetComponent<Player_Stats>().GetCurrentLevel();
        itemDroper.DropItems(itemsLevel);

        // Instantiate the Hole Effect
        GameObject _holeEffect = Instantiate(holeEffect, transform.position, transform.rotation);

        // Destroy this script.
        Destroy(this);
    }

    private void OnDestroy()
    {
        // Reset dog target to be sure when script is destroy by anyway to switch on player
        petMovement.SetTarget(playerCombat.transform);
    }
}
