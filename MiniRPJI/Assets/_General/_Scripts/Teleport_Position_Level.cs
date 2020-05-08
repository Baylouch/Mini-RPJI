/* Teleport_Position_Level.cs :
 * Permet d'effectuer une teleportation lorsque le joueur change de game level. Afin qu'il soit placé au bonne endroit sur la scene
 * 
 * 
 * */

using UnityEngine;

public class Teleport_Position_Level : MonoBehaviour
{
    // Mettre le build index du niveau d'ou le joueur vient : 0 = la position de départ
    public int levelFromBuildIndex; // Permet de savoir d'où vient le joueur, et donc de trouver la bonne position lorsqu'il atteint le niveau.

}
