/* Teleportail.cs
 * 
 * Permet au joueur de faire une teleportation rapide entre les scenes.
 * 
 * */

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Teleportail : Interactable
{
    [SerializeField] private int teleportailID = 0; // Represent the unique ID of the teleportail, linked to array's index in UI_Teleporter.
    public int GetTeleportailID()
    {
        return teleportailID;
    }
    [SerializeField] private int teleportailBuildIndex = 0; // Represent the build index of the level where player will be tp

    bool unlocked = false;
    public bool GetUnlocked()
    {
        return unlocked;
    }
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        interactionType = PlayerInteractionType.None;

        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Unlock TP
            if (UI_Player.instance)
            {
                if (UI_Player.instance.teleporterUI && !UI_Player.instance.teleporterUI.GetUnlockedTp(teleportailID))
                {
                    UI_Player.instance.teleporterUI.SetUnlockedTp(teleportailID, true);
                    ConfigUnlockedTP();
                }
            }
        }
    }

    public void ConfigUnlockedTP()
    {
        unlocked = true;
        anim.enabled = true;
    }

    public override void Interact()
    {
        base.Interact();

        // Open UI_Teleporter via UI_Player.instance
        if (UI_Player.instance)
        {
            if (UI_Player.instance.teleporterUI)
            {
                UI_Player.instance.ToggleTeleporterMenu(true);
                UI_Player.instance.teleporterUI.currentTeleportail = teleportailBuildIndex;
            }
        }
    }

    public override void UnInteract()
    {
        base.UnInteract();

        if (UI_Player.instance)
        {
            if (UI_Player.instance.teleporterUI)
            {
                UI_Player.instance.ToggleTeleporterMenu(false);
                UI_Player.instance.teleporterUI.currentTeleportail = -1;
            }
        }
    }
}
