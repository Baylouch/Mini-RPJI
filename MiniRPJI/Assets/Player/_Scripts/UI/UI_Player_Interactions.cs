/* UI_Player_Interactions.cs
 * 
 * Attaché au top hierarchie de l'UI du player (sur le meme gameobject que UI_Player)
 * 
 * Permet d'afficher à l'écran quand le joueur intéragit.
 * 
 * */

using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Interactions : MonoBehaviour
{
    [SerializeField] GameObject interactionsUI;
    [SerializeField] Text interactionsText;

    public void SetInteractionUI(string interactionText)
    {
        interactionsUI.SetActive(true);

        interactionsText.text = interactionText;
    }

    public void ResetInteractionUI()
    {
        interactionsUI.SetActive(false);

        interactionsText.text = "";
    }
}
