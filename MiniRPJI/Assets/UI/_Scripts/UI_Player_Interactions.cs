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
