using UnityEngine;

[RequireComponent(typeof(PetUnlockerMovement))]
public class PetUnlocker : Interactable
{
    public PetConfig config;

    PetUnlockerMovement movement;
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<PetUnlockerMovement>();

        if (Player_Stats.instance)
        player = Player_Stats.instance.transform;

        interactionType = PlayerInteractionType.None;

        if (Player_Pets.instance)
        {
            if (Player_Pets.instance.GetPlayerPetByID(config.petID) != null)
                Destroy(gameObject);
        }

        if (Quests_Control.instance && Player_Pets.instance)
        {
            if (!Quests_Control.instance.GetQuestAchievement(Player_Pets.questIDToUnlockPets))
                Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            if (Vector3.Distance(transform.position, player.position) > 30f)
            {
                if (movement.enabled)
                {
                    movement.enabled = false;
                }
            }
            else
            {
                if (!movement.enabled)
                {
                    movement.enabled = true;
                }
            }
        }
    }

    public override void Interact()
    {
        base.Interact();

        if (Player_Pets.instance)
        {
            if (Player_Pets.instance.GetPlayerPetByID(config.petID) == null)
            {
                Player_Pets.instance.GetNewPet(config);

                // Valid success linked to the petID by incrementing Success_Objective attached
                if (GetComponent<Success_Objective>())
                    GetComponent<Success_Objective>().IncrementSuccessObjective();

                if (Sound_Manager.instance)
                {
                    Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.achievement);
                }
            }
        }

        Destroy(gameObject);
    }
}
