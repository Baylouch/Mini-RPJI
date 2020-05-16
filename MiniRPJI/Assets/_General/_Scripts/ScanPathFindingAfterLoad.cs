/* ScanPathFindingAfterLoad.cs :
 * 
 * Permet de faire un scan avec le A* de la scene courante afin de fixer le problème suivant :
 * 
 * Lorsque le joueur passe d'une scène A (déjà chargée) à une scène B (déjà chargée), le pathfinding rencontre un probleme de formation de nodes apparemment.
 * 
 * A Attaché sur le A* dans chaque scenes
 * */

using UnityEngine;

public class ScanPathFindingAfterLoad : MonoBehaviour
{
    private void OnEnable()
    {
        AstarPath.active = GetComponent<AstarPath>();
        AstarPath.active.Scan();
    }
}
