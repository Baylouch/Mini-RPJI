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
    [SerializeField] GameObject interactionsUI; // THIS must have a Button component on it since Android adapation.
    [SerializeField] Text interactionsText;

    Button interactionButton;

    private void Start()
    {
        interactionButton = interactionsUI.GetComponent<Button>();
    }

    public void SetInteractionUI(string interactionText)
    {
        interactionsUI.SetActive(true);

        interactionsText.text = interactionText;

        // Set the button interaction
        // onClick.AddListener(...) so we need a way to pass the type of interaction via Player_Interactions.cs
    }

    public void ResetInteractionUI()
    {
        interactionsUI.SetActive(false);

        interactionsText.text = "";

        // Reset the button interaction
        if (interactionButton != null)
        {

        }
    }
}
