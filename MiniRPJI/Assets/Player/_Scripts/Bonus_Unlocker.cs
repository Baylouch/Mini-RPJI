using UnityEngine;

public class Bonus_Unlocker : Interactable
{
    public Bonus_Type bonusType;

    [SerializeField] GameObject particles;
    [SerializeField] GameObject smokeEffect;

    public override void Interact()
    {
        base.Interact();

        Player_Bonus player_Bonus = FindObjectOfType<Player_Bonus>();

        if (player_Bonus)
        {
            player_Bonus.SetPlayerBonus(bonusType);

            if (smokeEffect)
            {
                GameObject smokeEffectInstance = Instantiate(smokeEffect, player_Bonus.transform);
                smokeEffectInstance.transform.position = player_Bonus.transform.position;
                Destroy(smokeEffectInstance, 2f);
            }
        }

        if (Sound_Manager.instance)
        {
            Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.playerBonus);
        }

        Destroy(particles);
        Destroy(this);
    }
}
