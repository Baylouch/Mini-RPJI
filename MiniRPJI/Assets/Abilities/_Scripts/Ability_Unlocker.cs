/* Ability_Unlocker.cs
 * 
 * A attaché sur un gameobject pour le rendre interactif.
 * Permet au joueur de débloquer une abilité
 * 
 * Ancien script créé avant le nouveau système d'abilité.
 * 
 * */

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Ability_Unlocker : Interactable
{
    //[SerializeField] Ability_Config ability;

    //SpriteRenderer rend;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    interactionType = PlayerInteractionType.None;

    //    // Security if player's already got this ability (just destroy gameobject)
    //    if (Player_Abilities.instance)
    //    {
    //        if (Player_Abilities.instance.GetUnlockAbility(ability.abilityID))
    //        {
    //            Destroy(gameObject);
    //        }
    //    }

    //    rend = GetComponent<SpriteRenderer>();
        
    //    if (ability)
    //    {
    //        rend.sprite = ability.abilitySprite;
    //    }
    //}

    //public override void Interact()
    //{
    //    base.Interact();

    //    if (ability != null)
    //    {
    //        // Check if we have acces to player's abilities
    //        if (Player_Abilities.instance)
    //        {
    //            // Verify player hasn't unlock this ability yet
    //            if (!Player_Abilities.instance.GetUnlockAbility(ability.abilityID))
    //            {
    //                // Now we can unlock ability
    //                Player_Abilities.instance.SetUnlockAbility(ability.abilityID, true);

    //                // If player already display abilities, we undisplay it to refresh
    //                UI_Player_Abilities.instance.ResetAbilitiesPanel();

    //                // Play a sound to indicate player he gots a new ability !
    //                if (Sound_Manager.instance)
    //                {
    //                    Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.unlockAbility);
    //                }
    //            }

    //            // In all case now we can destroy gameobject
    //            Destroy(gameObject);
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("You must set an ability on " + gameObject.name);
    //    }
    //}
}
