/* Player_Bank_Activator.cs :
 * Permet d'activer la banque du joueur afin qu'il puisse stocker / retirer des objets.
 * A attaché sur un objet représentant la banque du joueur.
 * 
 * 
 * */

public class Player_Bank_Activator : Interactable
{
    private void Start()
    {
        interactionType = PlayerInteractionType.None;
    }

    public override void Interact()
    {
        base.Interact();

        UI_Player.instance.ToggleBankUI(true);
    }

    public override void UnInteract()
    {
        base.UnInteract();

        UI_Player.instance.ToggleBankUI(false);
    }
}
