/* FastTeleportation.cs
 * 
 * Permet au joueur d'être teleporter à un endroit dans une même scene.
 * 
 * 
 * 
 * */

using UnityEngine;

public class FastTeleportation : Interactable
{
    [SerializeField] Transform placeToGo;

    private void Start()
    {
        interactionType = PlayerInteractionType.None;
    }

    public override void Interact()
    {
        base.Interact();

        // Set player position
        FindObjectOfType<Player_Movement>().transform.position = placeToGo.position;


        // Set pet's player position
        if (FindObjectOfType<PetMovement>())
        {
            FindObjectOfType<PetMovement>().transform.position = FindObjectOfType<Player_Movement>().transform.position;
        }

        if (Sound_Manager.instance)
        {
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.teleportSound);
        }
    }
}
