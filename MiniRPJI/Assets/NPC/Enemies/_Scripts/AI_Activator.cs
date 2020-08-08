/* AI_Activator :
 * 
 * Permet d'activer un ennemi lorsque le joueur se trouve à une certaine distance ou de le désactiver quand le joueur est loin.
 * 
 * Le joueur activera les objets via le script "Player_Activator" qui cherchera chaque seconde s'il rencontre AI_Activator à une certaine distance.
 * 
 *  * SCRIPT USELESS DEPUIS REMEMBERENEMYDATA.CS / CLEARENEMY.CS
 * */

using UnityEngine;

public class AI_Activator : MonoBehaviour
{
    [SerializeField] float desactivationDistance = 30f; // Use same or superior value than activationDistance in Player_Activator

    bool aiActivated = false;
    public bool IsAIActivate()
    {
        return aiActivated;
    }

    Transform playerTransform;

    Behaviour[] behaviours; // Contains all behaviour to disable / enable
    GameObject[] childrenOfThis; // Contains all child to unactive / active

    // Start is called before the first frame update
    void Start()
    {
        behaviours = GetComponents<Behaviour>();
        
        if (Player_Stats.instance)
        {
            playerTransform = Player_Stats.instance.transform;
        }

        // Invoke 2 seconds after scene started method to check if player is out of desactivationDistance
        //Invoke("CheckForDesactivation", 2f);
        CheckForDesactivation();
    }

    private void Update()
    {
        // To desactive AI when player is out of desactivationDistance
        if (aiActivated)
        {
            CheckForDesactivation();
        }
    }

    void CheckForDesactivation()
    {
        if (playerTransform == null)
        {
            if (Player_Stats.instance)
            {
                playerTransform = Player_Stats.instance.transform;
            }
            else
            {
                return;
            }
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > desactivationDistance)
        {
            DesactiveBehaviours();
        }
    }

    // Method to disable component when player is far from it.
    // Transform and Rigidbody2D are not in the behaviours array because they're not "disablable".
    public void DesactiveBehaviours()
    {
        aiActivated = false;

        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] == null)
                return;

            // Debug.Log(behaviours[i].GetType());
            // Check type you don't want to disable then continue to the next iteration
            if (behaviours[i].GetType() == this.GetType())
            {
                //Debug.Log("We found AI_Activator component ! ");
                continue;
            }

            // Disable behaviours
            behaviours[i].enabled = false;
            // Disable child gameobjects
            for (int j = 0; j < transform.childCount; j++)
            {
                transform.GetChild(j).gameObject.SetActive(false);
            }
        }

    }

    // Method to enable components when player is close from it
    public void ActiveBehaviours()
    {
        aiActivated = true;

        if (behaviours != null)
        {
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] && behaviours[i].enabled == false)
                {
                    behaviours[i].enabled = true;
                }

                for (int j = 0; j < transform.childCount; j++)
                {
                    transform.GetChild(j).gameObject.SetActive(true);
                }
            }
        }
    }
}
