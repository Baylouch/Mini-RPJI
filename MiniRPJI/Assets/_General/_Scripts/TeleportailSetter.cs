/* TeleportailSetter.cs
 * 
 * Ce script a pour unique but de configurer le teleportail si le joueur l'as déjà débloqué auparavant dans une précédente partie.
 * 
 * En effet, le systeme de chargement de données fonctionne de tel sorte que le moyen le plus simple que j'ai trouvé est celui-ci.
 * 
 * Pour rappel : Quand le joueur charge des données, un gameobject persistent est créé, et existe jusqu'à ce que les données aient été transmises au scripts du joueur.
 * Dans ce sens, lorsque le joueur a débloqué des teleportails, il faut un moyen pour le détecter non pas au start mais quand le joueur a reçu les données.
 * Voulant que les teleportails soient responsables d'eux mêmes, et se trouvant dans des scenes différentes, je trouve ce moyen approprié.
 * 
 * 
 * */
using UnityEngine;

[RequireComponent(typeof(Teleportail))]
public class TeleportailSetter : MonoBehaviour
{
    Teleportail teleportail;

    // Start is called before the first frame update
    void Start()
    {
        teleportail = GetComponent<Teleportail>();
    }

    // Update is called once per frame
    void Update()
    {
        if (teleportail)
        {
            if (!teleportail.GetUnlocked() && UI_Player.instance)
            {
                if (UI_Player.instance.teleporterUI.GetUnlockedTp(teleportail.GetTeleportailID()))
                {
                    teleportail.ConfigUnlockedTP();
                    Destroy(this);
                }
            }

            if (teleportail.GetUnlocked())
            {
                Destroy(this);
            }
        }
    }
}
