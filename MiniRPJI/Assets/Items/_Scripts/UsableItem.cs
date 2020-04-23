/* UsableItem.cs
Utilisé pour dérivé de BaseItem tout objets utilisable par le joueur 
(sera aussi considéré comme un objet d'inventaire par défaut)
Pour l'instant est seulement utilisé pour les potions
*/
using UnityEngine;

[CreateAssetMenu(fileName = "UsableItemName", menuName = "ScriptableObjects/Items/UsableItem", order = 1)]
public class UsableItem : BaseItem
{
    public int healthRegenerationPoints;
    public int energyRegenerationPoints;

    // Security if player is already full life or energy to not use the item (UI_Player_Inventory)
    public bool CanUse()
    {
        if (Player_Stats.instance)
        {
            if (healthRegenerationPoints != 0)
            {
                if (Player_Stats.instance.playerHealth.GetCurrentHealthPoints() >= Player_Stats.instance.playerHealth.GetTotalHealthPoints())
                {
                    return false;
                }
            }

            if (energyRegenerationPoints != 0)
            {
                if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= Player_Stats.instance.playerEnergy.GetTotalEnergyPoints())
                {
                    return false;
                }
            }
        }

        return true;
    }

    // For now we assume using an item will change something on player's stats.
    public void Use()
    {
        if (Player_Stats.instance)
        {
            if (healthRegenerationPoints != 0)
            {
                if (Player_Stats.instance.playerHealth.GetCurrentHealthPoints() >= Player_Stats.instance.playerHealth.GetTotalHealthPoints())
                {
                    return;
                }

                int tempHealth = Player_Stats.instance.playerHealth.GetCurrentHealthPoints() + healthRegenerationPoints;
                if (tempHealth > Player_Stats.instance.playerHealth.GetTotalHealthPoints())
                {
                    Player_Stats.instance.playerHealth.SetCurrentHealthPoints(Player_Stats.instance.playerHealth.GetTotalHealthPoints());
                }
                else
                {
                    Player_Stats.instance.playerHealth.SetCurrentHealthPoints(tempHealth);
                }
                
            }

            if (energyRegenerationPoints != 0)
            {
                if (Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() >= Player_Stats.instance.playerEnergy.GetTotalEnergyPoints())
                {
                    return;
                }

                int tempEnergy = Player_Stats.instance.playerEnergy.GetCurrentEnergyPoints() + energyRegenerationPoints;
                if (tempEnergy > Player_Stats.instance.playerEnergy.GetTotalEnergyPoints())
                {
                    Player_Stats.instance.playerEnergy.SetCurrentEnergyPoints(Player_Stats.instance.playerEnergy.GetTotalEnergyPoints());
                }
                else
                {
                    Player_Stats.instance.playerEnergy.SetCurrentEnergyPoints(tempEnergy);
                }


            }

            // Because of CanUse security, we can use potion sound here. /!\ TODO See if there is other tings than potions in UsableItem. /!\
            if (Sound_Manager.instance)
                Sound_Manager.instance.PlaySound(Sound_Manager.instance.asset.potUse);
        }
    }
}
