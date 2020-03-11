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

    public bool used = false;

    // For now we assume using an item will change something on player's stats.
    public void Use()
    {
        if (Player_Stats.stats_instance)
        {
            if (healthRegenerationPoints != 0)
            {
                if (Player_Stats.stats_instance.getCurrentHealthPoints() >= Player_Stats.stats_instance.getTotalHealthPoints())
                {
                    return;
                }

                int tempHealth = Player_Stats.stats_instance.getCurrentHealthPoints() + healthRegenerationPoints;
                if (tempHealth > Player_Stats.stats_instance.getTotalHealthPoints())
                {
                    Player_Stats.stats_instance.SetCurrentHealthPoints(Player_Stats.stats_instance.getTotalHealthPoints());
                }
                else
                {
                    Player_Stats.stats_instance.SetCurrentHealthPoints(tempHealth);
                }

                used = true;
            }

            if (energyRegenerationPoints != 0)
            {
                if (Player_Stats.stats_instance.getCurrentEnergyPoints() >= Player_Stats.stats_instance.getTotalEnergyPoints())
                {
                    return;
                }

                int tempMana = Player_Stats.stats_instance.getCurrentEnergyPoints() + energyRegenerationPoints;
                if (tempMana > Player_Stats.stats_instance.getTotalEnergyPoints())
                {
                    Player_Stats.stats_instance.SetCurrentEnergyPoints(Player_Stats.stats_instance.getTotalEnergyPoints());
                }
                else
                {
                    Player_Stats.stats_instance.SetCurrentEnergyPoints(tempMana);
                }

                used = true;
            }
        }
    }
}
