using UnityEngine;

public class Bonus_Unlocker : Interactable
{
    public Bonus_Type bonusType;

    [SerializeField] GameObject particles;

    public override void Interact()
    {
        base.Interact();

        if (FindObjectOfType<Player_Bonus>())
        {
            FindObjectOfType<Player_Bonus>().SetPlayerBonus(bonusType);
        }

        if (Sound_Manager.instance)
        {
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.playerBonus);
        }

        Destroy(particles);
        Destroy(this);
    }
}
