/* SubZoneTrigger.cs
 * Permet d'entrée dans une sous-level du level courant via trigger.
 * 
 * */

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Sub_Zone_Trigger : Interactable
{
    // Use build index of scenes.
    [SerializeField] int levelFrom;
    [SerializeField] int levelToGo;

    private void Start()
    {
        interactionType = PlayerInteractionType.SubZoneTrigger;
    }

    public override void Interact()
    {
        if (Scenes_Control.instance)
        {
            Scenes_Control.instance.SwitchPlayerLevel(levelFrom, levelToGo);
        }
    }
   
}
