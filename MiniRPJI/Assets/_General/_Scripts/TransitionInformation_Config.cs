/* TransitionInformation_Config.cs (ScriptableObject)
 * 
 * Permet de définir une image et un texte pour une information liée au jeu qui sera affichée lors des écrans de transition.
 * 
 * */

using UnityEngine;

[CreateAssetMenu(fileName = "TransitionInfo", menuName = "ScriptableObjects/TransitionInformations/Information", order = 1)]
public class TransitionInformation_Config : ScriptableObject
{
    public Sprite informationSprite;

    [TextArea] public string informationText;
}
