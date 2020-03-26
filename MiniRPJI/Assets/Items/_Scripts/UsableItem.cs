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

    // For now we assume using an item will change something on player's stats.
    public void Use()
    {
        if (Player_Stats.stats_instance)
        {
            if (healthRegenerationPoints != 0)
            {
                if (Player_Stats.stats_instance.playerHealth.GetCurrentHealthPoints() >= Player_Stats.stats_instance.playerHealth.GetTotalHealthPoints())
                {
                    return;
                }

                int tempHealth = Player_Stats.stats_instance.playerHealth.GetCurrentHealthPoints() + healthRegenerationPoints;
                if (tempHealth > Player_Stats.stats_instance.playerHealth.GetTotalHealthPoints())
                {
                    Player_Stats.stats_instance.playerHealth.SetCurrentHealthPoints(Player_Stats.stats_instance.playerHealth.GetTotalHealthPoints());
                }
                else
                {
                    Player_Stats.stats_instance.playerHealth.SetCurrentHealthPoints(tempHealth);
                }

            }

            if (energyRegenerationPoints != 0)
            {
                if (Player_Stats.stats_instance.playerEnergy.GetCurrentEnergyPoints() >= Player_Stats.stats_instance.playerEnergy.GetTotalEnergyPoints())
                {
                    return;
                }

                int tempEnergy = Player_Stats.stats_instance.playerEnergy.GetCurrentEnergyPoints() + energyRegenerationPoints;
                if (tempEnergy > Player_Stats.stats_instance.playerEnergy.GetTotalEnergyPoints())
                {
                    Player_Stats.stats_instance.playerEnergy.SetCurrentEnergyPoints(Player_Stats.stats_instance.playerEnergy.GetTotalEnergyPoints());
                }
                else
                {
                    Player_Stats.stats_instance.playerEnergy.SetCurrentEnergyPoints(tempEnergy);
                }

            }
        }
    }
}
