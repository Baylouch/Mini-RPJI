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
    public int manaRegenerationPoints;

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

            if (manaRegenerationPoints != 0)
            {
                if (Player_Stats.stats_instance.getCurrentManaPoints() >= Player_Stats.stats_instance.getTotalManaPoints())
                {
                    return;
                }

                int tempMana = Player_Stats.stats_instance.getCurrentManaPoints() + manaRegenerationPoints;
                if (tempMana > Player_Stats.stats_instance.getTotalManaPoints())
                {
                    Player_Stats.stats_instance.SetCurrentManaPoints(Player_Stats.stats_instance.getTotalManaPoints());
                }
                else
                {
                    Player_Stats.stats_instance.SetCurrentManaPoints(tempMana);
                }

                used = true;
            }
        }
    }
}
